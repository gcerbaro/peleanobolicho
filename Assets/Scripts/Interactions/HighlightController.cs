using UnityEngine;

public class HighlightController : MonoBehaviour
{
    private Material[] _originalMaterials;
    private Renderer _objectRenderer;
    private Material _highlightMaterial;
    private bool _isHighlighted = false;

    [Header("Configurações de Highlight")]
    [SerializeField] private Material defaultHighlightMaterial;

    private void Awake()
    {
        // Inicializa o Renderer
        _objectRenderer = GetComponent<Renderer>();
        if (_objectRenderer)
        {
            _originalMaterials = _objectRenderer.materials;
        }
        else
        {
            Debug.LogError("Renderer não encontrado no objeto!");
        }

        // Configura um material padrão de highlight, caso não tenha sido atribuído
        if (!defaultHighlightMaterial)
        {
            defaultHighlightMaterial = new Material(Shader.Find("Unlit/Color"))
            {
                color = Color.red
            };
        }

        _highlightMaterial = defaultHighlightMaterial;
    }

    public void EnableHighlight()
    {
        if (_isHighlighted || !_objectRenderer) return;

        // Cria um array de materiais com o highlight
        Material[] highlightMaterials = new Material[_originalMaterials.Length + 1];
        for (int i = 0; i < _originalMaterials.Length; i++)
        {
            highlightMaterials[i] = _originalMaterials[i];
        }
        highlightMaterials[_originalMaterials.Length] = _highlightMaterial;
        _objectRenderer.materials = highlightMaterials;

        _isHighlighted = true;
    }

    public void DisableHighlight()
    {
        if (!_isHighlighted || !_objectRenderer) return;

        // Restaura os materiais originais
        _objectRenderer.materials = _originalMaterials;
        _isHighlighted = false;
    }
}