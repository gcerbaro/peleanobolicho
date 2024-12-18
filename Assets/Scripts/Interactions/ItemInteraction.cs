﻿using UnityEngine;

public class ItemInteraction : Interactable
{
    [SerializeField] private GameObject lowPolyArms;
    [SerializeField] private GameObject knifeInPlayer;

    [Header("Highlight Controller")]
    [SerializeField] private HighlightController highlightController; 

    [Header("Audio clips")]
    [SerializeField] private AudioClip knifeEquipSfx;
    
    [Header("Dependencias")]
    [SerializeField] private KnifeAttack knifeAttack;

    public new void Awake()
    {
        if (!knifeAttack)
            Debug.Log("Knife attack nao encontrado em interagivel");
        
        gameObject.layer = 6;
        
        if (!lowPolyArms || !knifeInPlayer) Debug.LogError("LowPolyArms ou KnifeInPlayer não encontrados na cena!");
        
        knifeInPlayer.SetActive(false);
        lowPolyArms.SetActive(true);
        
        highlightController = GetComponent<HighlightController>();
        
        if (!highlightController) Debug.LogWarning("HighlightController não encontrado neste objeto.");
    }
    
    public override void OnFocus()
    {
        highlightController?.EnableHighlight();
    }
    
    public override void OnLoseFocus()
    {
        highlightController?.DisableHighlight();
    }
    
    public override void OnInteract()
    {
        SoundFXManager.instance.PlaySoundEffect(knifeEquipSfx, transform, 0.5f);

        knifeAttack.knifeDurability = knifeAttack.startKnifeDur;
        
        if (!knifeInPlayer.activeSelf && lowPolyArms.activeSelf)
        {
            lowPolyArms.SetActive(false);
            knifeInPlayer.SetActive(true);
        }

        gameObject.SetActive(false);
    }
}