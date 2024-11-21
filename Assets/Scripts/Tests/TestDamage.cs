using UnityEngine;

public class TestDamage : MonoBehaviour 
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Actions.onTakeDamage(15);
        }
    }
}