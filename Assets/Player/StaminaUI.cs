using UnityEngine;
using UnityEngine.UI;

public class StaminaUI : MonoBehaviour
{
    [SerializeField] private PlayerStamina playerStamina;
    [SerializeField] private Slider staminaSlider;

    private void Update()
    {
        if (playerStamina == null || staminaSlider == null)
            return;
		
        staminaSlider.value =
            playerStamina.CurrentStamina / playerStamina.MaxStamina;
    }
}