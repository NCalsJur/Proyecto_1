using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatWaypoints : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private CharacterController player;
    private Animator anim;
    private int actualPosition = 0;
    private bool isMoving = true;
    private bool chasingPlayer = false;
    private bool attackingPlayer = false;
    private bool isDead = false; // Nueva variable para evitar que haga otras acciones tras morir
    private Coroutine returnToRoutine;

    public LayerMask playerLayer;
    public float movementVelocity = 3;
    public float detectionRadius = 15;
    public float attackRadius = 3;
    public float chaseSpeed = 4f;
    public float chaseTime = 5f;
    public int batLife = 3;
    public string batName;
    public Vector2 headPosition;
    public List<Transform> points = new List<Transform>();

    [Header("Tiempos de Espera en Puntos")]
    public float waitTimeAtPoint1 = 2f;
    public float waitTimeAtPoint2 = 2f;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>();
    }

    void Start()
    {
        gameObject.name = batName;
        anim.SetBool("IsFlying", true);
    }

    void Update()
    {
        if (isDead) return; // No ejecuta el código si está muerto

        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

        if (distanceToPlayer <= attackRadius)
        {
            if (!attackingPlayer)
            {
                StartCoroutine(AttackPlayer());
            }
        }
        else if (distanceToPlayer <= detectionRadius)
        {
            if (!chasingPlayer)
            {
                chasingPlayer = true;
                isMoving = false;
                anim.SetBool("IsFlying", true);
                anim.SetBool("IsHanging", false);
                if (returnToRoutine != null)
                    StopCoroutine(returnToRoutine);
            }
            ChasePlayer();
        }
        else if (chasingPlayer)
        {
            chasingPlayer = false;
            returnToRoutine = StartCoroutine(ReturnToRoutine());
        }
        else if (isMoving)
        {
            MovementWaypoints();
        }
    }

    private void MovementWaypoints()
    {
        Vector2 direction = (points[actualPosition].position - transform.position).normalized;
        rb.linearVelocity = direction * movementVelocity;

        if (Vector2.Distance(transform.position, points[actualPosition].position) <= 0.7f)
        {
            StartCoroutine(WaitAtWaypoint());
        }

        ChangeView(direction.x);
    }

    private IEnumerator WaitAtWaypoint()
    {
        isMoving = false;
        rb.linearVelocity = Vector2.zero;

        if (actualPosition == 0) // Llegó al punto 1
        {
            anim.SetBool("IsHanging", true);
            anim.SetBool("IsFlying", false);
            yield return new WaitForSeconds(waitTimeAtPoint1);
        }
        else // Llegó al punto 2
        {
            anim.SetBool("IsFlying", true);
            anim.SetBool("IsHanging", false);
            yield return new WaitForSeconds(waitTimeAtPoint2);
        }

        // Antes de moverse al siguiente punto, aseguramos que haga la animación de "Fly"
        anim.SetBool("IsFlying", true);
        anim.SetBool("IsHanging", false);

        actualPosition = (actualPosition + 1) % points.Count;
        isMoving = true;
    }

    private void ChasePlayer()
    {
        Vector2 direction = (player.transform.position - transform.position).normalized;
        rb.linearVelocity = direction * chaseSpeed;
        ChangeView(direction.x);
    }

    private IEnumerator ReturnToRoutine()
    {
        yield return new WaitForSeconds(chaseTime);
        isMoving = true;
    }

    private IEnumerator AttackPlayer()
    {
        attackingPlayer = true;
        rb.linearVelocity = Vector2.zero;
        anim.SetBool("IsAttacking", true);
        yield return new WaitForSeconds(1f);
        anim.SetBool("IsAttacking", false);
        attackingPlayer = false;
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player.GetDamage((transform.position - player.transform.position).normalized);
        }
    }

    public void GetDamage()
    {
        if (isDead) return; // Si ya está muerto, no recibe más daño

        batLife--;

        if (batLife <= 0)
        {
            StartCoroutine(Die());
        }
        else
        {
            StartCoroutine(DamageEffect());
        }
    }

    private IEnumerator DamageEffect()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = Color.white;
    }

    private IEnumerator Die()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero; // Se detiene en el aire
        rb.gravityScale = 1.5f; // Activar gravedad para que caiga
        rb.constraints = RigidbodyConstraints2D.None; // Permitir que rote mientras cae
        anim.SetBool("IsDeath", true); // Activar animación de muerte
        gameObject.layer = LayerMask.NameToLayer("NPC_Background"); // Cambiar layer

        yield return new WaitForSeconds(5f); // Esperar 5 segundos antes de desaparecer

        Destroy(gameObject); // Destruir el murciélago
    }

    private void OnDrawGizmosSelected()
    {
        // Área de detección (color amarillo)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Área de ataque (color rojo)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}