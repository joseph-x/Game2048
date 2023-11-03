using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    static string savedHighScoreKey = "HighScore";

    public TileBoard board;
    public CanvasGroup gameOverCanvas;

    public Text txtScore;
    public Text txtBest;

    private int score = 0;

    void Awake()
    {
        Screen.SetResolution(600, 1000, false);
    }

    void Start()
    {
        NewGame();
    }

    public void NewGame()
    {
        SetScore(0);

        txtBest.text = LoadHighScore().ToString();

        gameOverCanvas.alpha = 0;
        gameOverCanvas.interactable = false;

        board.ClearBoard();
        board.CreateTile();
        board.CreateTile();
        board.enabled = true;
    }

    public void GameOver()
    {
        board.enabled = false;
        gameOverCanvas.interactable = true;
        PlayerPrefs.Save();

        StartCoroutine(Fade(gameOverCanvas, 1f, 1f));
    }

    public void IncreaseScore(int points)
    {
        SetScore(score + points);
    }

    private void SetScore(int score)
    {
        this.score = score;
        txtScore.text = score.ToString();

        // 保存分数
        SaveHighSore(score);
    }

    private void SaveHighSore(int score)
    {
        int highScore = LoadHighScore();

        if (score > highScore)
        {
            PlayerPrefs.SetInt(savedHighScoreKey, score);
        }
    }

    private int LoadHighScore()
    {
        return PlayerPrefs.GetInt(savedHighScoreKey, 0);
    }

    private IEnumerator Fade(CanvasGroup canvasGroup, float to, float delay = 0f)
    {
        yield return new WaitForSeconds(delay);

        float elapsed = 0f;
        float duration = 0.5f;
        float from = canvasGroup.alpha;

        while (elapsed < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;

            yield return null;
        }

        canvasGroup.alpha = to;
    }
}
