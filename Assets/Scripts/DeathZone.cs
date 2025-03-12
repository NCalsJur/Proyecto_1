using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        CharacterController player = other.GetComponent<CharacterController>();

        if (player != null)
        {
            player.Respawn(); // Vuelve a la última posición segura
        }
    }
}

