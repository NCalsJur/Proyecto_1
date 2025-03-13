using System.Collections;
using UnityEngine;

public class Skeleton : MonoBehaviour
{
    private CharacterController player;
    private Rigidbody2D rb;
    private SpriteRenderer sp;
    private Animator anim;
    private bool applyForce;

    public float playerDetection = 15;
    public float arrowDetection = 10;
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

        if (actualDistance <= playerDetection)
        {
            Vector2 direction = (player.transform.position - transform.position).normalized;
            Debug.DrawRay(transform.position, direction * playerDetection, Color.yellow);

            if (actualDistance <= arrowDetection)
            {
                rb.linearVelocity = Vector2.zero;
                anim.SetBool("Walk", false);
                ChangeView(direction.x);

                if (!shootingArrow)
                {
                    StartCoroutine(ShootArrow(direction));
                }
            }
            else
            {
                Vector2 movement = new Vector2(direction.x, 0).normalized;
                rb.linearVelocity = movement * skeletonSpeed;
                anim.SetBool("Walk", true);
                ChangeView(movement.x);
            }
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
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
        Gizmos.DrawWireSphere(transform.position, playerDetection);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, arrowDetection);
    }

    private IEnumerator ShootArrow(Vector2 arrowDirection)
    {
        shootingArrow = true;
        anim.SetBool("Shoot", true);
        yield return new WaitForSeconds(1.42f);
        anim.SetBool("Shoot", false);

        Vector2 playerPosition = player.transform.position;
        Vector2 skeletonPosition = transform.position;
        Vector2 direction = (playerPosition - skeletonPosition).normalized;

        GameObject arrowGo = Instantiate(arrow, transform.position, Quaternion.identity);
        Arrow arrowScript = arrowGo.GetComponent<Arrow>();
        Rigidbody2D arrowRb = arrowGo.GetComponent<Rigidbody2D>();
        SpriteRenderer arrowSprite = arrowGo.GetComponent<SpriteRenderer>();

        if (arrowScript != null && arrowRb != null && arrowSprite != null)
        {
            arrowScript.arrowDirection = direction;
            arrowScript.skeleton = this.gameObject;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            arrowGo.transform.rotation = Quaternion.Euler(0, 0, angle);

            arrowSprite.sortingLayerName = "Floor";
            arrowSprite.sortingOrder = 1;
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
            StartCoroutine(Die()); // Llamar a la nueva función de muerte
        }
    }

    private IEnumerator Die()
    {
        skeletonSpeed = 0;
        rb.linearVelocity = Vector2.zero;

        // Activar la animación de muerte
        anim.SetBool("IsDeath", true);

        // Cambiar al layer NPC_Background para evitar colisiones
        gameObject.layer = LayerMask.NameToLayer("NPC_Background");

        // Esperar 3 segundos antes de desaparecer
        yield return new WaitForSeconds(5f);

        // Destruir el objeto después del tiempo establecido
        Destroy(gameObject);
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