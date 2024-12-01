using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [SerializeField] GameObject difficultyPanel; // Painel de dificuldade
    [SerializeField] GameObject menu;            // Painel do menu inicial
    [SerializeField] GameObject about;
    [SerializeField] GameObject main;
    
    private DifficultyManager difficultyManager;
    [SerializeField] private AudioClip clickSound;

    private bool isVisible;
    
    private void Awake()
    {
        difficultyManager = FindObjectOfType<DifficultyManager>();
    }

    void Start()
    {
        // Inicia com o menu ativo e o painel de dificuldade desativado
        menu.SetActive(true);
        difficultyPanel.SetActive(false);
        about.SetActive(false);
    }

    private void PlayClickSound()
    {
        SoundFXManager.instance.PlaySoundEffect(clickSound,transform,1f);
    }

    public void LoadGame()
    {
        PlayClickSound();
        SceneManager.LoadScene("TestSCENE");
    }

    public void LoadRandomDungeon(){
        PlayClickSound();
        SceneManager.LoadScene("RandomDungeon");
    }

    // Método chamado ao clicar no botão "Difficulty"
    public void ShowDifficultyPanel()
    {
        PlayClickSound();
        menu.SetActive(false); // Desativa o menu inicial
        difficultyPanel.SetActive(true); // Ativa o painel de dificuldade
    }

    // Método chamado ao selecionar uma dificuldade ou voltar ao menu
    public void BackToMenu()
    {
        PlayClickSound();
        difficultyPanel.SetActive(false); // Desativa o painel de dificuldade
        menu.SetActive(true); // Ativa o menu inicial
    }

    // Métodos para selecionar dificuldades (você pode adicionar lógica específica)
    public void SelectEasy()
    {
        PlayClickSound();
        DifficultyManager.SetDifficulty(DifficultyManager.Difficulty.Easy);
        Debug.Log("Dificuldade Easy selecionada.");
        BackToMenu();
    }
    public void SelectMedium()
    {
        PlayClickSound();
        DifficultyManager.SetDifficulty(DifficultyManager.Difficulty.Normal);
        Debug.Log("Dificuldade Medium selecionada.");
        BackToMenu(); // Retorna ao menu após selecionar
    }
    
    public void SelectHard()
    {
        PlayClickSound();
        DifficultyManager.SetDifficulty(DifficultyManager.Difficulty.Hard);
        Debug.Log("Dificuldade Hard selecionada.");
        BackToMenu(); // Retorna ao menu após selecionar
    }

    public void ShowAbout()
    {
        PlayClickSound();

        isVisible = !isVisible;
        about.SetActive(isVisible);
        main.SetActive(!isVisible);
    }
}
