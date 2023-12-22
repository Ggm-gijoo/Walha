using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject endingPanel;

    private void Update()
    {
        if(!gameOverPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
            SetPause();
    }
    public void OnGameOver()
    {
        Time.timeScale = 0f;
        gameOverPanel.SetActive(true);
    }
    public void OnEnding()
    {
        Time.timeScale = 0f;
        Managers.Sound.Play("End", Define.Sound.Bgm);
        endingPanel.SetActive(true);
    }
    public void SetPause()
    {
        Time.timeScale = pausePanel.activeSelf? 1 : 0;
        pausePanel.SetActive(!pausePanel.activeSelf);
    }

    public void Restart()
    {
        Managers.Scene.LoadScene(Define.Scene.InGame);
        Time.timeScale = 1.0f;
    }
    public void Exit()
    {
        Managers.Scene.LoadScene(Define.Scene.Title);
    }
}
