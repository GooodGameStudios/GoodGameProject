using UnityEngine;
using UnityEngine.InputSystem;

// Odtwarza animacje rak gracza (f_hands_01) w odpowiedzi na akcje gracza
// i zarzadza widocznoscia modelu rak oraz dedykowanej HandsCamera - obie
// sa wylaczone na starcie i wlaczane dopiero przy pierwszym podniesieniu przedmiotu.
[RequireComponent(typeof(PlayerInput))]
public class HandsAnimationController : MonoBehaviour
{
    [SerializeField] private Animator handsAnimator;
    [SerializeField] private GameObject handsCamera;
    [SerializeField] private string swingTrigger = "Swing";
    [SerializeField] private string pickUpTrigger = "PickUp";

    private void Awake()
    {
        if (handsCamera != null) handsCamera.SetActive(false);
    }

    private void OnEnable()
    {
        PlayerItemPickup.OnItemPickedUp += HandleItemPickedUp;
    }

    private void OnDisable()
    {
        PlayerItemPickup.OnItemPickedUp -= HandleItemPickedUp;
    }

    // Wywolywane przez PlayerInput (Send Messages) na akcji "Attack".
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (handsAnimator == null) return;

        handsAnimator.SetTrigger(swingTrigger);
    }

    // Uruchamiane po udanym podniesieniu przedmiotu ze swiata (PlayerItemPickup).
    private void HandleItemPickedUp()
    {
        if (handsCamera != null) handsCamera.SetActive(true);
        if (handsAnimator != null) handsAnimator.SetTrigger(pickUpTrigger);
    }
}
