using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public float MaxHealth { get; private set; } = 100f;
    public float CurrentHealth { get; private set; }

    private void OnEnable()
    {
        Actions.onTakeDamage += TakeDamage;
        Actions.onHealLife += HealLife;
    }

    private void OnDisable()
    {
        Actions.onTakeDamage -= TakeDamage;
        Actions.onHealLife -= HealLife;
    }

    private void Start()
    {
        CurrentHealth = MaxHealth;
    }

    private void TakeDamage(float damage)
    {
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);
        Debug.Log("Cheguei no take damage");

        // Invoca o evento de dano para atualizar a UI
        Actions.onDamage?.Invoke(CurrentHealth);
        
        if (CurrentHealth <= 0)
            KillPlayer();
    }
    
    private void HealLife(float healAmount)
    {
        if (CurrentHealth >= MaxHealth)
            return;

        // Limita a vida ao valor máximo permitido
        CurrentHealth = Mathf.Min(CurrentHealth + healAmount, MaxHealth);

        Debug.Log("Cheguei no heal life");

        // Invoca o evento para atualizar a UI
        Actions.onHeal?.Invoke(CurrentHealth);
    }


    private void KillPlayer()
    {
        CurrentHealth = 0;
        print("dead");
    }
}