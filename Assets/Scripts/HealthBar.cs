using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthSliderRed;
    public Slider healthSliderYellow;
    public float maxHealth = 100f;
    public float yellowBarSmoothTime = 0.5f;
    public float minSmoothTime = 0.2f;  
    public float maxSmoothTime = 1.5f; 

    private float _currentHealth;
    private float _yellowBarVelocity;

    void Start()
    {
        _currentHealth = maxHealth;
        healthSliderRed.maxValue = maxHealth;
        healthSliderYellow.maxValue = maxHealth;

        healthSliderRed.value = maxHealth;
        healthSliderYellow.value = maxHealth;
    }

    void Update()
    {
        // Atualize a barra vermelha instantaneamente com a vida atual do player
        healthSliderRed.value = _currentHealth;

        // Atualize a barra amarela gradualmente
        healthSliderYellow.value = Mathf.SmoothDamp(
            healthSliderYellow.value,
            healthSliderRed.value,
            ref _yellowBarVelocity,
            yellowBarSmoothTime
        );
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(10);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            TakeDamage(50);
        }
        
    }

    public void TakeDamage(float damage)
    {
        // Reduz a saúde atual, mas não abaixo de zero
        _currentHealth = Mathf.Max(_currentHealth - damage, 0);

        // Ajusta o tempo de suavização da barra amarela de acordo com o dano
        yellowBarSmoothTime = Mathf.Lerp(minSmoothTime, maxSmoothTime, 1 - (damage / maxHealth));
    }
}