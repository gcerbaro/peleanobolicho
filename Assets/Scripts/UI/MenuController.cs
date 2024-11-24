using UnityEngine;

public class MenuController : MonoBehaviour
{
    public void SetEasyDifficulty()
    {
        DifficultyManager.SetDifficulty(DifficultyManager.Difficulty.Easy);
    }

    public void SetNormalDifficulty()
    {
        DifficultyManager.SetDifficulty(DifficultyManager.Difficulty.Normal);
    }

    public void SetHardDifficulty()
    {
        DifficultyManager.SetDifficulty(DifficultyManager.Difficulty.Hard);
    }

    public void StartGame()
    {
        // Inicia o jogo
        Debug.Log($"Dificuldade Selecionada: {DifficultyManager.CurrentDifficulty}");
        // Carregue a próxima cena ou inicie o jogo
    }
}