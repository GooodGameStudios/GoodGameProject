
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;

    [SerializeField] private float WalkSpeed = 5.0f;
    [SerializeField] private float RunSpeed = 9.0f;
    [SerializeField] private float Gravity = -20.0f;
    [SerializeField] private float JumpHeight = 1.0f;

    private Vector2 moveInput;

    //Do bobbingu
    public bool IsMoving => moveInput.magnitude > 0.1f;

    private float verticalVelocity;

    public bool isRunning;

    private bool jumpRequested;
    private bool isGrounded;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        PlayerMoving();
        CheckGround();
        ApplyGravity();
    }




    //Daje PlayerMoving kierunek ruchu
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    //Zaczyna i kończy bieg
    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.performed)
            isRunning = true;
        else if (context.canceled)
            isRunning = false;
    }

    //Pozwala na skos
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            jumpRequested = true;
        }
    }





    //Wprowadza postać w ruch
    private void PlayerMoving()
    {
        Vector3 direction = transform.right * moveInput.x + transform.forward * moveInput.y;
        direction = Vector3.ClampMagnitude(direction, 1f);

        float currentSpeed = isRunning ? RunSpeed : WalkSpeed;
        Vector3 horizontalVelocity = direction * currentSpeed;
        controller.Move(horizontalVelocity * Time.deltaTime);
    }

    //Raycast, sprawdza czy dotyka ziemi
    private void CheckGround()
    {
        Vector3 rayOrigin = controller.bounds.center;

        float rayLength = controller.bounds.extents.y + 0.30f;
        isGrounded = Physics.Raycast(rayOrigin, Vector3.down, rayLength, ~0, QueryTriggerInteraction.Ignore
        );
    }

    //Grawitacja
    private void ApplyGravity()
    {
        if (isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        if (jumpRequested)
        {
            if (isGrounded)
            {
                verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
            }
            jumpRequested = false;
        }
        verticalVelocity += Gravity * Time.deltaTime;
        controller.Move(Vector3.up * verticalVelocity * Time.deltaTime);
    }
}
