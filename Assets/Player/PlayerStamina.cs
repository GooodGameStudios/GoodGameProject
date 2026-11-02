using UnityEngine;

public class PlayerStamina : MonoBehaviour
{
    [Header("Stamina")]
    [SerializeField] private float maxStamina = 100.0f;
    [SerializeField] private float runStaminaDrain = 15.0f;
    [SerializeField] private float staminaRegenRate = 20.0f;
	[SerializeField] private float staminaRegenDelay = 2.0f;

	private float staminaRegenTimer;

    private float currentStamina;

    public float CurrentStamina => currentStamina;
    public float MaxStamina => maxStamina;


	private void Update()
    {
        Debug.Log(currentStamina);
    }

    private void Awake()
    {
        currentStamina = maxStamina;
    }





	//Check
	public bool UseStamina(float amount)
	{
		if (currentStamina < amount)
			return false;

		currentStamina -= amount;
		currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
		
		staminaRegenTimer = staminaRegenDelay;
		return true;
	}

	//Aktualizuje poziom staminy
	 public void UpdateStamina(bool isRunning, bool isMoving)
	{
		if (isRunning && isMoving)
		{
			currentStamina -= runStaminaDrain * Time.deltaTime;
			staminaRegenTimer = staminaRegenDelay;
		}
		else
		{
			if (staminaRegenTimer > 0f)
			{
				staminaRegenTimer -= Time.deltaTime;
			}
			else
			{
				currentStamina += staminaRegenRate * Time.deltaTime;
			}
		}
		currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
	}
}