using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Room : MonoBehaviour
{
    public List<EnemyBehavior> enemies; // Lista de inimigos na sala
    public DoorScript.Door[] connectedDoors; // Portas conectadas à sala
    private int enemiesRemaining = 0;

    [SerializeField] private GameObject[] doorTriggers;
    [SerializeField] private bool isFinalRoom = false; // Marca se esta sala é a última do jogo

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

            if (isFinalRoom)
            {
                EndGame();
            }
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

    private void EndGame()
    {
        Debug.Log("All enemies defeated! Loading EndOfGame scene...");
        SceneManager.LoadScene("Endofgame"); // Substitua "EndOfGame" pelo nome da sua cena
    }
}