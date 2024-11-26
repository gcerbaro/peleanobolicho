using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Quando for usar algo para interagir tem que colocar a layer 6 (interactable)
public class HealPlayerInteractable : Interactable
{
    [SerializeField] private float healAmount = 15f; // Valor de cura ao interagir.
    [SerializeField] private AudioClip bonusSound;
    [SerializeField] private HighlightController highlightController;

    private new void Awake()
    {
        // Verifica se o HighlightController está atribuído. Se não, tenta buscar no objeto.
        if (!highlightController)
        {
            highlightController = GetComponent<HighlightController>();
            if (!highlightController)
            {
                Debug.LogError("HighlightController não encontrado neste objeto!");
            }
        }
    }

    public override void OnInteract()
    {
        Actions.onHealLife?.Invoke(healAmount); // Aciona o evento para curar a vida.
        SoundFXManager.instance.PlaySoundEffect(bonusSound, transform, 0.5f);

        gameObject.SetActive(false); // Desativa o objeto após a interação.
    }

    public override void OnFocus()
    {
        Debug.Log("olhando pro salame");
        highlightController?.EnableHighlight(); // Ativa o highlight.
    }

    public override void OnLoseFocus()
    {
        Debug.Log("parei de olhar pro salame");
        highlightController?.DisableHighlight(); // Desativa o highlight.
    }
}