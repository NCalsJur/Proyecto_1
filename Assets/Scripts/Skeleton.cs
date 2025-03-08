using System.Collections;
using UnityEngine;

public class Skeleton : MonoBehaviour
{
    private CharacterController player;
    private Rigidbody2D rb;
    private SpriteRenderer sp;
    private Animator anim;
    private bool applyForce;

    public float playerDetection = 15; // Rango para detectar al jugador y moverse hacia él
    public float arrowDetection = 10;  // Rango para disparar flechas
    public float arrowStrength = 5f;
    public float skeletonSpeed;
    public int skeletonLife = 3;
    public bool shootingArrow;
    public GameObject arrow;

    private void Awake()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.GetComponent<CharacterController>();
        }
        else
        {
            Debug.LogError("No se encontró un objeto con el tag 'Player'.");
        }

        rb = GetComponent<Rigidbody2D>();
        sp = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        if (rb == null) Debug.LogError("No se encontró un componente Rigidbody2D en el objeto.");
        if (sp == null) Debug.LogError("No se encontró un componente SpriteRenderer en el objeto.");
        if (anim == null) Debug.LogError("No se encontró un componente Animator en el objeto.");
    }

    void Start()
    {
        gameObject.name = "Skeleton";
    }

    void Update()
    {
        float actualDistance = Vector2.Distance(transform.position, player.transform.position);

        // Si el jugador está dentro del rango de detección
        if (actualDistance <= playerDetection)
        {
            Vector2 direction = (player.transform.position - transform.position).normalized;
            Debug.DrawRay(transform.position, direction * playerDetection, Color.yellow);

            // Si el jugador está dentro del rango de disparo
            if (actualDistance <= arrowDetection)
            {
                rb.linearVelocity = Vector2.zero; // Detener el movimiento
                anim.SetBool("Walk", false);

                // Cambiar la dirección del esqueleto
                ChangeView(direction.x);

                // Disparar una flecha si no está disparando
                if (!shootingArrow)
                {
                    StartCoroutine(ShootArrow(direction));
                }
            }
            else // Si el jugador está fuera del rango de disparo pero dentro del rango de detección
            {
                // Moverse hacia el jugador
                Vector2 movement = new Vector2(direction.x, 0).normalized;
                rb.linearVelocity = movement * skeletonSpeed;
                anim.SetBool("Walk", true);
                ChangeView(movement.x);
            }
        }
        else // Si el jugador está fuera del rango de detección
        {
            rb.linearVelocity = Vector2.zero; // Detener el movimiento
            anim.SetBool("Walk", false);
        }
    }

    private void ChangeView(float directionX)
    {
        if (directionX < 0 && transform.localScale.x > 0)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        else if (directionX > 0 && transform.localScale.x < 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, playerDetection); // Rango de detección del jugador
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, arrowDetection);  // Rango de disparo
    }

    private IEnumerator ShootArrow(Vector2 arrowDirection)
    {
        shootingArrow = true;
        anim.SetBool("Shoot", true);
        yield return new WaitForSeconds(1.42f); // Tiempo de la animación de disparo
        anim.SetBool("Shoot", false);

        // Calcular la dirección hacia el jugador
        Vector2 playerPosition = player.transform.position;
        Vector2 skeletonPosition = transform.position;
        Vector2 direction = (playerPosition - skeletonPosition).normalized;

        // Instanciar la flecha en la posición exacta del esqueleto
        GameObject arrowGo = Instantiate(arrow, transform.position, Quaternion.identity); // Elimina el desplazamiento vertical
        Arrow arrowScript = arrowGo.GetComponent<Arrow>();
        Rigidbody2D arrowRb = arrowGo.GetComponent<Rigidbody2D>();
        SpriteRenderer arrowSprite = arrowGo.GetComponent<SpriteRenderer>();

        if (arrowScript != null && arrowRb != null && arrowSprite != null)
        {
            arrowScript.arrowDirection = direction; // Asignar la dirección correcta
            arrowScript.skeleton = this.gameObject;

            // Rotar la flecha hacia la dirección del jugador
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            arrowGo.transform.rotation = Quaternion.Euler(0, 0, angle);

            arrowSprite.sortingLayerName = "Floor"; // Asegúrate de que esté en la capa correcta
            arrowSprite.sortingOrder = 1; // Ajusta este valor según sea necesario
        }
        else
        {
            Debug.LogError("El objeto 'arrow' no tiene los componentes necesarios.");
        }

        shootingArrow = false;
    }

    public void GetDamage()
    {
        if (skeletonLife > 0)
        {
            StartCoroutine(DamageEffect());
            applyForce = true;
            skeletonLife--;
        }
        else
        {
            skeletonSpeed = 0;
            rb.linearVelocity = Vector2.zero;
            Destroy(gameObject, 0.2f);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player.GetDamage((transform.position - player.transform.position).normalized);
        }
    }

    private IEnumerator DamageEffect()
    {
        sp.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        sp.color = Color.white;
    }

    private void FixedUpdate()
    {
        if (applyForce)
        {
            rb.AddForce((transform.position - player.transform.position).normalized * 100, ForceMode2D.Impulse);
            applyForce = false;
        }
    }
}
