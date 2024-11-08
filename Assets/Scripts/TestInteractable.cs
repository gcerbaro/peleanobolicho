using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Quando for usar algo para interagir tem que colocar a layer 6 (interactable)

public class TestInteractable : Interactable
{
    public override void OnInteract()
    {
        print("Interacting with" + gameObject.name);
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
