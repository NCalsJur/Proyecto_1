using UnityEngine;

public class LoadMenuOnTrigger : MonoBehaviour
{
    public string sceneToLoad = "SelectLevel"; // Escena a cargar (men� principal)

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verificar si el objeto que entra en el trigger es el jugador
        if (collision.CompareTag("Player"))
        {
            // Iniciar la corrutina para cargar la escena despu�s de 2 segundos
            StartCoroutine(LoadMenuAfterDelay());
        }
    }

    private System.Collections.IEnumerator LoadMenuAfterDelay()
    {
        // Esperar 2 segundos
        yield return new WaitForSeconds(2f);

        // Cargar la escena del men� principal
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToLoad);
    }
}