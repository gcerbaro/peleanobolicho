using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    
    private int _finalScore;
    
    void Awake()
    {
        _finalScore = ScoreManager.Instance.GetScore();
        scoreText.text = "Enemies killed: " + _finalScore.ToString();
        ServiceLocator.Reset();
        Time.timeScale = 1; // Garante que o jogo não está pausado.
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    public void RestartGame()
    {
        SceneManager.LoadScene("TestSCENE");
        ScoreManager.Instance.ResetScore();
        
    }

    public void ReturnToMenu(){
        SceneManager.LoadScene("Menu");
        ScoreManager.Instance.ResetScore();
    }

    public void QuitGame(){
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
