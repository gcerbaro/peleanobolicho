using System.Collections;
using UnityEngine;

public class StaminaSystem : MonoBehaviour
{
    public float MaxStamina { get; private set; } = 100f;
    public float CurrentStamina { get; private set; }

    [SerializeField] private float staminaUseMultiplier = 50f;
    [SerializeField] private float timeBeforeStaminaRegen = 0.7f; 
    [SerializeField] private float staminaValueIncrement = 5f;  

    private Coroutine _regeneratingStamina;

    private void Start()
    {
        CurrentStamina = MaxStamina;
    }

    public void UseStamina()
    {
        if (CurrentStamina > 0)
        {
            CurrentStamina -= staminaUseMultiplier * Time.deltaTime;

            if (CurrentStamina < 0) CurrentStamina = 0;

            //Atualiza UI
            Actions.onStaminaChange?.Invoke(CurrentStamina);
            
            if (_regeneratingStamina != null)
            {
                StopCoroutine(_regeneratingStamina);
                _regeneratingStamina = null;
            }
        }
    }

    public void StartRegen()
    {
        if (_regeneratingStamina == null && CurrentStamina < MaxStamina)
        {
            _regeneratingStamina = StartCoroutine(RegenerateStamina());
        }
    }

    private IEnumerator RegenerateStamina()
    {
        yield return new WaitForSeconds(timeBeforeStaminaRegen);

        while (CurrentStamina < MaxStamina)
        {
            CurrentStamina = Mathf.MoveTowards(CurrentStamina, MaxStamina, staminaValueIncrement * Time.deltaTime);

            // Atualiza a UI
            Actions.onStaminaChange?.Invoke(CurrentStamina);

            yield return null; 
        }
        
        CurrentStamina = MaxStamina;
        
        Actions.onStaminaChange?.Invoke(CurrentStamina);
        
        _regeneratingStamina = null;
    }
}
