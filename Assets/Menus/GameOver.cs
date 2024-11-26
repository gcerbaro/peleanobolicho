using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        ServiceLocator.Reset();
    }

    // Update is called once per frame
    public void RestartGame()
    {
        SceneManager.LoadScene("TestSCENE");
    }

    public void ReturnToMenu(){
        SceneManager.LoadScene("Menu");
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
