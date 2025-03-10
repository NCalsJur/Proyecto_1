using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D bc;

    public Vector2 arrowDirection;
    public GameObject skeleton; // Referencia al esqueleto que disparó la flecha
    public LayerMask groundLayer; // Capa de los tiles
    public float lifetime = 3f; // Tiempo de vida de la flecha si no colisiona con nada
    private bool isStuck = false; // Indica si la flecha está incrustada en un tile

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

        // Destruir la flecha después de un tiempo si no colisiona con nada
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verificar si la flecha colisiona con un tile
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            StickToTile();
        }
        // Verificar si la flecha colisiona con el jugador y no está incrustada
        else if (collision.CompareTag("Player") && !isStuck)
        {
            collision.GetComponent<CharacterController>().GetDamage(-(collision.transform.position - skeleton.transform.position).normalized);
            Destroy(gameObject); // La flecha se destruye al impactar con el jugador
        }
    }

    private void StickToTile()
    {
        isStuck = true; // La flecha está incrustada
        rb.linearVelocity = Vector2.zero; // Detener el movimiento
        rb.bodyType = RigidbodyType2D.Static; // Hacer la flecha estática
        bc.enabled = false; // Desactivar el collider para que no cause más colisiones

        // Destruir la flecha después de un tiempo
        Destroy(gameObject, 2f); // Destruir después de 2 segundos
    }
}
