using System.Collections;
using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    private bool CanMove { get; set; } = true;
    private bool IsSprinting => canSprint && Input.GetKey(sprintKey);
    private bool ShouldJump => Input.GetKeyDown(jumpKey) && _characterController.isGrounded; 
    private bool ShouldCrouch => Input.GetKeyDown(crouchKey) && !_duringCrouchAnimation && _characterController.isGrounded;
    
    [Header("Functional Options")]
    [SerializeField] private bool canSprint = true;
    [SerializeField] private bool canJump = true;
    [SerializeField] private bool canCrouch = true;
    [SerializeField] private bool canUseHeadbob = true;
    [SerializeField] private bool canInteract = true;
    [SerializeField] private bool useStamina = true;
    [SerializeField] private bool useFootsteps = true;
    
    [Header("Controls")] 
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    [Header("Movement parameters")] 
    [SerializeField] private float walkSpeed = 2.0f;
    [SerializeField] private float speedSmoothTime = 0.15f;
    private float sprintSpeed = 6.0f;
    private float crouchSpeed = 3.0f;
    
    [Header("Look parameters")]
    [SerializeField] private float sensitivity = 2f;
    
    [Header("Jump parameters")]
    [SerializeField] private float jumpForce = 4.0f;
    [SerializeField] private float gravity = -9.81f;
    
    //crouching
    private float _crouchHeight = 0.5f;
    private float _standingHeight = 2f;
    private float _timeToCrouch = 0.25f;
    private Vector3 _crouchingCenter = new Vector3(0, 0.5f, 0);
    private Vector3 _standingCenter = new Vector3(0, 0, 0);
    
    [Header("Headbob parameters")] 
    [SerializeField] private float walkBobSpeed = 14f;
    [SerializeField] private float walkBobAmount = 0.05f;
    [SerializeField] private float sprintBobSpeed = 18f;
    [SerializeField] private float sprintBobAmount = 0.01f;
    [SerializeField] private float crouchBobSpeed = 8f;
    [SerializeField] private float crouchBobAmount = 0.025f;
    
    [Header("Footstep parameters")]
    [SerializeField] private float baseStepSpeed = 0.5f;
    [SerializeField] private float crouchStepMultiplier = 1.5f;
    [SerializeField] private float speedStepMultiplier = 0.6f;
    [SerializeField] private AudioClip woodFootstepSound;
    [SerializeField] private AudioClip stoneFootstepSound;
    
    //Variaveis dos passos
    private float _footstepTimer = 0f;
    private float _footstepVolume = 0.25f;
    private float GetCurrentOffset => _isCrouching ? baseStepSpeed * crouchStepMultiplier : 
                                       IsSprinting ? baseStepSpeed * speedStepMultiplier : baseStepSpeed;
    
    [Header("Interaction")] 
    private Vector3 _interactionRayPoint = new Vector3(0.5f, 0.5f, 0);
    private float _interactionDistance = 2f;
    private LayerMask _interactionLayer;
    
    //Animator
    private Animator _animator;
    private static readonly int Speed = Animator.StringToHash("Speed");
    private float _blendValue = 0.0f; // Valor atual do parâmetro de blend
    
    //Health
    HealthSystem _healthSystem;
    private float _currentHealth;
    private float _maxHealth;
    
    //Stamina
    StaminaSystem _staminaSystem;
    private float _currentStamina;
    
    //Interaction
    private Interactable _currentInteractable;
    
    //headBob
    private float _defaultYpos;
    private float _timer;
    
    //Crouching
    private bool _isCrouching;
    private bool _duringCrouchAnimation;
    
    //Variaveis de velocidade
    private float _targetSpeed; 
    private float _currentSpeed; 
    private float _speedSmoothVelocity;  
    
    //Limite para visualizacao vertical
    private float _limit = 90f;
    private Camera _playerCamera;
    private CharacterController _characterController;

    private Vector3 _moveDirection;
    private Vector2 _currentInput;
    
    private float _rotationX;
    
    void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _interactionLayer = LayerMask.GetMask("Interactable");
        _staminaSystem = FindObjectOfType<StaminaSystem>();
        _healthSystem = FindObjectOfType<HealthSystem>();
        _playerCamera = GetComponentInChildren<Camera>();
        _characterController = GetComponent<CharacterController>();
        
        _defaultYpos = _playerCamera.transform.localPosition.y;
        
        // Inicializa variáveis de saúde
        _currentHealth = _healthSystem.CurrentHealth;
        _maxHealth = _healthSystem.MaxHealth;
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        //movement variables
        sprintSpeed = walkSpeed * 2;
        crouchSpeed = walkSpeed * 0.5f;
    }
    
    void Update()
    {
        if (CanMove)
        {
            HandleMovementInput();
            HandleMouseLook();
            HandleAnimations();
            UpdateHealth();

            if (canJump) 
                HandleJump();
            
            if (canCrouch)
                HandleCrouch();

            if (canUseHeadbob)
                HandleHeadbob();

            if (useFootsteps)
                HandleFootsteps();
            
            if (canInteract)
            {
                HandleInteractionCheck();
                HandleInteractionInput();
            }

            if (useStamina)
                HandleStamina();
            
            ApplyFinalMovements();
        }
    }

    private void HandleMovementInput()
    {
        //seta a velocidade buscada
        _targetSpeed = IsSprinting ? sprintSpeed : (_isCrouching ? crouchSpeed : walkSpeed);
        
        //Armazena velocidade atual e usa speedsmoothVelocity para desacelerar ou acelerar player.
        _currentSpeed = Mathf.SmoothDamp(_currentSpeed, _targetSpeed, ref _speedSmoothVelocity, speedSmoothTime);

        // Atualiza _currentInput com a velocidade 
        _currentInput = new Vector2(_currentSpeed * Input.GetAxis("Vertical"), Input.GetAxis("Horizontal") * _currentSpeed);
        
        float moveDirectionY = _moveDirection.y;
        _moveDirection = (transform.TransformDirection(Vector3.forward) * _currentInput.x) + (transform.TransformDirection(Vector3.right) * _currentInput.y);
        _moveDirection.y = moveDirectionY;
    }

    private void HandleMouseLook()
    {
        _rotationX -= Input.GetAxis("Mouse Y") * sensitivity;
        _rotationX = Mathf.Clamp(_rotationX, -_limit, _limit);
        
        _playerCamera.transform.localRotation = Quaternion.Euler(_rotationX, 0f, 0f);
        transform.rotation *= Quaternion.Euler(0f, Input.GetAxis("Mouse X") * sensitivity, 0f);
    }

    private void HandleAnimations()
    {
        if (IsSprinting)
        {
            _blendValue = Mathf.Clamp(_blendValue + Time.deltaTime, 0f, 1f); // Incrementa o valor até 1
        }
        else
        {
            _blendValue = Mathf.Clamp(_blendValue - Time.deltaTime, 0f, 1f); // Decrementa o valor até 0
        }

        // Atualiza o parâmetro de speed no Animator
        if (!_animator) Debug.LogError("Animator não encontrado!");
        _animator.SetFloat(Speed, _blendValue);
    }
    
    private void UpdateHealth()
    {
        _currentHealth = _healthSystem.CurrentHealth;
        _maxHealth = _healthSystem.MaxHealth;
    }
    
    private void HandleJump()
    {
        if (ShouldJump)
            _moveDirection.y = jumpForce; 
            
    }
    
    private void HandleCrouch()
    {
        if (ShouldCrouch)
            StartCoroutine(CrouchStand());
    }
    
    private void HandleHeadbob()
    {
        if (!_characterController.isGrounded) return;

        if (Mathf.Abs(_moveDirection.x) > 0.1f || Mathf.Abs(_moveDirection.z) > 0.1f)
        {
            _timer += Time.deltaTime * (_isCrouching ? crouchBobSpeed : IsSprinting ? sprintBobSpeed : walkBobSpeed);
            _playerCamera.transform.localPosition = new Vector3(
                _playerCamera.transform.localPosition.x,
                _defaultYpos + Mathf.Sin(_timer) * 
                (_isCrouching ? crouchBobAmount : IsSprinting ? sprintBobAmount : walkBobAmount),
                _playerCamera.transform.localPosition.z);
        }
    }

    private void HandleFootsteps()
    {
        if (!_characterController.isGrounded) return;
        if (_currentInput == Vector2.zero) return;
        
        _footstepTimer -= Time.deltaTime;

        if (_footstepTimer <= 0)
        {
            if (Physics.Raycast(_playerCamera.transform.position, Vector3.down, out RaycastHit hit, 3))
            {
                switch (hit.collider.tag)
                {
                    case "Footsteps/Wood":
                        SoundFXManager.instance.PlaySoundEffect(woodFootstepSound, transform, _footstepVolume);
                        break;
                    case "Footsteps/Stone":
                        SoundFXManager.instance.PlaySoundEffect(stoneFootstepSound, transform, _footstepVolume);
                        break;
                }
            }

            _footstepTimer = GetCurrentOffset;
        }
    }
    
    private void HandleInteractionCheck()
    {
        if (Physics.Raycast(_playerCamera.ViewportPointToRay(_interactionRayPoint), out RaycastHit hit,
                _interactionDistance))
        {
            if (hit.collider.gameObject.layer == 6 && (!_currentInteractable || hit.collider.gameObject.GetInstanceID() != _currentInteractable.GetInstanceID()))
            {
                hit.collider.TryGetComponent(out _currentInteractable);
                
                if (_currentInteractable)
                    _currentInteractable.OnFocus();
            }
        }
        else if (_currentInteractable)
        {
            _currentInteractable.OnLoseFocus();
            _currentInteractable = null;
        }
    }

    private void HandleInteractionInput()
    {
        if (Input.GetKeyDown(interactKey) && _currentInteractable &&
            Physics.Raycast(_playerCamera.ViewportPointToRay(_interactionRayPoint), out RaycastHit hit,
                _interactionDistance, _interactionLayer))
        {
            _currentInteractable.OnInteract();
        }
    }

    private void HandleStamina()
    {
        
        if (_currentInput != Vector2.zero && IsSprinting)
        {
            _staminaSystem.UseStamina(Time.deltaTime); 

          
            if (_staminaSystem.CurrentStamina <= 0)
            {
                canSprint = false;
            }
        }
        else
        {
            _staminaSystem.StartRegen();
            
            if (_staminaSystem.CurrentStamina > 0)
            {
                canSprint = true;
            }
        }
    }
    
    private void ApplyFinalMovements()
    {
        if (!_characterController.isGrounded) 
            _moveDirection.y += gravity * Time.deltaTime;
        
        _characterController.Move(_moveDirection * Time.deltaTime);
    }
    
    private IEnumerator CrouchStand()
    {
        if (_isCrouching && Physics.Raycast(_playerCamera.transform.position, Vector3.up, 1f))
            yield break;
            
        _duringCrouchAnimation = true;

        float timeElapsed = 0f;
        float targetHeight = _isCrouching ? _standingHeight : _crouchHeight;
        float currentHeight = _characterController.height;
        Vector3 targetCenter = _isCrouching ? _standingCenter : _crouchingCenter;
        Vector3 currentCenter = _characterController.center;

        while (timeElapsed < _timeToCrouch)
        {
            _characterController.height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed / _timeToCrouch);
            _characterController.center = Vector3.Lerp(currentCenter, targetCenter, timeElapsed / _timeToCrouch);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        
        _characterController.height = targetHeight;
        _characterController.center = targetCenter;
        
        _isCrouching = !_isCrouching;
        
        _duringCrouchAnimation = false;
    }
}