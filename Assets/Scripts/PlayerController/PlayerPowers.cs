using UnityEngine;

public class PlayerPowers : MonoBehaviour
{
    public bool hasInfiniteHealth = false;
    public bool hasSuperStrength = false;

    private HealthSystem _playerHealth;
    private PlayerAttack _playerAttack;

    void Start()
    {
        _playerAttack = GetComponent<PlayerAttack>();

        if (!_playerAttack) Debug.LogError("PlayerAttack não encontrados!");
    }

    void Update()
    {
        //Toggle para vida infinita
        if (Input.GetKeyDown(KeyCode.H)) ToggleInfiniteHealth();
        
        //Toggle para super forca
        if (Input.GetKeyDown(KeyCode.F)) ToggleSuperStrength();
    }

    public void ToggleInfiniteHealth()
    {
        hasInfiniteHealth = !hasInfiniteHealth;
        if (hasInfiniteHealth)
        {
            Actions.onInfiniteLife?.Invoke(1f);
            Debug.Log("Vida infinita ativada!");
        }
        else
        {
            Actions.onInfiniteLife?.Invoke(0f);
            Debug.Log("Vida infinita desativada!");
        }
    }

    public void ToggleSuperStrength()
    {
        hasSuperStrength = !hasSuperStrength;
        if (hasSuperStrength)
        {
            _playerAttack.SetSuperStrength(true);
            Debug.Log("Força muito alta ativada!");
        }
        else
        {
            _playerAttack.SetSuperStrength(false);
            Debug.Log("Força muito alta desativada!");
        }
    }
}