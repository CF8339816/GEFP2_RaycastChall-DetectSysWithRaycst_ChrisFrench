using UnityEngine;
using UnityEngine.InputSystem;
public class inputcontroler : MonoBehaviour
{

    [SerializeField] playercontroler CharacterController;
    [SerializeField] camLookControler CameraController;
    public InputAction moveAction;
    public InputAction lookAction;
    public InputAction jumpAction;
    public InputAction sprintAction;
    public InputAction crouchAction;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");  // adds the look action to the input controler script to assist binding , not that it works... fml
        lookAction = InputSystem.actions.FindAction("Look"); //                           ||
        jumpAction = InputSystem.actions.FindAction("Jump"); //                          ||
        sprintAction = InputSystem.actions.FindAction("Sprint"); //                           ||
        crouchAction = InputSystem.actions.FindAction("Crouch"); //                          ||


        Cursor.visible = false;
    }



}
