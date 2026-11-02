using UnityEngine;
using UnityEngine.InputSystem;

public class CameraHandler : MonoBehaviour
{
	[SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private Transform cameraTarget;
	[SerializeField] private CharacterController controller;


    [SerializeField] private float mouseSensitivity = 0.1f;
    [SerializeField] private int UpCameraLookBlock = 90;
    [SerializeField] private int DownCameraLookBlock = -90;


	[SerializeField] private float walkBobFrequency = 8.0f;
	[SerializeField] private float runBobFrequency = 13.0f;

	[SerializeField] private float walkBobAmplitude = 0.07f;
	[SerializeField] private float runBobAmplitude = 0.2f;

	[SerializeField] private float bobSmoothness = 8.0f;

	private Vector3 initialCameraPosition;
	private float bobTimer;

    private float cameraRotationX;
    private Vector2 lookInput;




	private void Start()
	{
		if (cameraTarget != null)
			initialCameraPosition = cameraTarget.localPosition;
	}

	private void Update()
	{
		CameraLook();
		CameraBobbing();
		
	}





    //Daje CameraLook vektor w którą strone patrzeć
    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }





    //Nadaje ruch kamery
    private void CameraLook()
    {
        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;

        cameraRotationX -= mouseY;
        cameraRotationX = Mathf.Clamp(cameraRotationX, DownCameraLookBlock, UpCameraLookBlock);

        if (cameraTarget != null)
        {
            cameraTarget.localRotation =
                Quaternion.Euler(cameraRotationX, 0f, 0f);
        }
        transform.Rotate(0f, mouseX, 0f);
    }

	//Niech Unity spłonie w piekle
	private void CameraBobbing()
	{
		if (cameraTarget == null || controller == null || playerMovement == null)
			return;

		float frequency = playerMovement.isRunning ? runBobFrequency : walkBobFrequency;
		float amplitude = playerMovement.isRunning ? runBobAmplitude : walkBobAmplitude;

		if (playerMovement.IsMoving && controller.isGrounded)
		{
			bobTimer += Time.deltaTime * frequency;

			float bobOffset = Mathf.Sin(bobTimer) * amplitude;
			Vector3 targetPosition = initialCameraPosition;
			targetPosition.y += bobOffset;

			cameraTarget.localPosition = Vector3.Lerp(cameraTarget.localPosition, targetPosition, bobSmoothness * Time.deltaTime);
		}
		else
		{
			bobTimer = 0f;
			cameraTarget.localPosition = Vector3.Lerp(cameraTarget.localPosition, initialCameraPosition, bobSmoothness * Time.deltaTime);
		}
	}

}