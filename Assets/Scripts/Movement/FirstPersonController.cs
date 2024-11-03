using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.Rendering;
using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    private bool CanMove { get; set; } = true;
    private bool _isSprinting => canSprint && Input.GetKey(sprintKey);
    
    [Header("Functional Options")]
    [SerializeField] private bool canSprint = true;
    
    [Header("Controls")] 
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;

    [Header("Movement parameters")] 
    [SerializeField] private float walkSpeed = 2.0f;
    [SerializeField] private float sprintSpeed = 6.0f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float speedSmoothTime = 0.15f;
    
    [Header("Look parameters")]
    [SerializeField] private float sensitivity = 2f;
    
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
            
            ApplyFinalMovements();
        }
    }

    private void HandleMovementInput()
    {
        //seta a velocidade buscada
        _targetSpeed = _isSprinting ? sprintSpeed : walkSpeed;
        
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

    private void ApplyFinalMovements()
    {
        if (!_characterController.isGrounded) 
            _moveDirection.y += gravity * Time.deltaTime;
        
        _characterController.Move(_moveDirection * Time.deltaTime);
    }
}
