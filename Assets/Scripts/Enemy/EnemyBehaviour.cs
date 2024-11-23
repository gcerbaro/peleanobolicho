using UnityEngine;
using UnityEngine.AI;
using System.Collections;

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
    [SerializeField] private float detectionRadius = 10f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private Vector3 attackBoxSize = new Vector3(2f, 2f, 2f);
    [SerializeField] private float attackBoxOffset = 1f;

    private bool _isAttacking = false;
    private float _nextAttackTime = 0f;

    // Para guardar a posição e rotação inicial do GameObject filho
    private Vector3 _childInitialPosition;
    private Quaternion _childInitialRotation;
    private Transform _childTransform;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponentInChildren<Animator>();
        _enemyHealth = GetComponent<EnemyHealth>();

        // Obtém o transform do GameObject filho (aquele que possui o Animator)
        _childTransform = _animator.transform;

        if (_enemyHealth)
        {
            // Associa os eventos de dano e morte
            _enemyHealth.ApplyDamagetoEnemy += ReactToDamage;
            _enemyHealth.OnEnemyDeath += HandleDeath;
        }
    }

    void Update()
    {
        if (player && _agent.isOnNavMesh)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            // Atualiza o parâmetro de velocidade para a Blend Tree
            _animator.SetFloat(Speed, _agent.velocity.magnitude);

            // Verifica se o jogador está dentro do raio de detecção
            if (distanceToPlayer <= detectionRadius)
            {
                if (!IsPlayerInAttackRange())
                {
                    // Persegue o jogador
                    _agent.SetDestination(player.position);
                    _animator.SetBool(IsFighting, false);
                }
                else
                {
                    // Prepara para atacar
                    _animator.SetFloat(Speed, 0f);
                    _animator.SetBool(IsFighting, true);

                    _agent.ResetPath(); // Para o movimento

                    if (!_isAttacking && Time.time >= _nextAttackTime)
                    {
                        StartCoroutine(AttackPlayer());
                    }
                }
            }
            else
            {
                // Se fora de alcance, parar movimento e animações de luta
                _animator.SetBool(IsFighting, false);
                _agent.ResetPath();
            }
        }
    }

    private void ReactToDamage(float damage)
    {
        if (_animator)
        {
            _animator.SetTrigger(HitReaction);
        }

        if (_agent)
        {
            _agent.isStopped = true;
            Invoke(nameof(ResumeMovement), 0.5f);
        }
    }

    private void HandleDeath()
    {
        if (_animator)
        {
            Debug.Log("Death triggered");
            _animator.SetTrigger(Death); // Ativa a animação de morte
        }

        if (_agent) _agent.isStopped = true;

        enabled = false; // Desativa o comportamento do inimigo
    }

    private void ResumeMovement()
    {
        if (_agent)
        {
            _agent.isStopped = false;
        }
    }

    private IEnumerator AttackPlayer()
    {
        _isAttacking = true;

        if (_animator)
        {
            // Salva a posição e rotação inicial do GameObject filho antes do ataque
            _childInitialPosition = _childTransform.position;
            _childInitialRotation = _childTransform.rotation;

            _animator.SetTrigger(Attack); // Executa a animação de ataque
        }

        // Aguarda a duração da animação (ajuste conforme necessário)
        yield return new WaitForSeconds(1f);

        // Após a animação, restaura a posição e rotação do GameObject filho
        RestoreChildPositionAndRotation();

        if (IsPlayerInAttackRange())
        {
            Actions.onTakeDamage(10); // Aplica dano ao jogador
        }

        _nextAttackTime = Time.time + attackCooldown;
        yield return new WaitForSeconds(attackCooldown);
        _isAttacking = false;
    }

    private bool IsPlayerInAttackRange()
    {
        Vector3 boxCenter = transform.position + transform.forward * attackBoxOffset;
        Collider[] hitObjects = Physics.OverlapBox(boxCenter, attackBoxSize / 2, transform.rotation);

        foreach (Collider obj in hitObjects)
        {
            if (obj.CompareTag("Player")) return true;
        }

        return false;
    }

    private void RestoreChildPositionAndRotation()
    {
        // Restaura a posição e rotação do GameObject filho
        _childTransform.position = _childInitialPosition;
        _childTransform.rotation = _childInitialRotation;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 0f, 1f, 0.5f);
        Vector3 boxCenter = transform.position + transform.forward * attackBoxOffset;
        Gizmos.DrawCube(boxCenter, attackBoxSize);
    }
}
