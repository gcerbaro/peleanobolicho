using UnityEngine;

public class ItemInteraction : Interactable
{
    private GameObject _lowPolyArms;
    private GameObject _knifeInPlayer;

    [Header("Highlight Controller")]
    [SerializeField] private HighlightController highlightController; // Controller de Highlight.

    [Header("Audio clips")]
    [SerializeField] private AudioClip knifeEquipSfx;

    public new void Awake()
    {
        // Inicializa objetos de referência
        _lowPolyArms = GameObject.Find("LowPolyArms");
        _knifeInPlayer = GameObject.Find("KnifeInPlayer");

        if (!_lowPolyArms || !_knifeInPlayer)
        {
            Debug.LogError("LowPolyArms ou KnifeInPlayer não encontrados na cena!");
        }

        _knifeInPlayer.SetActive(false);
        _lowPolyArms.SetActive(true);

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
        highlightController.enabled = true;
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
        if (!_knifeInPlayer.activeSelf && _lowPolyArms.activeSelf)
        {
            SoundFXManager.instance.PlaySoundEffect(knifeEquipSfx, transform, 0.5f);
            _lowPolyArms.SetActive(false);
            _knifeInPlayer.SetActive(true);
        }

        gameObject.SetActive(false);
    }
}