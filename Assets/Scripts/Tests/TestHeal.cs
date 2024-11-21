using UnityEngine;

public class TestHeal : MonoBehaviour 
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Actions.onHealLife(15);
        }
    }
}