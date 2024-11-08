using Unity.VisualScripting;
using UnityEngine;

public class testHealth : MonoBehaviour 
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Actions.onTakeDamage(15);
        }
    }
}