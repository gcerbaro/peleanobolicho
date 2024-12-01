using UnityEngine;

public class HighlightController : MonoBehaviour
{
    private Material[] _originalMaterials;
    private Renderer _objectRenderer;
    private bool _isHighlighted = false;
    
    [Header("Configurações de Highlight")]
    [SerializeField] private Material highlightMaterial;

    private void Awake()
    {
        _objectRenderer = GetComponent<Renderer>();
        if (!_objectRenderer) Debug.LogError("Renderer não encontrado no objeto!");
        
        _originalMaterials = _objectRenderer.materials;
        
        if (!highlightMaterial)
        {
            highlightMaterial = new Material(Shader.Find("Unlit/Color"))
            {
                color = Color.red
            };
        }
    }

    public void EnableHighlight()
    {
        if (_isHighlighted) return;
        
        Material[] highlightMaterials = new Material[_originalMaterials.Length + 1];
        
        for (int i = 0; i < _originalMaterials.Length; i++)
        {
            highlightMaterials[i] = _originalMaterials[i];
        }
        
        highlightMaterials[^1] = highlightMaterial;
        _objectRenderer.materials = highlightMaterials;

        _isHighlighted = true;
    }

    public void DisableHighlight()
    {
        if (!_isHighlighted) return;
        
        _objectRenderer.materials = _originalMaterials;
        
        _isHighlighted = false;
    }
}