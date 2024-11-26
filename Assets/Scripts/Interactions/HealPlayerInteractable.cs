using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Quando for usar algo para interagir tem que colocar a layer 6 (interactable)

public class HealPlayerInteractable : Interactable
{
    [SerializeField] private float healAmount = 15f; // Valor de cura ao interagir.
    [SerializeField] private AudioClip bonusSound;
    
    public override void OnInteract()
    {
        print("Interacting with" + gameObject.name);
        
        Actions.onHealLife?.Invoke(healAmount);
        SoundFXManager.instance.PlaySoundEffect(bonusSound, transform, 0.5f);
        
        gameObject.SetActive(false);
    }

    public override void OnFocus()
    {
        print("Looking at" + gameObject.name);
    }

    public override void OnLoseFocus()
    {
        print("STOPPED Looking at" + gameObject.name);
    }
}
