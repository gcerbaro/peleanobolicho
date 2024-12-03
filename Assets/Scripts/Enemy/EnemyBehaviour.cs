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
    [SerializeField] private float attackBoxHeightOffset = 1f; 
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

        
        _animator.SetFloat(Speed, _agent.velocity.magnitude);

    
        HandleFootsteps();
        
        if (distanceToPlayer <= detectionRadius && HasLineOfSight(player))
        {
            
            if (!IsPlayerInAttackRange())
            {
                
                _animator.SetBool(IsFighting, false);
                SetCombatState(false, false); 
                _agent.SetDestination(player.position);
            }
            else
            {
                
                _animator.SetFloat(Speed, 0f);
                _animator.SetBool(IsFighting, true);
                SetCombatState(true, false); 

                _agent.ResetPath(); 

                if (!isAttacking && Time.time >= _nextAttackTime)
                {
                    isAttacking = true; 
                    SetCombatState(false, true); 
                    _animator.SetTrigger(Attack); 
                    _nextAttackTime = Time.time + attackCooldown; 
                }
            }
        }
        else
        {
            
            _animator.SetBool(IsFighting, false);
            SetCombatState(false, false); 
            _agent.ResetPath();
        }
    }

    public void AssignRoom(Room room)
    {
        roomControl = room;
    }

    private void HandleFootsteps()
    {
   
        if (_agent.velocity.magnitude <= 0f) return;

  
        _footstepTimer -= Time.deltaTime;

     
        if (_footstepTimer <= 0)
        {
            
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
            
            
            if (knifeInPlayer.activeSelf) 
            {
                
                SoundFXManager.instance.PlayRandomSoundEffects(knifeHitSoundClips, transform, 0.7f);
                Instantiate(bloodEffect, transform.position + Vector3.up * 1f, Quaternion.identity);
            }
            
            SoundFXManager.instance.PlayRandomSoundEffects(damageSoundClips,transform, 0.7f); 
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
        ScoreManager.Instance.AddScore(1);
        Debug.Log(ScoreManager.Instance.GetScore());

        if (_animator)
        {
            Debug.Log("Death triggered");
            _animator.SetTrigger(Death); 
            
            _animator.applyRootMotion = true;
            
            AudioClip[] currentSounds = deathSoundClips;

            if (knifeInPlayer.activeSelf)
            {
                currentSounds.Append(knifeExtraDeathSound);
            }
            
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

    public void ApplyDamage() 
    {
        SoundFXManager.instance.PlaySoundEffect(airSwooshClip,transform,0.3f);
        if (IsPlayerInAttackRange())
        {
            Actions.onTakeDamage(_baseDamage); 
                
            SoundFXManager.instance.PlayRandomSoundEffects(attackSoundClips,transform, 1f); 
            

            SoundFXManager.instance.PlayRandomSoundEffects(damageSoundClips,player.transform, 1f); 
        }

        isAttacking = false; 
        SetCombatState(true, false); 
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
        
        Vector3 origin = transform.position + Vector3.up * 1.5f; 

        
        Vector3 direction = (target.position - origin).normalized;

        
        float distance = Vector3.Distance(origin, target.position);

        
        if (Physics.Raycast(origin, direction, out RaycastHit hit, distance))
        {
           
            if (hit.collider.CompareTag("Player"))
            {
                return true; 
            }
        }

        return false; 
    }


    private void SetCombatState(bool isFighting, bool attacking)
    {
        if (_animator.GetBool(IsFighting) != isFighting)
        {
            _animator.SetBool(IsFighting, isFighting);
        }

        OnCombatStateChanged?.Invoke(isFighting, attacking);
    }

   
}
