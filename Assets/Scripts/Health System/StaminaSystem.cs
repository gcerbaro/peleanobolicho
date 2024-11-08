using System.Collections;
using UnityEngine;

public class StaminaSystem : MonoBehaviour
{
    public float MaxStamina { get; private set; } = 100f;
    public float CurrentStamina { get; private set; }

    [SerializeField] private float staminaUseMultiplier = 5f;
    [SerializeField] private float timeBeforeStaminaRegen = 5f;
    [SerializeField] private float staminaValueIncrement = 2f;  // Incremento de cada vez
    [SerializeField] private float staminaTimeIncrement = 0.1f; // Intervalo de tempo entre os incrementos
    [SerializeField] private float smoothFactor = 0.1f;  // Fator de suavização

    private Coroutine _regeneratingStamina;

    private void Start()
    {
        CurrentStamina = MaxStamina;
    }

    public void UseStamina(float deltaTime)
    {
        if (_regeneratingStamina != null)
        {
            StopCoroutine(_regeneratingStamina);
            _regeneratingStamina = null;
        }

        CurrentStamina -= staminaUseMultiplier * deltaTime;

        if (CurrentStamina < 0)
            CurrentStamina = 0;

        Actions.onStaminaChange?.Invoke(CurrentStamina);
    }

    public void StartRegen()
    {
        if (_regeneratingStamina == null && CurrentStamina < MaxStamina)
            _regeneratingStamina = StartCoroutine(RegenerateStamina());
    }

    private IEnumerator RegenerateStamina()
    {
        yield return new WaitForSeconds(timeBeforeStaminaRegen);

        while (CurrentStamina < MaxStamina)
        {
            // Incremento suave: aumentamos o valor com um fator de suavização
            float targetStamina = CurrentStamina + staminaValueIncrement;
            CurrentStamina = Mathf.Lerp(CurrentStamina, targetStamina, smoothFactor);

            Actions.onStaminaChange?.Invoke(CurrentStamina);

            yield return new WaitForSeconds(staminaTimeIncrement);  // Espera entre os incrementos
        }

        // Garantir que o valor final seja exatamente o máximo
        CurrentStamina = MaxStamina;
        Actions.onStaminaChange?.Invoke(CurrentStamina);

        _regeneratingStamina = null;
    }
}
