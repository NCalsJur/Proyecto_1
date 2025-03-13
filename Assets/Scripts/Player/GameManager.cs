using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager: MonoBehaviour
{
    public static GameManager instance;
    public GameObject lifesUI;
    public CharacterController player;
    public Text coinText;

    public GameObject panelPause;
    public GameObject panelGameOver;
    public GameObject panelLoad;
    public int coins;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void UpdateCoinCounter()
    {
        coins++;
        coinText.text = coins.ToString();
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        panelPause.SetActive(true);
    }

    public void UnPauseGame()
    {
        Time.timeScale = 1;
        panelPause.SetActive(false);
    }

    public void BackToMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("SelectLevel");
    }

    public void LoadSelected()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Level_1");
    }

    public void LoadScene(string sceneToLoad)
    {
        SceneManager.LoadScene(sceneToLoad);
    }

    public void GameOver()
    {
        panelGameOver.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void LoadSelectScene()
    {
        StartCoroutine(LoadScene());
    }

    private IEnumerator LoadScene()
    {
        panelLoad.SetActive(true);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Level_1");

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return new WaitForSeconds(5f);
        }
    }
}