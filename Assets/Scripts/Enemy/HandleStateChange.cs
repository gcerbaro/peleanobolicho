using System;
using UnityEngine;
using UnityEngine.AI;

public class HandleStateChange : MonoBehaviour
{
    private NavMeshAgent _agent;
    private EnemyBehavior _enemy;
    void Start()
    {
        _enemy = GetComponent<EnemyBehavior>();
        _agent = GetComponent<NavMeshAgent>();

        if (_enemy)
        {
            _enemy.OnCombatStateChanged += HandleCombatStateChange;
        }
    }

    private void HandleCombatStateChange(bool isFighting, bool isAttacking)
    {
        if (isFighting || isAttacking)
        {
            _agent.isStopped = true; // Para completamente o agente
            _agent.ResetPath();     // Remove qualquer destino ativo
        }
        else
        {
            _agent.isStopped = false; // Permite movimento
        }
    }

}
