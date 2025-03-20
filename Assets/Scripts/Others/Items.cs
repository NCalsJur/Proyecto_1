using System.Collections;
using UnityEngine;

public class Items : MonoBehaviour
{
    private Animator anim;
    private AudioSource audioSource; // Referencia al AudioSource del ítem
    private bool isPickedUp = false;

    [Header("Sounds")]
    public AudioClip pickUpSound; // Sonido para monedas y pociones
    public AudioClip powerUpSound; // Sonido para power-ups

    private void Awake()
    {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>(); // Obtener el componente AudioSource
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isPickedUp)
        {
            isPickedUp = true;
            AssignItem();
        }
    }

    private void AssignItem()
    {
        if (gameObject.CompareTag("Coin"))
        {
            GameManager.instance.UpdateCoinCounter();
            PlaySound(pickUpSound); // Reproducir sonido de recogida
        }
        else if (gameObject.CompareTag("PowerUp"))
        {
            GameManager.instance.player.GiveImmortality();
            PlaySound(powerUpSound); // Reproducir sonido de power-up
        }
        else if (gameObject.CompareTag("Heal"))
        {
            PlaySound(pickUpSound); // Reproducir sonido de recogida
            CharacterController player = GameManager.instance.player;
            if (player.lifes < 5)
            {
                player.lifes++;
                player.UpdateUILifes(-1); // Ahora correctamente activa una vida en la UI
            }
        }

        // Activar animación de "PickUpEffect"
        anim.SetBool("PickUp", true);

        // Esperar a que termine la animación y destruir el objeto
        StartCoroutine(DestroyAfterAnimation());
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip); // Reproducir el sonido
        }
    }

    private IEnumerator DestroyAfterAnimation()
    {
        // Esperar la duración de la animación
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);

        // Destruir el objeto
        Destroy(gameObject);
    }
}
