using UnityEngine;
using UnityEngine.Rendering.UI;
using UnityEngine.Serialization;

public class ItemInteraction: Interactable
{
    private Material[] _originalMaterials;
    private Renderer _objectRenderer;
    private bool _isHighlighted = false;

    private GameObject _lowPolyArms;
    private GameObject _knifeInPlayer;
    
    [SerializeField] private Material highlightMaterial;
    
    [Header("Audio clips")]
    [SerializeField] private AudioClip knifeEquipSfx;

    public new void Awake()
    {
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

        // Inicializa o Renderer e os materiais
        _objectRenderer = GetComponent<Renderer>();
        if (_objectRenderer)
        {
            _originalMaterials = _objectRenderer.materials; // Salva os materiais originais
        }

        // Cria um material de highlight automaticamente, caso não tenha um pronto
        if (!highlightMaterial)
        {
            highlightMaterial = new Material(Shader.Find("Custom/OutlineShader"))
            {
                color = Color.yellow // Configura a cor do contorno
            };
        }
    }

    // Ativar o highlight
    private void EnableHighlight()
    {
        if (_isHighlighted || !_objectRenderer) return;

        // Cria um novo array de materiais com o highlight
        Material[] highlightMaterials = new Material[_originalMaterials.Length + 1];
        for (int i = 0; i < _originalMaterials.Length; i++)
        {
            highlightMaterials[i] = _originalMaterials[i];
        }
        highlightMaterials[_originalMaterials.Length] = highlightMaterial;
        _objectRenderer.materials = highlightMaterials;

        _isHighlighted = true;
    }

    // Desativar o highlight
    private void DisableHighlight()
    {
        if (!_isHighlighted || !_objectRenderer) return;

        _objectRenderer.materials = _originalMaterials; // Restaura os materiais originais
        _isHighlighted = false;
    }

    // Chamado ao ganhar o foco
    public override void OnFocus()
    {
        EnableHighlight();
    }

    // Chamado ao perder o foco
    public override void OnLoseFocus()
    {
        DisableHighlight();
    }

    // Chamado ao interagir
    public override void OnInteract()
    {
        if (!_knifeInPlayer.activeSelf && _lowPolyArms.activeSelf)
        {
            SoundFXManager.instance.PlaySoundEffect(knifeEquipSfx, transform, 0.5f);
            _lowPolyArms.SetActive(false);
            _knifeInPlayer.SetActive(true);
        }
        
        
        
        gameObject.SetActive(false);
    }
    
}
