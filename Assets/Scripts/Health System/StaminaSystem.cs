using System.Collections;
using UnityEngine;

public class StaminaSystem : MonoBehaviour
{
    public float MaxStamina { get; private set; } = 100f;
    public float CurrentStamina { get; private set; }

    [SerializeField] private float staminaUseMultiplier = 50f;
    [SerializeField] private float timeBeforeStaminaRegen = 0.7f; // Tempo de espera menor para iniciar regeneração
    [SerializeField] private float staminaValueIncrement = 5f;  // Incremento direto de stamina

    private Coroutine _regeneratingStamina;

    private void Start()
    {
        CurrentStamina = MaxStamina;
    }

    public void UseStamina(float deltaTime)
    {
        // Permitir uso da stamina se houver um valor positivo
        if (CurrentStamina > 0)
        {
            CurrentStamina -= staminaUseMultiplier * deltaTime;

            if (CurrentStamina < 0)
                CurrentStamina = 0;

            Actions.onStaminaChange?.Invoke(CurrentStamina);

            // Reinicia a regeneração se a stamina acabou
            if (CurrentStamina <= 0)
            {
                StartRegen();
            }
            else if (_regeneratingStamina != null)
            {
                // Parar regeneração se a stamina ainda está sendo usada
                StopCoroutine(_regeneratingStamina);
                _regeneratingStamina = null;
            }
        }
    }

    public void StartRegen()
    {
        // Iniciar regeneração apenas se não estiver regenerando e se a stamina não estiver no máximo
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
            // Incrementa de forma suave e contínua
            CurrentStamina = Mathf.MoveTowards(CurrentStamina, MaxStamina, staminaValueIncrement * Time.deltaTime);

            // Atualiza o UI ou outras partes do jogo
            Actions.onStaminaChange?.Invoke(CurrentStamina);

            yield return null; // Espera um frame antes do próximo incremento
        }

        // Quando a regeneração termina, garante que CurrentStamina esteja exatamente no valor máximo
        CurrentStamina = MaxStamina;
        Actions.onStaminaChange?.Invoke(CurrentStamina);
        _regeneratingStamina = null;
    }
}
