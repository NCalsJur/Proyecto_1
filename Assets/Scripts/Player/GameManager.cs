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
    public Text saveTextGame;

    public GameObject panelPause;
    public GameObject panelGameOver;
    public GameObject panelLoad;
    public int coins;

    private bool executing;

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

        if (PlayerPrefs.GetInt("Lifes") != 0)
        {
            LoadGame();
        }
    }

    public void SaveGame()
    {
        float x, y;
        x = player.transform.position.x;
        y = player.transform.position.y;

        int lifes = player.lifes;
        PlayerPrefs.SetInt("Coin", coins);
        PlayerPrefs.SetFloat("x", x);
        PlayerPrefs.SetFloat("y", y);
        PlayerPrefs.SetInt("Lifes", lifes);

        if (!executing)
        {
            StartCoroutine(ShowSaveText());
        }

    }

    private IEnumerator ShowSaveText()
    {
        executing = true;
        saveTextGame.gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        saveTextGame.gameObject.SetActive(false);
        executing = false;
    }

    public void LoadGame()
    {
        coins = PlayerPrefs.GetInt("Coin");
        player.transform.position = new Vector2(PlayerPrefs.GetFloat("x"), PlayerPrefs.GetFloat("y"));
        player.lifes = PlayerPrefs.GetInt("Lifes");
        coinText.text = coins.ToString();

        int descountLifes = 3 - player.lifes;
        player.UpdateUILifes(descountLifes);
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