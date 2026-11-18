using UnityEngine;

// Odtwarza animacje rak gracza (f_hands_01) w odpowiedzi na akcje gracza
// i zarzadza widocznoscia modelu rak oraz dedykowanej HandsCamera - obie
// sa wylaczone na starcie i wlaczane dopiero przy pierwszym podniesieniu przedmiotu.
public class HandsAnimationController : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private Animator handsAnimator;
    [SerializeField] private GameObject handsCamera;
    [SerializeField] private string swingTrigger = "HitPickaxe";
    [SerializeField] private string pickUpTrigger = "PickUp";
    [SerializeField] private string pullPickaxeTrigger = "PullPickaxe";

    private void Awake()
    {
        if (handsCamera != null) handsCamera.SetActive(false);
    }

    private void OnEnable()
    {
        PlayerItemPickup.OnItemPickedUp += HandleItemPickedUp;
        Inventory.OnActiveSlotSelected += HandleActiveSlotSelected;
        PlayerAttack.OnAttackPerformed += HandleAttackPerformed;
    }

    private void OnDisable()
    {
        PlayerItemPickup.OnItemPickedUp -= HandleItemPickedUp;
        Inventory.OnActiveSlotSelected -= HandleActiveSlotSelected;
        PlayerAttack.OnAttackPerformed -= HandleAttackPerformed;
    }

    // Uruchamiane po wybraniu slotu na hotbarze - reka pojawia sie (lub chowa),
    // w zaleznosci od tego, czy nowo wybrany slot trzyma narzedzie typu WeaponData.
    private void HandleActiveSlotSelected(int slotIndex)
    {
        bool hasWeapon = inventory.ActiveSlot?.Item is WeaponData;

        if (handsCamera != null) handsCamera.SetActive(hasWeapon);
        if (hasWeapon && handsAnimator != null) handsAnimator.SetTrigger(pullPickaxeTrigger);
    }

    // Uruchamiane, gdy PlayerAttack faktycznie wykona atak (bron w rece, cooldown gotowy).
    private void HandleAttackPerformed()
    {
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
