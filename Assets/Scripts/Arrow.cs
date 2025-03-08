using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D bc;

    public Vector2 arrowDirection;
    public GameObject skeleton; // Referencia al esqueleto que disparó la flecha

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();

        // Asegurar que el Rigidbody2D sea Dynamic
        rb.bodyType = RigidbodyType2D.Dynamic;
    }

    private void Start()
    {
        // Aplicar la velocidad a la flecha
        rb.linearVelocity = arrowDirection * 10f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Aquí podrías aplicar daño al jugador utilizando la referencia a `skeleton`
            Destroy(gameObject); // La flecha se destruye al impactar con el jugador
        }
    }
}