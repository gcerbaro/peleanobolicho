using UnityEngine;

public class EnemyEffects : MonoBehaviour
{
    private EnemyHealth _enemyHealthSystem;

    private void Awake()
    {
        _enemyHealthSystem = GetComponent<EnemyHealth>();
    }

    private void OnEnable()
    {
        // Inscreve-se nos eventos locais
        _enemyHealthSystem.ApplyDamagetoEnemy += ShowDamageEffect;
        _enemyHealthSystem.OnEnemyDeath += ShowDeathEffect;
    }

    private void OnDisable()
    {
        // Remove a inscrição nos eventos
        _enemyHealthSystem.ApplyDamagetoEnemy -= ShowDamageEffect;
        _enemyHealthSystem.OnEnemyDeath -= ShowDeathEffect;
    }

    private void ShowDamageEffect(float damage)
    {
        Debug.Log($"{gameObject.name} shows damage effect for {damage} damage.");
        // Adicione aqui efeitos visuais, como partículas ou flashes
    }

    private void ShowDeathEffect()
    {
        Debug.Log($"{gameObject.name} shows death effect.");
        // Adicione aqui efeitos visuais ou sons para a morte
    }
}