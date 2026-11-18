using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

// Logika ataku graczem - wymaga trzymanej bron (WeaponData), pilnuje cooldownu
// i zadaje obrazenia celowi, na ktory aktualnie patrzy gracz (PlayerInteractor).
// Animacja wachniecia (HandsAnimationController) reaguje na OnAttackPerformed,
// zamiast na surowy input - dzieki temu nie odgrywa sie bez trzymanej broni.
public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private PlayerInteractor interactor;
    [SerializeField] private Inventory inventory;
    [SerializeField] private float attackCooldown = 0.6f;

    private bool isOnCooldown;

    // Wywolywane, gdy atak faktycznie sie odbywa (bron w rece, cooldown gotowy) - niezaleznie od tego, czy cos trafil.
    public static event Action OnAttackPerformed;

    // Callback z Input Systemu - wywolywany po wcisnieciu klawisza ataku.
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        TryAttack();
    }

    private void TryAttack()
    {
        if (isOnCooldown) return;
        if (inventory.ActiveSlot?.Item is not WeaponData weapon) return;

        StartCoroutine(CooldownRoutine());
        OnAttackPerformed?.Invoke();

        HitCurrentTarget(weapon);
    }

    private IEnumerator CooldownRoutine()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(attackCooldown);
        isOnCooldown = false;
    }

    // Zadaje obrazenia celowi z PlayerInteractor i, jesli ten ma Rigidbody, odrzuca go (knockback).
    private void HitCurrentTarget(WeaponData weapon)
    {
        if (interactor == null || interactor.CurrentTarget == null) return;

        GameObject target = interactor.CurrentTarget.gameObject;

        if (target.TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage(weapon.Damage);
        }

        if (weapon.Knockback > 0f && target.TryGetComponent(out Rigidbody rb))
        {
            Vector3 direction = interactor.InteractionOrigin.forward;
            rb.AddForce(direction * weapon.Knockback, ForceMode.Impulse);
        }
    }
}
