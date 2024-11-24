using System.Collections;
using UnityEngine;

public class KnifeAttack : MonoBehaviour
{
    private static readonly int KnifeHit = Animator.StringToHash("Hit");

    [Header("Configurações de Ataque com Faca")]
    [SerializeField] private float knifeDamage = 15f; 
    [SerializeField] private float knifeAttackRange = 2f; 
    [SerializeField] private Vector3 knifeAttackBoxSize = new Vector3(2f, 2f, 3f); 
    [SerializeField] private Vector3 attackBoxOffset = new Vector3(0f, 0f, 1f);
    [SerializeField] private KeyCode knifeAttackKey = KeyCode.Mouse0; 
    [SerializeField] private float attackCooldown = 0.5f; // Tempo entre ataques
    [SerializeField] private float damageDelay = 0.2f; // Atraso antes do dano
    [SerializeField] private GameObject lowPolyArms; 
    [SerializeField] private GameObject knifeInPlayer; 
    [SerializeField] private Animator animator;
    
    private LayerMask _enemyLayer; 
    private float _lastAttackTime = 0f; // Controle de cooldown

    private void Start()
    {
        _enemyLayer = LayerMask.GetMask("Enemy"); 
    }

    private void Update()
    {
        if (CanUseKnife())
        {
            if (Input.GetKeyDown(knifeAttackKey) && Time.time >= _lastAttackTime + attackCooldown)
            {
                _lastAttackTime = Time.time; // Atualiza o tempo do último ataque
                Attack();
            }
        }
    }

    private void Attack()
    {
        // Ativa a animação de ataque da faca
        if (animator)
        {
            animator.SetTrigger(KnifeHit);
        }

        // Inicia a corrotina para aplicar o dano após o atraso
        StartCoroutine(PerformKnifeAttackWithDelay());
    }

    private IEnumerator PerformKnifeAttackWithDelay()
    {
        // Aguarda o tempo configurado para o atraso
        yield return new WaitForSeconds(damageDelay);

        Debug.Log("Detectando inimigos...");

        // Calcula o centro da caixa de ataque da faca com offset
        Vector3 attackBoxCenter = transform.position + transform.forward * knifeAttackRange +
                                  transform.TransformDirection(attackBoxOffset);

        // Detecta inimigos na frente do jogador, dentro da área de ataque
        Collider[] hitEnemies = Physics.OverlapBox(
            attackBoxCenter,
            knifeAttackBoxSize / 2,
            transform.rotation,
            _enemyLayer);

        // Aplica dano aos inimigos detectados
        if (hitEnemies.Length > 0)
        {
            foreach (Collider enemy in hitEnemies)
            {
                EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
                if (enemyHealth)
                {
                    enemyHealth.TakeDamage(knifeDamage);
                    Debug.Log($"{enemy.name} tomou {knifeDamage} de dano da faca.");
                }
            }
        }
        else
        {
            Debug.Log("Nenhum inimigo detectado, mas ataque foi realizado.");
        }
    }
    
    private bool CanUseKnife()
    {
        // Checa se LPA está inativo e KnifeInPlayer está ativo
        return !lowPolyArms.activeSelf && knifeInPlayer.activeSelf;
    }

    private void OnDrawGizmosSelected()
    {
        // Desenha a área de ataque da faca no editor para ajuste
        Gizmos.color = Color.blue;
        Vector3 boxCenter = transform.position + transform.forward * knifeAttackRange + transform.TransformDirection(attackBoxOffset);
        Gizmos.matrix = Matrix4x4.TRS(boxCenter, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, knifeAttackBoxSize);
    }
}
