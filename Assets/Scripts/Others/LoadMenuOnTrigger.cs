using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadMenuOnTrigger : MonoBehaviour
{
    public string sceneToLoad = "SelectLevel"; // Escena a cargar (menú principal)
    public Transform startPoint; // Referencia al GameObject "Start"

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verificar si el objeto que entra en el trigger es el jugador
        if (collision.CompareTag("Player"))
        {
            // Guardar la posición del GameObject "Start" en PlayerPrefs
            if (startPoint != null)
            {
                PlayerPrefs.SetFloat("StartPositionX", startPoint.position.x);
                PlayerPrefs.SetFloat("StartPositionY", startPoint.position.y);
            }

            // Limpiar todos los datos guardados en PlayerPrefs (excepto la posición de "Start")
            PlayerPrefs.DeleteAll();

            // Guardar la posición de "Start" nuevamente después de borrar PlayerPrefs
            if (startPoint != null)
            {
                PlayerPrefs.SetFloat("StartPositionX", startPoint.position.x);
                PlayerPrefs.SetFloat("StartPositionY", startPoint.position.y);
            }

            // Iniciar la corrutina para cargar la escena después de 2 segundos
            StartCoroutine(LoadMenuAfterDelay());
        }
    }

    private System.Collections.IEnumerator LoadMenuAfterDelay()
    {
        // Esperar 2 segundos
        yield return new WaitForSeconds(0.5f);

        // Cargar la escena del menú principal
        SceneManager.LoadScene(sceneToLoad);
    }
}