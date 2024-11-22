using System;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private static readonly int Attack1 = Animator.StringToHash("Attack");

    [Header("Configurações de Ataque")]
    [SerializeField] private float attackDamage = 5f;
    [SerializeField] private KeyCode AttackKey = KeyCode.Mouse0;
    [SerializeField] private Vector3 attackBoxSize = new Vector3(1.5f, 1.5f, 2f); // Tamanho da área de ataque
    [SerializeField] private float attackRange = 1.5f; // Distância à frente do jogador
    private LayerMask enemyLayer; // Camada para identificar inimigos

    private Animator _animator;

    private void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        enemyLayer = LayerMask.GetMask("Enemy");
    }

    private void Update()
    {
        if (Input.GetKeyDown(AttackKey))
        {
            Attack();
        }
    }

    private void Attack()
    {
        // Envia o Trigger para o Animator para iniciar a animação de ataque
        if (_animator)
        {
            _animator.SetTrigger(Attack1);
        }

        // Detecta inimigos na área apenas no momento do ataque
        PerformAttack();
    }

    private void PerformAttack()
    {
        // Calcula o centro da caixa de ataque
        Vector3 attackBoxCenter = transform.position + transform.forward * attackRange;

        // Detecta inimigos na frente do jogador, dentro da área de ataque
        Collider[] hitEnemies = Physics.OverlapBox(
            attackBoxCenter,
            attackBoxSize / 2,
            transform.rotation,
            enemyLayer);

        if (hitEnemies.Length > 0)
        {
            Debug.Log($"Detectados {hitEnemies.Length} inimigos na área de ataque!");
        }

        foreach (Collider enemy in hitEnemies)
        {
            // Verifica explicitamente se o inimigo está na área de ataque
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            if (enemyHealth)
            {
                enemyHealth.TakeDamage(attackDamage);
                Debug.Log($"{enemy.name} tomou {attackDamage} de dano.");
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Desenha a área do ataque no editor
        Gizmos.color = Color.red;
        Vector3 boxCenter = transform.position + transform.forward * attackRange;
        Gizmos.matrix = Matrix4x4.TRS(boxCenter, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, attackBoxSize);
    }
}
