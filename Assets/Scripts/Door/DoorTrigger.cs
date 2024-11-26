using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    [SerializeField] private Room parentRoom;

    private void Start()
    {
        parentRoom = GetComponentInParent<Room>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Verifica se é o jogador
        {
            parentRoom.CloseDoors();
        }
    }
}
