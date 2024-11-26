using System.Collections;
using UnityEngine;

public class KnifeAttack : MonoBehaviour
{
    private static readonly int KnifeHit = Animator.StringToHash("Hit");

    [Header("Configurações de Ataque com Faca")]
    [SerializeField] private float knifeDamage = 15f; 
    [SerializeField] private float knifeAttackRange = 2f;  
    [SerializeField] private float knifeDurability = 7f; // Durabilidade da faca
    [SerializeField] private Vector3 knifeAttackBoxSize = new Vector3(2f, 2f, 3f); //Define tamanho da caixa de ataque
    [SerializeField] private Vector3 attackBoxOffset = new Vector3(0f, 0f, 1f); //Offset em relacao ao player para caixa de ataque
    [SerializeField] private KeyCode knifeAttackKey = KeyCode.Mouse0; // Botao de ataque 
    [SerializeField] private float attackCooldown = 0.5f; // Tempo entre ataques
    [SerializeField] private float damageDelay = 0.2f; // Atraso antes do dano
    
    [SerializeField] private GameObject lowPolyArms; 
    [SerializeField] private GameObject knifeInPlayer; 
    [SerializeField] private Animator animator;
    
    private LayerMask _enemyLayer; 
    private float _lastAttackTime = 0f; // Controle de cooldown
    private PlayerAttack _playerAttack;

    private void Start()
    {
        _playerAttack = GetComponent<PlayerAttack>();
        _enemyLayer = LayerMask.GetMask("Enemy"); 
    }

    private void Update()
    {
        if (CanUseKnife())
        {
            _playerAttack.enabled = false;
            if (Input.GetKeyDown(knifeAttackKey) && Time.time >= _lastAttackTime + attackCooldown)
            {
                _lastAttackTime = Time.time; // Atualiza o tempo do último ataque
                Attack();
            }
        }
    }
    
    private bool CanUseKnife()
    {
        if (!lowPolyArms || !knifeInPlayer)
        {
            Debug.Log("LPA ou KnifeInPlayer nao encontrado");
        }
        // Checa se LPA está inativo e KnifeInPlayer está ativo
        
        return !lowPolyArms.activeSelf && knifeInPlayer.activeSelf;
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

        bool hitOneEnemy = false;
        
        // Aplica dano aos inimigos detectados
        if (hitEnemies.Length > 0)
        {
            foreach (Collider enemy in hitEnemies)
            {
                EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
                if (enemyHealth)
                {
                    enemyHealth.TakeDamage(knifeDamage);
                    hitOneEnemy = true;
                    Debug.Log($"{enemy.name} tomou {knifeDamage} de dano da faca.");
                }
            }
        }
        
        // Reduz a durabilidade apenas se ao menos um inimigo foi atingido
        if (hitOneEnemy)
        {
            knifeDurability--;
            Debug.Log($"Durabilidade da faca: {knifeDurability}");

            if (knifeDurability <= 0)
            {
                BreakKnife();
            }
        }
    }
    
    private void BreakKnife()
    {
        Debug.Log("A faca quebrou!");

        // Desativa a faca e ativa as mãos
        knifeInPlayer.SetActive(false);
        lowPolyArms.SetActive(true);
        _playerAttack.enabled = true;
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
