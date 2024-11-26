using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject difficultyPanel; // Painel de dificuldade
    public GameObject menu;            // Painel do menu inicial
    private DifficultyManager difficultyManager;
    
    private void Awake()
    {
        difficultyManager = FindObjectOfType<DifficultyManager>();
        ServiceLocator.Reset(); // Reseta qualquer serviço do seu jogo
    }

    void Start()
    {
        // Inicia com o menu ativo e o painel de dificuldade desativado
        menu.SetActive(true);
        difficultyPanel.SetActive(false);
    }

    public void LoadGame(){
        SceneManager.LoadScene("TestSCENE");
    }

    public void LoadRandomDungeon(){
        SceneManager.LoadScene("RandomDungeon");
    }

    // Método chamado ao clicar no botão "Difficulty"
    public void ShowDifficultyPanel()
    {
        menu.SetActive(false); // Desativa o menu inicial
        difficultyPanel.SetActive(true); // Ativa o painel de dificuldade
    }

    // Método chamado ao selecionar uma dificuldade ou voltar ao menu
    public void BackToMenu()
    {
        difficultyPanel.SetActive(false); // Desativa o painel de dificuldade
        menu.SetActive(true); // Ativa o menu inicial
    }

    // Métodos para selecionar dificuldades (você pode adicionar lógica específica)
    
    public void SelectEasy(){
        DifficultyManager.SetDifficulty(DifficultyManager.Difficulty.Easy);
        Debug.Log("Dificuldade Easy selecionada.");
        BackToMenu();
    }
    public void SelectMedium()
    {
        DifficultyManager.SetDifficulty(DifficultyManager.Difficulty.Normal);
        Debug.Log("Dificuldade Medium selecionada.");
        BackToMenu(); // Retorna ao menu após selecionar
    }
    
    public void SelectHard()
    {
        DifficultyManager.SetDifficulty(DifficultyManager.Difficulty.Hard);
        Debug.Log("Dificuldade Hard selecionada.");
        BackToMenu(); // Retorna ao menu após selecionar
    }
}
