using System;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public List<EnemyBehavior> enemies; // Lista de inimigos na sala
    public DoorScript.Door[] connectedDoors; // Portas conectadas à sala
    private int enemiesRemaining = 0;
    
    [SerializeField] private GameObject[] doorTriggers;

    private void Start()
    {
        foreach (EnemyBehavior enemy in enemies)
        {
            enemiesRemaining++;
            enemy.AssignRoom(this); // Atribui a sala a cada inimigo
        }
    }

    public void OnEnemyDefeated()
    {
        Debug.Log("Enemy defeated in room: " + gameObject.name);
        enemiesRemaining--;
        if (enemiesRemaining <= 0)
        {
            OpenDoors();
        }
    }

    private void OpenDoors()
    {
        foreach (DoorScript.Door door in connectedDoors)
        {
            door.Open();
            
        }
        
        foreach (GameObject trig in doorTriggers)
        {
            trig.SetActive(false);
        }
    }
    
    public void CloseDoors()
    {
        foreach (DoorScript.Door door in connectedDoors)
        {
            door.Close();
            
        }
    }
    

    
}
