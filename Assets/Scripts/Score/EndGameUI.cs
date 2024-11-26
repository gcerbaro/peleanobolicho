using TMPro;
using UnityEngine;

public class EndGameUI : MonoBehaviour
{
    public TextMeshProUGUI scoreText;

    private void Start()
    {
        int finalScore = ScoreManager.Instance.GetScore();
        scoreText.text = "Enemies killed: " + finalScore.ToString();
    }
}