using System.Collections;
using UnityEngine;

public class Items : MonoBehaviour
{
    private Animator anim;
    private bool isPickedUp = false;

    private void Awake()
    {
        anim = GetComponent<Animator>();
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
        }
        else if (gameObject.CompareTag("PowerUp"))
        {
            GameManager.instance.player.GiveInmortality();
        }
        else if (gameObject.CompareTag("Heal"))
        {
            CharacterController player = GameManager.instance.player;
            if (player.lifes < 5)
            {
                player.lifes++;
                player.UpdateUILifes(-1); // Ahora correctamente activa una vida en la UI
            }
        }

        // Activar animaci�n de "PickUpEffect"
        anim.SetBool("PickUp", true);

        // Esperar a que termine la animaci�n y destruir el objeto
        StartCoroutine(DestroyAfterAnimation());
    }

    private IEnumerator DestroyAfterAnimation()
    {
        // Esperar la duraci�n de la animaci�n
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);

        // Destruir el objeto
        Destroy(gameObject);
    }
}
