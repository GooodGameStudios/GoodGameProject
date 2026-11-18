using UnityEngine;

// Odtwarza animacje rak gracza (f_hands_01) w odpowiedzi na akcje gracza.
// Rece sa renderowane glowna kamera gracza, bez dedykowanej HandsCamera.
public class HandsAnimationController : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private Animator handsAnimator;
    [SerializeField] private string swingTrigger = "HitPickaxe";
    [SerializeField] private string pickUpTrigger = "PickUp";
    [SerializeField] private string pullPickaxeTrigger = "PullPickaxe";
    [SerializeField] private string hidePickaxeTrigger = "HidePickaxe";

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

    // Uruchamiane po wybraniu slotu na hotbarze - reka wyciaga albo chowa pickaxe,
    // w zaleznosci od tego, czy nowo wybrany slot trzyma narzedzie typu WeaponData.
    private void HandleActiveSlotSelected(int slotIndex)
    {
        if (handsAnimator == null) return;

        bool hasWeapon = inventory.ActiveSlot?.Item is WeaponData;
        handsAnimator.SetTrigger(hasWeapon ? pullPickaxeTrigger : hidePickaxeTrigger);
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
        if (handsAnimator != null) handsAnimator.SetTrigger(pickUpTrigger);
    }
}
