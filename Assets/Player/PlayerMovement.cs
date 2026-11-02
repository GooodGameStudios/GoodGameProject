
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

	[SerializeField] private LayerMask climbableLayer;
	[SerializeField] private float climbSpeed = 3f;
	[SerializeField] private float wallStickDistance = 0.5f;
	[SerializeField] private float jumpOffForce = 5f;
	[SerializeField] private float wallJumpUpForce = 7f;
	[SerializeField] private float wallJumpAwayForce = 5f;


    private Vector2 moveInput;

	private bool isClimbing;
	private Vector3 climbWallNormal;
	private bool wallJumpRequested;

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
		if (isClimbing)
		{
			if (wallJumpRequested)
			{
				WallJump();
				return;
			}

			if (!IsStillTouchingWall())
			{
				isClimbing = false;
				return;
			}

			ClimbingMovement();
		}
		else
		{
			PlayerMoving();
			CheckGround();
			ApplyGravity();
		}
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
			if (isClimbing)
			{
				wallJumpRequested = true;
			}
			else
			{
				jumpRequested = true;
			}
		}
	}

	//Wykrywa czy gracz dotyka odpowiedniej ściany
	private void OnControllerColliderHit(ControllerColliderHit hit)
	{
		if (isClimbing)
			return;

		int wallLayer = 1 << hit.gameObject.layer;

		if ((climbableLayer.value & wallLayer) != 0)
		{
			// Sprawdzamy, czy dotknęliśmy pionowej ściany
			if (Mathf.Abs(hit.normal.y) < 0.3f)
			{
				isClimbing = true;
				climbWallNormal = hit.normal;
				verticalVelocity = 0f;
			}
		}
	}

	//Skok ze ściany
	private void WallJump()
	{
		isClimbing = false;

		verticalVelocity = wallJumpUpForce;

		Vector3 jumpDirection =
			climbWallNormal * wallJumpAwayForce;

		controller.Move(
			jumpDirection * Time.deltaTime
		);

		wallJumpRequested = false;
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

	private void ClimbingMovement()
	{
		// Kierunek góra/dół po ścianie
		Vector3 verticalDirection = Vector3.up * moveInput.y;

		// Kierunek lewo/prawo po ścianie
		Vector3 horizontalDirection = Vector3.Cross(
			climbWallNormal,
			Vector3.up
		) * moveInput.x;

		Vector3 climbDirection = verticalDirection + horizontalDirection;

		climbDirection = Vector3.ClampMagnitude(climbDirection, 1f);

		controller.Move(climbDirection * climbSpeed * Time.deltaTime);

		// Przyklejanie do ściany
		controller.Move(-climbWallNormal * wallStickDistance * Time.deltaTime);
	}

	private bool IsStillTouchingWall()
	{
		Vector3 origin = controller.bounds.center;

		return Physics.Raycast(
			origin,
			-climbWallNormal,
			wallStickDistance + 0.3f,
			climbableLayer,
			QueryTriggerInteraction.Ignore
		);
	}
}
