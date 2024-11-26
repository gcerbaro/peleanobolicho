using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        ServiceLocator.Reset();
        Time.timeScale = 1; // Garante que o jogo não está pausado.
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Update is called once per frame
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
         // For editor testing, use this line to stop play mode:
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
