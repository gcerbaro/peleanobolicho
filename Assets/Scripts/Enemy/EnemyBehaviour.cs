using System;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehavior : MonoBehaviour
{
    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int IsFighting = Animator.StringToHash("IsFighting");
    private static readonly int HitReaction = Animator.StringToHash("HitReaction");
    private static readonly int Death = Animator.StringToHash("Death");

    public Transform player;
    private NavMeshAgent _agent;
    private Animator _animator;
    private EnemyHealth _enemyHealth;

    [Header("Configurações de Detecção e Ataque")]
    [SerializeField] private float attackBoxHeightOffset = 1f; // Ajuste manual da altura
    [SerializeField] private float detectionRadius = 10f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private Vector3 attackBoxSize = new Vector3(2f, 2f, 2f);
    [SerializeField] private float attackBoxOffset = 1f;

    public bool _isAttacking = false;
    private float _nextAttackTime = 0f;
    private bool _isDead = false;

    public event Action<bool, bool> OnCombatStateChanged;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _enemyHealth = GetComponent<EnemyHealth>();

        if (_enemyHealth)
        {
            _enemyHealth.ApplyDamagetoEnemy += ReactToDamage;
            _enemyHealth.OnEnemyDeath += HandleDeath;
        }
    }

    void Update()
    {
        if (_isDead || !player || !_agent.isOnNavMesh) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Atualiza o parâmetro de velocidade para a Blend Tree
        _animator.SetFloat(Speed, _agent.velocity.magnitude);

        if (distanceToPlayer <= detectionRadius)
        {
            if (!IsPlayerInAttackRange())
            {
                // Persegue o jogador
                _animator.SetBool(IsFighting, false);
                SetCombatState(false, false); // Fora de combate
                _agent.SetDestination(player.position);
            }
            else
            {
                // Entrou em alcance de ataque
                _animator.SetFloat(Speed, 0f);
                _animator.SetBool(IsFighting, true);
                SetCombatState(true, false); // Está lutando

                _agent.ResetPath(); // Para o movimento

                if (!_isAttacking && Time.time >= _nextAttackTime)
                {
                    _isAttacking = true;
                    SetCombatState(false, true); // Está atacando
                    _animator.SetTrigger(Attack);
                    _nextAttackTime = Time.time + attackCooldown; // Define o cooldown
                }
            }
        }
        else
        {
            // Sai do alcance de detecção
            _animator.SetBool(IsFighting, false);
            SetCombatState(false, false); // Saiu do combate
            _agent.ResetPath();
        }
    }

    private void ReactToDamage(float damage)
    {
        if (_isDead) return;

        if (_animator)
        {
            _animator.SetTrigger(HitReaction);
        }

        if (_agent)
        {
            _agent.isStopped = true;
            Invoke(nameof(ResumeMovement), 1f);
        }
    }

    private void HandleDeath()
    {
        _isDead = true;

        if (_animator)
        {
            Debug.Log("Death triggered");
            _animator.SetTrigger(Death); // Ativa a animação de morte

            // Habilita Root Motion para permitir o movimento controlado pela animação
            _animator.applyRootMotion = true;
        }

        if (_agent)
        {
            _agent.isStopped = true;
            _agent.enabled = false;
        }
    }

    private void ResumeMovement()
    {
        if (_isDead) return;

        if (_agent)
        {
            _agent.isStopped = false;
        }
    }

    public void ApplyDamage() // Chamado via evento de animação
    {
        if (IsPlayerInAttackRange())
        {
            Actions.onTakeDamage(10); // Aplica dano ao jogador
        }

        _isAttacking = false; // Libera o ataque
        SetCombatState(true, false); // Continua lutando, mas não atacando
    }

    private bool IsPlayerInAttackRange()
    {
        Vector3 boxCenter = transform.position + transform.forward * attackBoxOffset + Vector3.up * attackBoxHeightOffset;
        Collider[] hitObjects = Physics.OverlapBox(boxCenter, attackBoxSize / 2, transform.rotation);

        foreach (Collider obj in hitObjects)
        {
            if (obj.CompareTag("Player")) return true;
        }

        return false;
    }

    private void SetCombatState(bool isFighting, bool isAttacking)
    {
        if (_animator.GetBool(IsFighting) != isFighting)
        {
            _animator.SetBool(IsFighting, isFighting);
        }

        OnCombatStateChanged?.Invoke(isFighting, isAttacking);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 0f, 1f, 0.5f);
        Vector3 boxCenter = transform.position + transform.forward * attackBoxOffset + Vector3.up * attackBoxHeightOffset;
        Gizmos.DrawCube(boxCenter, attackBoxSize);
    }
}
