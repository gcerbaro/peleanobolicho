using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public event Action<float> ApplyDamagetoEnemy; 
    public event Action OnEnemyDeath;             

    [Header("Enemy Health Settings")]
    [SerializeField] private float MaxHealth = 50f;

    private float CurrentHealth { get; set; }

    private void Start()
    {
        MaxHealth *= DifficultyManager.EnemyHealthMultiplier;
        CurrentHealth = MaxHealth;

        ApplyDamagetoEnemy += LogDamageLocally;
        OnEnemyDeath += LogDeathLocally;
    }


    public void TakeDamage(float damage)
    {
        if (CurrentHealth <= 0) return; 

        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);
        Debug.Log($"{gameObject.name} took {damage} damage. Current health: {CurrentHealth}");
        
  
        ApplyDamagetoEnemy?.Invoke(damage);

        
        if (CurrentHealth <= 0)
            Die();
    }


    private void Die()
    {
        Debug.Log($"{gameObject.name} died!");


        OnEnemyDeath?.Invoke();
    }


    private void LogDamageLocally(float damage)
    {
        Debug.Log($"{gameObject.name}: Local event triggered for damage {damage}");
    }

    private void LogDeathLocally()
    {
        Debug.Log($"{gameObject.name}: Local event triggered for death");
    }
}