using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject lifesUI;
    public CharacterController player; // Referencia al jugador
    public Text coinText;
    public Text saveTextGame;

    public GameObject panelPause;
    public GameObject panelGameOver;
    public GameObject panelLoad;
    public int coins;

    private bool executing;
    private bool isPaused = false; // Estado de pausa

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

        // Verificar si hay datos guardados y cargarlos
        if (PlayerPrefs.GetInt("Lifes") != 0)
        {
            LoadGame();
        }
        else
        {
            // Si no hay datos guardados, colocar al jugador en la posición de "Start"
            MovePlayerToStart();
        }
    }

    private void MovePlayerToStart()
    {
        // Verificar si hay una posición de "Start" guardada en PlayerPrefs
        if (PlayerPrefs.HasKey("StartPositionX") && PlayerPrefs.HasKey("StartPositionY"))
        {
            float startX = PlayerPrefs.GetFloat("StartPositionX");
            float startY = PlayerPrefs.GetFloat("StartPositionY");

            // Mover al jugador a la posición de "Start"
            if (player != null)
            {
                player.transform.position = new Vector2(startX, startY);
            }
        }
    }

    public void LoadSelected()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Level_1"); // Cargar el nivel actual
    }

    private void Update()
    {
        // Verificar si se presiona la tecla "Esc"
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                UnPauseGame(); // Si ya está en pausa, reanudar el juego
            }
            else
            {
                PauseGame(); // Si no está en pausa, pausar el juego
            }
        }
    }

    public void SaveGame()
    {
        if (player == null)
        {
            Debug.LogError("Player reference is null in SaveGame.");
            return;
        }

        float x = player.transform.position.x;
        float y = player.transform.position.y;

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

    public void LoadGame()
    {
        // Verificar si el jugador está asignado
        if (player == null)
        {
            Debug.LogError("Player reference is null in LoadGame.");
            return;
        }

        // Cargar datos guardados
        coins = PlayerPrefs.GetInt("Coin", coins); // Usar valor por defecto si no hay datos guardados
        float x = PlayerPrefs.GetFloat("x", player.transform.position.x);
        float y = PlayerPrefs.GetFloat("y", player.transform.position.y);
        player.lifes = PlayerPrefs.GetInt("Lifes", player.lifes);

        // Actualizar la posición del jugador
        player.transform.position = new Vector2(x, y);

        // Actualizar la interfaz de usuario
        coinText.text = coins.ToString();

        int descountLifes = 3 - player.lifes;
        player.UpdateUILifes(descountLifes);
    }

    private IEnumerator ShowSaveText()
    {
        executing = true;
        saveTextGame.gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        saveTextGame.gameObject.SetActive(false);
        executing = false;
    }

    public void UpdateCoinCounter()
    {
        coins++;
        coinText.text = coins.ToString();
    }

    public void PauseGame()
    {
        Time.timeScale = 0; // Pausar el tiempo del juego
        panelPause.SetActive(true); // Activar el panel de pausa
        isPaused = true; // Actualizar el estado de pausa
    }

    public void UnPauseGame()
    {
        Time.timeScale = 1; // Reanudar el tiempo del juego
        panelPause.SetActive(false); // Desactivar el panel de pausa
        isPaused = false; // Actualizar el estado de pausa
    }

    public void BackToMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("SelectLevel");
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

        while (!asyncLoad.isDone)
        {
            yield return new WaitForSeconds(5f);
        }
    }
}