using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleSceneManager: MonoBehaviour
{
    [SerializeField] private GameObject creditPanel;
    private void Start()
    {
        Managers.Sound.Play("Title", Define.Sound.Bgm);
    }

    public void ToGameScene()
    {
        Managers.Scene.LoadScene(Define.Scene.InGame);
        Time.timeScale = 1.0f;
    }
    public void SetCreditPanel() => creditPanel.SetActive(!creditPanel.activeSelf);

    public void GameExit() => Application.Quit();
}
