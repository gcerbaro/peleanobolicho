using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyBehavior : MonoBehaviour
{
    private static readonly int Attack = Animator.StringToHash("Attack");
    public Transform player;
    private NavMeshAgent _agent;
    private Animator _animator;
    private float _animDuration = 0.5f;

    [Header("Configurações de Detecção e Ataque")]
    [SerializeField] private float detectionRadius = 10f; // Distância de detecção
    [SerializeField] private float attackCooldown = 1f; // Tempo entre ataques
    [SerializeField] private Vector3 attackBoxSize = new Vector3(2f, 2f, 2f); // Tamanho da área de ataque
    [SerializeField] private float attackBoxOffset = 1f; // Distância à frente do inimigo

    private bool _isAttacking = false;
    private float _nextAttackTime = 0f;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();

        // Configuração inicial do agente de navegação
        if (_agent)
        {
            _agent.stoppingDistance = 0; // Desabilita a parada padrão
        }
    }

    void Update()
    {
        if (player && _agent.isOnNavMesh)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            // Verifica se o jogador está dentro do raio de detecção
            if (distanceToPlayer <= detectionRadius)
            {
                // O inimigo persegue o jogador até ele estar na área de ataque
                if (!IsPlayerInAttackRange())
                {
                    _agent.SetDestination(player.position);
                }
                else
                {
                    _agent.ResetPath(); // Para de se mover
                    if (!_isAttacking && Time.time >= _nextAttackTime)
                    {
                        StartCoroutine(AttackPlayer());
                    }
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

        // Inicia a animação de ataque
        // if (_animator)
        //     _animator.SetTrigger(Attack);

        yield return new WaitForSeconds(_animDuration); // Ajuste o tempo conforme a duração da animação

        // Aplica dano ao jogador somente se ele estiver na área de ataque
        if (IsPlayerInAttackRange())
        {
            Actions.onTakeDamage(10); // Ação para causar dano ao jogador
            Debug.Log($"{gameObject.name} atacou o jogador!");
        }

        _nextAttackTime = Time.time + attackCooldown;
        yield return new WaitForSeconds(attackCooldown);
        _isAttacking = false;
    }

    private bool IsPlayerInAttackRange()
    {
        // Centro da caixa de ataque
        Vector3 boxCenter = transform.position + transform.forward * attackBoxOffset;

        // Detecta todos os objetos na área da caixa
        Collider[] hitObjects = Physics.OverlapBox(boxCenter, attackBoxSize / 2, transform.rotation);

        // Filtra os objetos para encontrar o jogador pela Tag
        foreach (Collider obj in hitObjects)
        {
            if (obj.CompareTag("Player")) // Verifica se o objeto possui a tag "Player"
            {
                return true;
            }
        }

        return false; // Nenhum jogador detectado
    }

    private void OnDrawGizmosSelected()
    {
        // Desenha a área de ataque no editor
        Gizmos.color = Color.blue;
        Vector3 boxCenter = transform.position + transform.forward * attackBoxOffset;
        Gizmos.matrix = Matrix4x4.TRS(boxCenter, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, attackBoxSize);

        // Desenha o raio de detecção do inimigo
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
