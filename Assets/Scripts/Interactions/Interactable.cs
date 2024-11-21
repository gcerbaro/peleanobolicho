using System;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public void Awake()
    {
        gameObject.layer = 6;
    }

    public abstract void OnInteract();
    public abstract void OnFocus();
    public abstract void OnLoseFocus();
}
