using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private AudioClip clickSound;
    
    private int _finalScore;
    
    void Awake()
    {
        _finalScore = ScoreManager.Instance.GetScore();
        scoreText.text = "Enemies killed: " + _finalScore.ToString();
        Time.timeScale = 1; // Garante que o jogo não está pausado.
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    private void PlayClickSound()
    {
        SoundFXManager.instance.PlaySoundEffect(clickSound,transform,1f);
    }
    
    public void RestartGame()
    {
        PlayClickSound();
        SceneManager.LoadScene("TestSCENE");
        ScoreManager.Instance.ResetScore();
        
    }

    public void ReturnToMenu()
    {
        PlayClickSound();
        SceneManager.LoadScene("Menu");
        ScoreManager.Instance.ResetScore();
    }

    public void QuitGame()
    {
        PlayClickSound();
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
