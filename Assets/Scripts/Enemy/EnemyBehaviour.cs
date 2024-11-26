using System;
using System.Collections.Generic;
using System.Linq;
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
    private Collider _collider;
    

    [Header("Configurações de Detecção e Ataque")]
    [SerializeField] private float attackBoxHeightOffset = 1f; // Ajuste manual da altura
    [SerializeField] private float detectionRadius = 10f;
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private Vector3 attackBoxSize = new Vector3(2f, 2f, 2f);
    [SerializeField] private float attackBoxOffset = 1f;
    
    [Header("Audios")]
    [SerializeField] private AudioClip airSwooshClip;
    [SerializeField] private AudioClip[] attackSoundClips;
    [SerializeField] private AudioClip[] damageSoundClips;
    [SerializeField] private AudioClip[] deathSoundClips;
    [SerializeField] private AudioClip[] knifeHitSoundClips;
    [SerializeField] private AudioClip knifeExtraDeathSound;
    [SerializeField] private AudioClip woodFootstepSound;
    [SerializeField] private AudioClip stoneFootstepSound;
    
    
    [Header("Outras configuracoes")]
    [SerializeField] private Room roomControl;
    [SerializeField] private GameObject knifeInPlayer;

    [Header("Sangue na facada")]
    public ParticleSystem bloodEffect;
    
    private float _baseDamage = 10f;
    private bool isAttacking;
    private float _nextAttackTime;
    private bool _isDead;
    private float _footstepTimer = 0f;
    
    //Audio dos passos
    private float footstepVolume = 0.25f;
    private float footstepInterval = 0.74f;

    public event Action<bool, bool> OnCombatStateChanged;

    void Start()
    {
        _collider = GetComponent<Collider>();
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _enemyHealth = GetComponent<EnemyHealth>();

        // Encontra a sala no pai
        roomControl = GetComponentInParent<Room>();

        if (!roomControl) Debug.LogError("RoomControl não encontrado para o inimigo: " + gameObject.name);

        if (_enemyHealth)
        {
            _enemyHealth.ApplyDamagetoEnemy += ReactToDamage;
            _enemyHealth.OnEnemyDeath += HandleDeath;
        }

        _baseDamage *= DifficultyManager.EnemyDamageMultiplier;
    }


    void Update()
    {
        if (_isDead || !player || !_agent.isOnNavMesh) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Atualiza o parâmetro de velocidade para a Blend Tree
        _animator.SetFloat(Speed, _agent.velocity.magnitude);

        // Executa metodo dos passos
        HandleFootsteps();
        
        if (distanceToPlayer <= detectionRadius && HasLineOfSight(player))
        {
            // Persegue ou ataca o jogador com base na distância
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

                if (!isAttacking && Time.time >= _nextAttackTime)
                {
                    isAttacking = true; // Ativa "estado" de ataque
                    SetCombatState(false, true); // Está atacando
                    _animator.SetTrigger(Attack); // Ativa animação de ataque
                    _nextAttackTime = Time.time + attackCooldown; // Define o cooldown
                }
            }
        }
        else
        {
            // Sai do alcance de detecção ou está obstruído
            _animator.SetBool(IsFighting, false);
            SetCombatState(false, false); // Saiu do combate
            _agent.ResetPath();
        }
    }

    public void AssignRoom(Room room)
    {
        roomControl = room;
    }

    private void HandleFootsteps()
    {
        // Se o inimigo não estiver se movendo, não faça nada
        if (_agent.velocity.magnitude <= 0f) return;

        // Reduz o tempo do timer
        _footstepTimer -= Time.deltaTime;

        // Quando o timer chega a zero, reproduz o som do passo
        if (_footstepTimer <= 0)
        {
            // Raycast para detectar o tipo de superfície
            if (Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, out RaycastHit hit, 3f))
            {
                switch (hit.collider.tag)
                {
                    case "Footsteps/Wood":
                        SoundFXManager.instance.PlaySoundEffect(woodFootstepSound, transform, footstepVolume);
                        break;

                    case "Footsteps/Stone":
                        SoundFXManager.instance.PlaySoundEffect(stoneFootstepSound, transform, footstepVolume);
                        break;
                }
            }
            
            _footstepTimer = footstepInterval;
        }
    }


    private void ReactToDamage(float damage)
    {
        if (_isDead) return;

        if (_animator)
        {
            _animator.SetTrigger(HitReaction);
            
            // Se a faca estiver ativa
            if (knifeInPlayer.activeSelf) 
            {
                //Audio so para hits com a faca ativa
                SoundFXManager.instance.PlayRandomSoundEffects(knifeHitSoundClips, transform, 0.7f);
                Instantiate(bloodEffect, transform.position + Vector3.up * 1f, Quaternion.identity);
            }
            
            SoundFXManager.instance.PlayRandomSoundEffects(damageSoundClips,transform, 0.7f); // Audio de hit
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
        roomControl.OnEnemyDefeated();

        if (_animator)
        {
            Debug.Log("Death triggered");
            _animator.SetTrigger(Death); // Ativa a animação de morte
            
            // Habilita Root Motion para permitir o movimento controlado pela animação
            _animator.applyRootMotion = true;
            
            // Cria novo array para adicionar novo som apenas aqui
            AudioClip[] currentSounds = deathSoundClips;

            //Adiciona som apenas se faca estiver ativa
            if (knifeInPlayer.activeSelf)
            {
                currentSounds.Append(knifeExtraDeathSound);
            }
            
            //Reproz sons de morte
            SoundFXManager.instance.PlayRandomSoundEffects(currentSounds,transform,1f);
        }

        if (_agent)
        {
            _agent.isStopped = true;
            _agent.enabled = false;
        }

        if (_collider)
        {
            _collider.enabled = false;
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

    // Chamado via evento de animação
    public void ApplyDamage() 
    {
        SoundFXManager.instance.PlaySoundEffect(airSwooshClip,transform,0.3f);
        if (IsPlayerInAttackRange())
        {
            Actions.onTakeDamage(_baseDamage); // Aplica dano ao jogador
                
            // Audio de ataque de soco 
            SoundFXManager.instance.PlayRandomSoundEffects(attackSoundClips,transform, 1f); 
            
            // Audio de dano no player
            SoundFXManager.instance.PlayRandomSoundEffects(damageSoundClips,player.transform, 1f); 
        }

        isAttacking = false; // Libera o ataque
        SetCombatState(true, false); // Continua lutando, mas não atacando
    }

    private bool IsPlayerInAttackRange()
    {
        Vector3 boxCenter = transform.position + transform.forward * attackBoxOffset + Vector3.up * attackBoxHeightOffset;
        Collider[] hitObjects = Physics.OverlapBox(boxCenter, attackBoxSize / 2, transform.rotation);

        foreach (Collider obj in hitObjects)
        {
            if (obj.CompareTag("Player") && HasLineOfSight(obj.transform))
            {
                return true;
            }
        }

        return false;
    }

    
    private bool HasLineOfSight(Transform target)
    {
        // Define a origem do raycast (posição do inimigo)
        Vector3 origin = transform.position + Vector3.up * 1.5f; // Ajuste a altura para a cabeça do inimigo

        // Calcula a direção do raycast
        Vector3 direction = (target.position - origin).normalized;

        // Comprimento do raycast
        float distance = Vector3.Distance(origin, target.position);

        // Raycast para verificar obstruções
        if (Physics.Raycast(origin, direction, out RaycastHit hit, distance))
        {
            // Verifica se o objeto atingido é o jogador
            if (hit.collider.CompareTag("Player"))
            {
                return true; // Linha de visão clara
            }
        }

        return false; // Algo está bloqueando a visão
    }


    private void SetCombatState(bool isFighting, bool attacking)
    {
        if (_animator.GetBool(IsFighting) != isFighting)
        {
            _animator.SetBool(IsFighting, isFighting);
        }

        OnCombatStateChanged?.Invoke(isFighting, attacking);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 0f, 1f, 0.5f);
        Vector3 boxCenter = transform.position + transform.forward * attackBoxOffset + Vector3.up * attackBoxHeightOffset;
        Gizmos.DrawCube(boxCenter, attackBoxSize);
    }
}
