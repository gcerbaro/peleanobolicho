using UnityEngine;
using UnityEngine.SceneManagement;

public class HealthSystem : MonoBehaviour
{
    public float MaxHealth { get; private set; } = 100f;
    public float CurrentHealth { get; private set; }
    
    private float isInfiniteHealth = 0f;

    private void OnEnable()
    {
        Actions.onTakeDamage += TakeDamage;
        Actions.onHealLife += HealLife;
        Actions.onInfiniteLife += SetInfiniteHealth;
    }

    private void OnDisable()
    {
        Actions.onTakeDamage -= TakeDamage;
        Actions.onHealLife -= HealLife;
        Actions.onInfiniteLife -= SetInfiniteHealth;
    }

    private void Start()
    {
        CurrentHealth = MaxHealth;
    }

    private void TakeDamage(float damage)
    {
        if (isInfiniteHealth == 1) return;
        
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);
        Debug.Log("Cheguei no take damage");
        
        // Invoca o evento de dano para atualizar a UI
        Actions.onDamage?.Invoke(CurrentHealth);
        
        if (CurrentHealth <= 0) KillPlayer();
    }
    
    private void HealLife(float healAmount)
    {
        if (isInfiniteHealth == 1) return;
        
        if (CurrentHealth >= MaxHealth) return;
        
        CurrentHealth = Mathf.Min(CurrentHealth + healAmount, MaxHealth);
    
        // Invoca o evento para atualizar a UI
        Actions.onHeal?.Invoke(CurrentHealth);
    }
    
    public void SetInfiniteHealth(float f)
    {
        isInfiniteHealth = f;

        if (isInfiniteHealth == 1) CurrentHealth = MaxHealth; 
    }

    private void KillPlayer()
    {
        CurrentHealth = 0;
        SceneManager.LoadScene("Endofgame");
    }
}