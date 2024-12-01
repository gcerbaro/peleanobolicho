﻿using System;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private static readonly int Attack1 = Animator.StringToHash("Attack");

    [Header("Configurações de Ataque")]
    [SerializeField] private float attackDamage = 5f;
    [SerializeField] private KeyCode AttackKey = KeyCode.Mouse0;
    [SerializeField] private Vector3 attackBoxSize = new Vector3(1.5f, 1.5f, 2f); 
    [SerializeField] private float attackRange = 1.5f; 
    private float _damageMultiplier = 1f;

    [Header("Configuracoes de audio")] 
    [SerializeField] private AudioClip punchAirSoundFx;
    [SerializeField] private AudioClip[] punchHitSoundFxs;
    
    private LayerMask enemyLayer; 
    private Animator _animator;

    private void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        enemyLayer = LayerMask.GetMask("Enemy");
    }

    private void Update()
    {
        if (Input.GetKeyDown(AttackKey)) Attack();
    }

    private void PlayAirAttack()
    {
        SoundFXManager.instance.PlaySoundEffect(punchAirSoundFx, transform,0.4f);
    }

    private void Attack()
    {
        if (_animator) _animator.SetTrigger(Attack1);
        
        PerformAttack();
    }

    private void PerformAttack()
    {
        float finalDamage = attackDamage * _damageMultiplier;
        
        Vector3 attackBoxCenter = transform.position + transform.forward * attackRange;
        
        Collider[] hitEnemies = Physics.OverlapBox(
            attackBoxCenter,
            attackBoxSize / 2,
            transform.rotation,
            enemyLayer);

        if (hitEnemies.Length > 0)
        {
            Debug.Log($"Detectados {hitEnemies.Length} inimigos na área de ataque!");
        }

        foreach (Collider enemy in hitEnemies)
        {
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            if (enemyHealth)
            {
                SoundFXManager.instance.PlayRandomSoundEffects(punchHitSoundFxs, transform, 1f);
                enemyHealth.TakeDamage(finalDamage);
                Debug.Log($"{enemy.name} tomou {finalDamage} de dano.");
            }
        }
    }
    
    public void SetSuperStrength(bool isActive)
    {
        _damageMultiplier = isActive ? 15f : 1f;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 boxCenter = transform.position + transform.forward * attackRange;
        Gizmos.matrix = Matrix4x4.TRS(boxCenter, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, attackBoxSize);
    }
}
