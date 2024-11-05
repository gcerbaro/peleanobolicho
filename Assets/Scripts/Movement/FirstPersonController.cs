using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.Rendering;
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
    
    [Header("Controls")] 
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode crouchKey = KeyCode.Space;

    [Header("Movement parameters")] 
    [SerializeField] private float walkSpeed = 2.0f;
    [SerializeField] private float sprintSpeed = 6.0f;
    [SerializeField] private float speedSmoothTime = 0.15f;
    
    [Header("Look parameters")]
    [SerializeField] private float sensitivity = 2f;
    
    [Header("Jump parameters")]
    [SerializeField] private float jumpForce = 4.0f;
    [SerializeField] private float gravity = -9.81f;
    
    [Header("Crouch parameters")]
    [SerializeField] private float crouchHeight = 0.5f;
    [SerializeField] private float standingHeight = 0.5f;
    [SerializeField] private float timeToCrouch = 0.5f;
    [SerializeField] private Vector3 crouchingCenter = new Vector3(0, 0.5f, 0);
    [SerializeField] private Vector3 standingCenter = new Vector3(0, 0, 0);
    
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
    
    private float _rotationX = 0f;
    
    void Awake()
    {
        _playerCamera = GetComponentInChildren<Camera>();
        _characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    void Update()
    {
        if (CanMove)
        {
            HandleMovementInput();
            HandleMouseLook();

            if (canJump) 
                HandleJump();
            
            if (canCrouch)
                HandleCrouch();
            
            ApplyFinalMovements();
        }
    }

    private void HandleMovementInput()
    {
        //seta a velocidade buscada
        _targetSpeed = IsSprinting ? sprintSpeed : walkSpeed;
        
        _currentSpeed = Mathf.SmoothDamp(_currentSpeed, _targetSpeed, ref _speedSmoothVelocity, speedSmoothTime);

        // Atualiza _currentInput com a velocidade interpolada
        _currentInput = new Vector2(_currentSpeed * Input.GetAxis("Vertical"), Input.GetAxis("Horizontal") * _currentSpeed);

        
        Debug.Log("Velocidade do personagem: " + _characterController.velocity.magnitude);

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
        float targetHeight = _isCrouching ? standingHeight : crouchHeight;
        float currentHeight = _characterController.height;
        Vector3 targetCenter = _isCrouching ? standingCenter : crouchingCenter;
        Vector3 currentCenter = _characterController.center;

        while (timeElapsed < timeToCrouch)
        {
            _characterController.height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed / timeToCrouch);
            _characterController.center = Vector3.Lerp(currentCenter, targetCenter, timeElapsed / timeToCrouch);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        
        _characterController.height = targetHeight;
        _characterController.center = targetCenter;
        
        _isCrouching = !_isCrouching;
        
        _duringCrouchAnimation = false;
    }
}
