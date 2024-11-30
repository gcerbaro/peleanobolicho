using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    private int score = 0;

    private void Awake()
    {
        // Garante que o ScoreManager é único na cena
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persiste entre cenas
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Método para adicionar pontuação
    public void AddScore(int value)
    {
        score += value;
    }

    // Método para obter a pontuação atual
    public int GetScore()
    {
        return score;
    }

    // Método para resetar a pontuação (opcional)
    public void ResetScore()
    {
        score = 0;
    }
}