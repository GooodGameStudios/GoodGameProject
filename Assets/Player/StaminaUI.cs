using UnityEngine;
using UnityEngine.UI;

public class StaminaUI : MonoBehaviour
{
    [SerializeField] private Slider staminaSlider;


    private void OnEnable()
    {
        PlayerStamina.OnStaminaChanged += UpdateStaminaUI;
    }

    private void OnDisable()
    {
        PlayerStamina.OnStaminaChanged -= UpdateStaminaUI;
    }

	//
    private void UpdateStaminaUI(float currentStamina, float maxStamina)
    {
        if (staminaSlider == null)
            return;
        if (maxStamina <= 0f)
        {
            staminaSlider.value = 0f;
            return;
        }
        staminaSlider.value = currentStamina / maxStamina;
    }
}