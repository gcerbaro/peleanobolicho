using System.Collections;
using UnityEngine;

public class KnifeAttack : MonoBehaviour
{
    private static readonly int KnifeHit = Animator.StringToHash("Hit");

    [Header("Configurações de Ataque com Faca")]
    [SerializeField] private float knifeDamage = 15f; 
    [SerializeField] private float knifeAttackRange = 2f;  
    [SerializeField] public float knifeDurability = 8f; // Durabilidade da faca
    [SerializeField] private float attackCooldown = 0.5f; // Tempo entre ataques
    [SerializeField] private float damageDelay = 0.2f; // Atraso antes do dano
    [SerializeField] private Vector3 knifeAttackBoxSize = new Vector3(2f, 2f, 3f); //Define tamanho da caixa de ataque
    [SerializeField] private KeyCode knifeAttackKey = KeyCode.Mouse0; // Botao de ataque 
    
    [Header("Configuracoes de audio")]
    [SerializeField] private AudioClip[] knifeHitsSfx;
    [SerializeField] private AudioClip breakToolSfx;
    
    [Header("Outras configuracoes")]
    [SerializeField] private GameObject lowPolyArms; 
    [SerializeField] private GameObject knifeInPlayer; 
    [SerializeField] private Animator animator;
    
    private bool _canAttack = false;
    private float knifeBreakDelay = 0.5f; 
    public float startKnifeDur;
    private float _lastAttackTime = 0f; 
    private LayerMask _enemyLayer; 
    private PlayerAttack _playerAttack;

    private void Start()
    {
        if (!animator) Debug.Log("Animator da faca nao encontrado");
        if (!knifeInPlayer) Debug.Log("knifeInPlayer nao encontrado");
        if (!lowPolyArms) Debug.Log("lowPolyArms nao encontrado");
        
        //Seta durabilidade da faca
        startKnifeDur = knifeDurability;
        
        _playerAttack = GetComponent<PlayerAttack>();
        _enemyLayer = LayerMask.GetMask("Enemy"); 
    }

    private void Update()
    {
        if (CanUseKnife() && _canAttack)
        {
            _playerAttack.enabled = false;
            if (Input.GetKeyDown(knifeAttackKey) && Time.time >= _lastAttackTime + attackCooldown && knifeDurability != 0)
            {
                _lastAttackTime = Time.time; 
                Attack();
            }
        }
    }
    
    private bool CanUseKnife()
    {
        if (!_canAttack) 
            _canAttack = true;
        
        return !lowPolyArms.activeSelf && knifeInPlayer.activeSelf;
    }

    private void Attack()
    {
        animator.SetTrigger(KnifeHit);
        
        StartCoroutine(PerformKnifeAttackWithDelay());
    }

    private IEnumerator PerformKnifeAttackWithDelay()
    {
        yield return new WaitForSeconds(damageDelay);
        
        Vector3 attackBoxCenter = transform.position + transform.forward * knifeAttackRange;
        
        Collider[] hitEnemies = Physics.OverlapBox(
            attackBoxCenter,
            knifeAttackBoxSize / 2,
            transform.rotation,
            _enemyLayer);

        bool hitOneEnemy = false;
        
        foreach (Collider enemy in hitEnemies)
        {
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            if (enemyHealth)
            {
                enemyHealth.TakeDamage(knifeDamage);
                hitOneEnemy = true;
            }
        }
        
        if (hitOneEnemy)
        {
            knifeDurability--;
            Debug.Log($"Durabilidade da faca: {knifeDurability}");

            if (knifeDurability <= 0) 
                StartCoroutine(BreakKnifeWithDelay());
        }
    }
    private IEnumerator BreakKnifeWithDelay()
    {
        _canAttack = false;
        yield return new WaitForSeconds(knifeBreakDelay);

        BreakKnife();
    }

    
    private void BreakKnife()
    {
        SoundFXManager.instance.PlaySoundEffect(breakToolSfx,transform, 0.5f);
        
        knifeInPlayer.SetActive(false);
        lowPolyArms.SetActive(true);
        
        _playerAttack.enabled = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Vector3 boxCenter = transform.position + transform.forward * knifeAttackRange;
        Gizmos.matrix = Matrix4x4.TRS(boxCenter, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, knifeAttackBoxSize);
    }
}
