using System;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private static readonly int Attack1 = Animator.StringToHash("Attack");
    public float attackDamage = 25f;
    public float attackRange = 1.5f;
    private LayerMask enemyLayer;
    [SerializeField] private KeyCode AttackKey = KeyCode.Mouse0;
    private Animator _animator;

    private void Start()
    {
        enemyLayer = LayerMask.GetMask("Enemy");
        _animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(AttackKey))
        {
            Debug.Log("Mouse Button Detected");
            Attack();
        }

    }

    private void Attack()
    {
        // Envia o Trigger para o Animator para iniciar a animação de ataque
        if (_animator)
        {
            _animator.SetTrigger(Attack1); 
            Debug.Log("Attack Triggered");
        }

        // Detecta inimigos no alcance
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, attackRange, enemyLayer);

        foreach (Collider enemy in hitEnemies)
        {
            // Acessa o sistema de vida do inimigo e causa dano
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            if (enemyHealth)
            {
                enemyHealth.TakeDamage(attackDamage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Desenha o alcance do ataque no editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}