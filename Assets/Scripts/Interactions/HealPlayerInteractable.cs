using UnityEngine;

// Quando for usar algo para interagir tem que colocar a layer 6 (interactable)
public class HealPlayerInteractable : Interactable
{
    [SerializeField] private float healAmount = 15f; 
    [SerializeField] private AudioClip bonusSound;
    [SerializeField] private HighlightController highlightController;
    [SerializeField] private HealthSystem healthSystem;

    private new void Awake()
    {
        gameObject.layer = 6;
        
        highlightController = GetComponent<HighlightController>();
        
        if (!highlightController) Debug.Log("highlightController nao encontrado");
    }

    public override void OnInteract()
    {
        if (healthSystem.CurrentHealth >= healthSystem.MaxHealth) return;
        
        Actions.onHealLife?.Invoke(healAmount); 
        
        SoundFXManager.instance.PlaySoundEffect(bonusSound, transform, 0.5f);

        gameObject.SetActive(false); 
    }

    public override void OnFocus()
    {
        highlightController?.EnableHighlight(); 
    }

    public override void OnLoseFocus()
    {
        highlightController?.DisableHighlight(); 
    }
}