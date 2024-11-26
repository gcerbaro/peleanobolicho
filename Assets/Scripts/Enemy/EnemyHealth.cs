using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public event Action<float> ApplyDamagetoEnemy; // Acionado ao receber dano
    public event Action OnEnemyDeath;             // Acionado na morte

    [Header("Enemy Health Settings")]
    [SerializeField] private float MaxHealth = 50f;

    private float CurrentHealth { get; set; }

    private void Start()
    {
        MaxHealth *= DifficultyManager.EnemyHealthMultiplier;
        // Configura a vida inicial
        CurrentHealth = MaxHealth;

        // Associa os eventos localmente
        ApplyDamagetoEnemy += LogDamageLocally;
        OnEnemyDeath += LogDeathLocally;
    }

    // Método para receber dano
    public void TakeDamage(float damage)
    {
        if (CurrentHealth <= 0) return; // Evita dano em inimigos mortos

        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);
        Debug.Log($"{gameObject.name} took {damage} damage. Current health: {CurrentHealth}");
        
        // Dispara o evento de dano
        ApplyDamagetoEnemy?.Invoke(damage);

        // Checa se o inimigo morreu
        if (CurrentHealth <= 0)
            Die();
    }

    // Método chamado ao morrer
    private void Die()
    {
        Debug.Log($"{gameObject.name} died!");

        // Dispara o evento de morte
        OnEnemyDeath?.Invoke();
    }

    // Métodos locais para os eventos
    private void LogDamageLocally(float damage)
    {
        Debug.Log($"{gameObject.name}: Local event triggered for damage {damage}");
    }

    private void LogDeathLocally()
    {
        Debug.Log($"{gameObject.name}: Local event triggered for death");
    }
}