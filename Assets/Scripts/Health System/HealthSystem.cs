using System;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    private float _maxHealth = 100f;
    private float _currentHealth;

    private void OnEnable()
    {
        Actions.onTakeDamage += TakeDamage;
    }

    private void OnDisable()
    {
        Actions.onTakeDamage -= TakeDamage;
    }

    private void Start()
    {
        _currentHealth = _maxHealth;
    }

    private void TakeDamage(float damage)
    {
        _currentHealth = Mathf.Max(_currentHealth - damage, 0);
        Debug.Log("Cheguei no take damage");

        // Invoca o evento de dano para atualizar a UI
        Actions.onDamage?.Invoke(_currentHealth);
        
        if (_currentHealth <= 0)
            KillPlayer();
    }

    private void KillPlayer()
    {
        _currentHealth = 0;
        print("dead");
    }
}