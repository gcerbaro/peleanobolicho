using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI : MonoBehaviour
{
    private Slider _healthSliderRed;
    private Slider _healthSliderYellow;
    private Slider _staminaSlider;
    
    private readonly float _maxHealth = 100f;
    private readonly float _maxStamina = 100f;
    private float _yellowBarSmoothTime = 0.5f;
    private float _yellowBarVelocity;

    private void OnEnable()
    {
        Actions.onDamage += UpdateHealth;
        Actions.onHeal += UpdateHealth;
        Actions.onStaminaChange += UpdateStamina;
    }

    private void OnDisable()
    {
        Actions.onDamage -= UpdateHealth;
        Actions.onHeal -= UpdateHealth;
        Actions.onStaminaChange -= UpdateStamina;
    }

    private void Start()
    {
        _healthSliderRed = transform.Find("Red Health Bar").GetComponentInChildren<Slider>();
        _healthSliderYellow = transform.Find("Yellow Health Bar").GetComponentInChildren<Slider>();
        _staminaSlider = transform.Find("Stamina Bar").GetComponentInChildren<Slider>();

        UpdateStamina(_maxStamina);
        UpdateHealth(_maxHealth);
    }

    private void UpdateHealth(float currentHealth)
    {
        _healthSliderRed.value = currentHealth;

        // Inicia a Coroutine para atualizar a barra amarela
        StopAllCoroutines(); 
        StartCoroutine(UpdateYellowBar(currentHealth));
    }
    
    private void UpdateStamina(float currentStamina)
    {
        _staminaSlider.value = currentStamina;
    }

    private IEnumerator UpdateYellowBar(float targetHealth)
    {
        while (_healthSliderYellow.value > targetHealth)
        {
            _healthSliderYellow.value = Mathf.SmoothDamp(
                _healthSliderYellow.value, 
                targetHealth, 
                ref _yellowBarVelocity, 
                _yellowBarSmoothTime
            );

            yield return null;
        }
    }
}