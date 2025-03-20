using System.Collections;
using UnityEngine;

public class Skeleton : MonoBehaviour
{
    private CharacterController player;
    private Rigidbody2D rb;
    private SpriteRenderer sp;
    private Animator anim;
    public GameObject arrow;

    private bool applyForce;
    private bool isDead = false;
    public bool shootingArrow;
    public bool isSentinel = false; // Nuevo: si es centinela, no se moverá

    public int skeletonLife = 3;
    public float playerDetection = 15;
    public float arrowDetection = 10;
    public float arrowStrength = 5f;
    public float skeletonSpeed;

    public float fireRate = 2f;
    private float lastShotTime = 2f;

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
        sp.flipX = (transform.localScale.x < 0);

        // Si es centinela, aseguramos que el Rigidbody2D esté en modo Kinematic
        if (isSentinel)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.linearVelocity = Vector2.zero;
        }
    }

    void Update()
    {
        if (isDead) return;

        float actualDistance = Vector2.Distance(transform.position, player.transform.position);

        if (actualDistance <= playerDetection)
        {
            Vector2 direction = (player.transform.position - transform.position).normalized;
            Debug.DrawRay(transform.position, direction * playerDetection, Color.yellow);

            sp.flipX = player.transform.position.x < transform.position.x;

            if (actualDistance <= arrowDetection)
            {
                rb.linearVelocity = Vector2.zero;
                anim.SetBool("Walk", false);

                if (!shootingArrow && Time.time >= lastShotTime + fireRate)
                {
                    StartCoroutine(ShootArrow(direction));
                    lastShotTime = Time.time;
                }
            }
            else if (!isSentinel) // Solo los esqueletos normales se mueven
            {
                Vector2 movement = new Vector2(direction.x, 0).normalized;
                rb.linearVelocity = movement * skeletonSpeed;
                anim.SetBool("Walk", true);
            }
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            anim.SetBool("Walk", false);
        }
    }

    private IEnumerator ShootArrow(Vector2 arrowDirection)
    {
        if (isDead) yield break;

        shootingArrow = true;
        anim.SetBool("Shoot", true);
        yield return new WaitForSeconds(1.4f);
        anim.SetBool("Shoot", false);

        if (isDead) yield break;

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
            StartCoroutine(Die());
        }
    }

    private IEnumerator Die()
    {
        isDead = true;
        skeletonSpeed = 0;
        rb.linearVelocity = Vector2.zero;

        anim.SetBool("IsDeath", true);
        gameObject.layer = LayerMask.NameToLayer("NPC_Background");

        StopAllCoroutines();

        yield return new WaitForSeconds(5f);
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
        if (applyForce && !isSentinel)
        {
            rb.AddForce((transform.position - player.transform.position).normalized * 100, ForceMode2D.Impulse);
            applyForce = false;
        }
    }

    // Dibuja los Gizmos de los rangos de detección
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, playerDetection);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, arrowDetection);
    }
}
