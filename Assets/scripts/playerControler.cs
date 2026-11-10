using TMPro;
using UnityEngine;

public class playercontroler : MonoBehaviour
{
    [Header("References")]
    public CharacterController characterController;
    public Camera firstPersonCam; // Assign your main player camera here in Inspector
    //public Camera grappleCamera;

    [Header("Movement Settings")]
    [SerializeField] float MoveSpeed = 10f;
    [SerializeField] float SprintSpeed = 15f;
    [SerializeField] float CrouchSpeed = 5f;
    //[SerializeField] float RotateSpeed = 5f;
    [SerializeField] float gravity = -9.8f;
    [SerializeField] float jumpHeight = 2f;
    [SerializeField] float StandHeight = 2f;
    [SerializeField] float CrouchHeight = 1f;
    [SerializeField] float movementSmoothTime = 0.1f;
    [SerializeField] float mouseSensitivity = 100f;

    [Header("Ground Check")]
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundDistance = 0.2f;
    [SerializeField] LayerMask groundMask;

    [Header("Status")]
    public bool isGrounded;
    //[SerializeField] public bool PowerOn;

    private Vector3 velocity;
    private bool isSprinting = false;
    private bool isCrouching = false;
    private float xRotation = 0f;
    private float targetSpeed;
    private float currentHorizontalSpeed;
    private Vector3 currentMovementInput;
    private float smoothMoveVelocity; // Changed to float to match the float SmoothDamp

    void Start()
    {
        if (characterController == null)
            characterController = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        characterController.height = StandHeight;

        // Default speeds
        targetSpeed = MoveSpeed;
        currentHorizontalSpeed = MoveSpeed;
        //PowerOn = true;
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
             
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        if (firstPersonCam != null)
        {
            firstPersonCam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
        transform.Rotate(Vector3.up * mouseX);
        
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        currentMovementInput = transform.right * x + transform.forward * z;

        if (currentMovementInput.magnitude > 1)
        {
            currentMovementInput.Normalize();
        }

        HandleSpeedChanges();
        
        currentHorizontalSpeed = Mathf.SmoothDamp(currentHorizontalSpeed, targetSpeed, ref smoothMoveVelocity, movementSmoothTime); 
        characterController.Move(currentMovementInput * currentHorizontalSpeed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    public void SetVerticalVelocity(float y)
    {
        velocity.y = y;
    }

    public void ResetVelocity()
    {
        velocity = Vector3.zero;
    }

    private void HandleSpeedChanges()
    {
        // Check Inputs
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            isSprinting = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
        {
            isSprinting = false;
        }
        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
        {
            isCrouching = true;
            characterController.height = CrouchHeight;
        }
        if (Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
        {
            isCrouching = false;
            characterController.height = StandHeight;
        }

        // Apply state evaluations
        if (isCrouching)
        {
            targetSpeed = CrouchSpeed;
            isSprinting = false;
        }
        else if (isSprinting)
        {
            targetSpeed = SprintSpeed;
        }
        else
        {
            targetSpeed = MoveSpeed;
        }
    }
}