using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public class playercontroler : MonoBehaviour
{
    public CharacterController characterController;
    [SerializeField] float MoveSpeed = 10f;
    [SerializeField] float SprintSpeed = 15f;
    [SerializeField] float CrouchSpeed = 5f;
    [SerializeField] float RotateSpeed = 5f;
    [SerializeField] float gravity = -9.8f; //has to be neg because is downward force
    [SerializeField] float jumpHeight = 2f;
    [SerializeField] float StandHeight = 2f; // default ht of the character
    [SerializeField] float CrouchHeight = 1f; // target ht when crouched
    //[SerializeField] public float Health = 1f;
    [SerializeField] float accelerationRate = 5f; //accelleration and decelleration rate
    [SerializeField] float movementSmoothTime = 0.1f; //time the accel & decel takes
    //[SerializeField] public Transform ActiveCheckPoint;
    [SerializeField] float mouseSensitivity = 100f;
    [SerializeField] Transform groundCheck; //checks the ground  objgect
    [SerializeField] float groundDistance = 0.2f; //grounding variance
    [SerializeField] LayerMask groundMask;

    private Vector3 velocity;
    public bool isGrounded;
    private bool isSprinting = false;
    private bool isCrouching = false;
    private float currentSpeed;
    private float xRotation = 0f;
    public Vector3 externalVelocity;
    //public TextMeshProUGUI textPowerCells;
    //public TextMeshProUGUI textGrappleGun;
    //public GrappleStatus grappleStatus;
    private float targetSpeed;
    private float currentHorizontalSpeed;
    private Vector3 currentMovementInput;
    private Vector3 smoothMoveVelocity; // vector for the SmoothDamp function
    //private GameObject PausedLevel; //stores current levvel duuring pause
    //public GameObject PauseScreen;
    ///GrappleStatus Vars
    public playercontroler playerScript;
    //public Transform CheckPointGround;
    //public Transform CheckPointTowerTop;
    //public Transform CheckPointTowerMid;
    public Camera firstPersonCam;
    public Camera grappleCamera;
    /// </summary>
    //private int CellCount;
    [SerializeField] public bool PowerOn;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked; // lock cursor to center of screen
        currentSpeed = MoveSpeed; // presets speed at base move
        characterController.height = StandHeight;
        targetSpeed = MoveSpeed;
        currentHorizontalSpeed = MoveSpeed;
        //CellCount = 0;
        PowerOn = true;
        //SetTextPowerCells();
        //grappleStatus.SetTextGrappleGun();
        //if (ActiveCheckPoint == null)
        //{
        //    //ActiveCheckPoint = CheckPointGround;
        //}
       
    }
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask); //grounded check
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -8f; // small downward force to keep grounded
        }
        else
        {
            isGrounded = false;
        }

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;// mouse rotation
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // clamp vertical look

        Camera.main.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        float x = Input.GetAxis("Horizontal");  //movement
        float z = Input.GetAxis("Vertical");

        currentMovementInput = transform.right * x + transform.forward * z;

        if (currentMovementInput.magnitude > 1)// normalizes movement input in order to prevent diagional magnatude speed increases
        {
            currentMovementInput.Normalize();
        }

        characterController.Move(currentMovementInput * currentHorizontalSpeed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded) // jump using input from input System
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
           
        }

        velocity.y += gravity * Time.deltaTime; // applies gravity to character object
        characterController.Move(velocity * Time.deltaTime);

        HandleSpeedChanges();// calls the method below that has been used to adjust the sprint and crouch speeds  and limit sprinting while crouched

        currentHorizontalSpeed = Mathf.SmoothDamp(currentHorizontalSpeed, targetSpeed, ref smoothMoveVelocity.x, movementSmoothTime);//  uses SmoothDamp to adjust ease in and out of horizontal movements
    }
    //void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.CompareTag("PickUp")) //checks obj ffor PickUp tag
    //    {
    //        other.gameObject.SetActive(false); //deactivates obj when collided
    //        //CellCount++; //adds 1 to count when picked up
    //        //SetTextPowerCells();   //calls SetCountText method
    //    }
    //}
    public void SetVerticalVelocity(float y)
    {
        velocity.y = y;
    }
    public void ResetVelocity()
    {
        velocity = Vector3.zero;
    }
    //void SetTextPowerCells()
    //{
    //    textPowerCells.text = "Power Cells collected: " + CellCount.ToString() + "/8"; // sets count to output to string

    //    if (CellCount == 8)
    //    {
    //        PowerOn = true;
    //        textGrappleGun.text = "Grapple Gun Powered: Yes \ncenter mouse to grapple \n right mouse to grapple and pull object";
    //    }

    //    else
    //    {
    //        PowerOn = false;
    //        textGrappleGun.text = "Grapple Gun Powered: No";
    //    }
    //}
    private void HandleSpeedChanges()
    {
        if (isCrouching)
        {
            targetSpeed = CrouchSpeed;
            isSprinting = false; // cannot sprint while crouched
        }
        else if (isSprinting)
        {
            targetSpeed = SprintSpeed;
        }
        else
        {
            targetSpeed = MoveSpeed;
        }

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
            {
                isCrouching = false;
                characterController.height = StandHeight;
            }
        }
    }

}