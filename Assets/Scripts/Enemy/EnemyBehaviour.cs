using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyBehavior : MonoBehaviour
{
    private static readonly int Attack = Animator.StringToHash("Attack");
    public Transform player;
    private NavMeshAgent _agent;
    private SphereCollider _detectionSphere;
    private Animator _animator;

    [SerializeField] private float detectionRadius = 10f; // Distância de detecção
    [SerializeField] private float attackRange = 2f; // Distância de ataque
    [SerializeField] private float attackCooldown = 2f; // Tempo entre ataques

    private float _stoppingDistance = 2f;
    private bool _isAttacking = false;
    private float _nextAttackTime = 0f;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _detectionSphere = GetComponent<SphereCollider>();
        _animator = GetComponent<Animator>();

        // Define a distância de parada como metade do raio da esfera
        if (_detectionSphere)
        {
            _agent.stoppingDistance = _stoppingDistance; 
        }
    }

    void Update()
    {
        if (player && _agent.isOnNavMesh)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer <= detectionRadius)
            {
                // O inimigo percebe o jogador e começa a persegui-lo
                _agent.SetDestination(player.position);

                // Quando o jogador está dentro da distância de parada, o inimigo vai parar e tentar atacar
                if (distanceToPlayer <= _stoppingDistance && !_isAttacking && Time.time >= _nextAttackTime)
                {
                    StartCoroutine(AttackPlayer());
                }
            }
            else
            {
                // Quando o jogador sai da área de detecção, o inimigo para de seguir
                _agent.ResetPath();
            }
        }
    }

    private IEnumerator AttackPlayer()
    {
        _isAttacking = true;

        // Executa a animação de ataque
        //_animator.SetTrigger(Attack);

        // Espera o tempo necessário para que a animação de ataque termine
        yield return new WaitForSeconds(0.5f); // Tempo de animação (ajuste conforme necessário)

        // Lógica de dano ao jogador
        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            // Exemplo de aplicação de dano ao jogador
            Actions.onTakeDamage(10);
            Debug.Log("Inimigo atacou o jogador!");
        }

        // Define o tempo do próximo ataque
        _nextAttackTime = Time.time + attackCooldown;

        // Após o ataque, o inimigo deve parar de atacar por um tempo
        yield return new WaitForSeconds(attackCooldown);

        _isAttacking = false;
    }
}
