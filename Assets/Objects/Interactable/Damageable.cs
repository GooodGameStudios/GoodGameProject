using UnityEngine;
using UnityEngine.Events;

// Generyczny cel ataku (np. ruda, skala) - trzyma pasek HP i wystawia UnityEventy,
// zeby efekty/drop/zniszczenie mozna bylo podpiac w Inspectorze per-prefab,
// bez kolejnego skryptu na kazdy typ obiektu.
public class Damageable : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 50f;
    [SerializeField] private UnityEvent onDamaged;
    [SerializeField] private UnityEvent onDeath;

    private float currentHealth;
    private bool isDead;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth = Mathf.Max(0f, currentHealth - amount);
        onDamaged?.Invoke();

        if (currentHealth <= 0f)
        {
            isDead = true;
            onDeath?.Invoke();
        }
    }
}
