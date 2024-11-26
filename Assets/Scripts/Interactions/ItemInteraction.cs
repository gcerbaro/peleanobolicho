using UnityEngine;

public class ItemInteraction : Interactable
{
    [SerializeField] private GameObject lowPolyArms;
    [SerializeField]private GameObject knifeInPlayer;

    [Header("Highlight Controller")]
    [SerializeField] private HighlightController highlightController; // Controller de Highlight.

    [Header("Audio clips")]
    [SerializeField] private AudioClip knifeEquipSfx;

    public new void Awake()
    {

        if (!lowPolyArms || !knifeInPlayer)
        {
            Debug.LogError("LowPolyArms ou KnifeInPlayer não encontrados na cena!");
        }

        knifeInPlayer.SetActive(false);
        lowPolyArms.SetActive(true);

        // Configura a camada como interagível
        gameObject.layer = 6;

        // Inicializa o HighlightController
        if (!highlightController)
        {
            highlightController = GetComponent<HighlightController>();
            if (!highlightController)
            {
                Debug.LogWarning("HighlightController não encontrado neste objeto.");
            }
        }
    }

    // Chamado ao ganhar o foco
    public override void OnFocus()
    {
        Debug.Log("Foco no item.");
        highlightController?.EnableHighlight();
    }

    // Chamado ao perder o foco
    public override void OnLoseFocus()
    {
        Debug.Log("Item perdeu o foco.");
        highlightController?.DisableHighlight();
    }

    // Chamado ao interagir
    public override void OnInteract()
    {
        Debug.Log("Interagindo com o item.");
        if (!knifeInPlayer.activeSelf && lowPolyArms.activeSelf)
        {
            SoundFXManager.instance.PlaySoundEffect(knifeEquipSfx, transform, 0.5f);
            lowPolyArms.SetActive(false);
            knifeInPlayer.SetActive(true);
        }

        gameObject.SetActive(false);
    }
}