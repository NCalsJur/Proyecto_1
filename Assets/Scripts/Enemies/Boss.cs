using System.Collections;
using UnityEngine;

public class Boss : MonoBehaviour
{
    private CharacterController player;
    private Rigidbody2D rb;
    private SpriteRenderer sp;
    private Animator anim;

    public int bossLife = 3;
    public float bossSpeed = 3f;
    public float attackRange = 1.5f; // Área de ataque
    public float detectionRange = 5f; // Área de detección
    public LayerMask playerLayer;

    // Waypoints
    public Transform[] waypoints; // Arreglo de waypoints
    private int currentWaypointIndex = 0; // Índice del waypoint actual
    public float waitTime = 2f; // Tiempo de espera en cada waypoint
    private bool isMovingBetweenWaypoints = true; // Controla si el jefe se mueve entre waypoints

    private bool isAttacking;
    private bool isDead = false;
    private bool isFacingRight = true;
    private bool applyForce;
    private Coroutine attackCoroutine;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sp = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>();
        gameObject.name = "Boos_1";

        // Iniciar el movimiento entre waypoints
        if (waypoints.Length > 0)
        {
            StartCoroutine(MoveBetweenWaypoints());
        }
    }

    void Update()
    {
        if (isDead) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
        Debug.Log("Distancia al jugador: " + distanceToPlayer); // Depuración

        if (distanceToPlayer <= detectionRange)
        {
            Debug.Log("Jugador detectado"); // Depuración
            isMovingBetweenWaypoints = false; // Detener el movimiento entre waypoints
            if (distanceToPlayer <= attackRange && !isAttacking)
            {
                attackCoroutine = StartCoroutine(AttackCombo());
            }
            else if (!isAttacking)
            {
                ChasePlayer();
            }
        }
        else
        {
            Debug.Log("Jugador fuera de rango"); // Depuración
            anim.SetBool("IsRunning", false);
            anim.SetBool("IsWalking", false);

            // Reanudar el movimiento entre waypoints si no estaba activo
            if (!isMovingBetweenWaypoints)
            {
                isMovingBetweenWaypoints = true;
                StartCoroutine(MoveBetweenWaypoints());
            }
        }
    }

    void ChasePlayer()
    {
        if (player == null) return;

        // Determinar si el boss está corriendo o caminando
        if (bossSpeed > 2f) // Ejemplo: Si la velocidad es alta, está corriendo
        {
            anim.SetBool("IsRunning", true);
            anim.SetBool("IsWalking", false);
        }
        else // Si la velocidad es baja, está caminando
        {
            anim.SetBool("IsRunning", false);
            anim.SetBool("IsWalking", true);
        }

        Vector2 targetPosition = new Vector2(player.transform.position.x, transform.position.y);
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, bossSpeed * Time.deltaTime);
        FlipTowardsPlayer();
    }

    void FlipTowardsPlayer()
    {
        if ((player.transform.position.x < transform.position.x && isFacingRight) || (player.transform.position.x > transform.position.x && !isFacingRight))
        {
            isFacingRight = !isFacingRight;
            transform.Rotate(0f, 180f, 0f);
        }
    }

    IEnumerator AttackCombo()
    {
        isAttacking = true;

        // Detener movimiento y animaciones de caminar/correr
        anim.SetBool("IsRunning", false);
        anim.SetBool("IsWalking", false);

        // Primera parte del ataque
        anim.SetBool("IsAttacking_1", true);
        yield return new WaitForSeconds(0.5f);
        DealDamage();
        anim.SetBool("IsAttacking_1", false);

        yield return new WaitForSeconds(0.5f);

        // Segunda parte del ataque
        anim.SetBool("IsAttacking_2", true);
        DealDamage();
        yield return new WaitForSeconds(0.5f);
        anim.SetBool("IsAttacking_2", false);

        isAttacking = false;
    }

    void DealDamage()
    {
        Collider2D hitPlayer = Physics2D.OverlapCircle(transform.position, attackRange, playerLayer);
        if (hitPlayer != null)
        {
            CharacterController playerController = hitPlayer.GetComponent<CharacterController>();
            if (playerController != null)
            {
                playerController.GetDamage((playerController.transform.position - transform.position).normalized);
            }
        }
    }

    public void GetDamage()
    {
        if (bossLife > 0)
        {
            StartCoroutine(DamageEffect());
            applyForce = true;
            bossLife--;
            Debug.Log("Boss recibió daño. Vida restante: " + bossLife); // Depuración
        }
        else
        {
            Debug.Log("Boss muriendo..."); // Depuración
            StartCoroutine(Die());
        }
    }

    private IEnumerator DamageEffect()
    {
        sp.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        sp.color = Color.white;
    }

    private IEnumerator Die()
    {
        Debug.Log("Boss está muriendo..."); // Depuración
        isDead = true;
        bossSpeed = 0;
        rb.linearVelocity = Vector2.zero;

        // Activar animación de muerte
        anim.SetBool("IsDeath", true);
        Debug.Log("Animación de muerte activada: " + anim.GetBool("IsDeath")); // Depuración

        // Desactivar otras animaciones
        anim.SetBool("IsRunning", false);
        anim.SetBool("IsWalking", false);
        anim.SetBool("IsAttacking_1", false);
        anim.SetBool("IsAttacking_2", false);

        // Cambiar de layer para evitar colisiones
        gameObject.layer = LayerMask.NameToLayer("NPC_Background");

        StopAllCoroutines();

        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player.GetDamage((transform.position - player.transform.position).normalized);
        }
    }

    // Corrutina para moverse entre waypoints
    private IEnumerator MoveBetweenWaypoints()
    {
        while (isMovingBetweenWaypoints && waypoints.Length > 0)
        {
            // Obtener la posición del waypoint actual
            Vector2 targetPosition = waypoints[currentWaypointIndex].position;

            // Mover al jefe hacia el waypoint
            while (Vector2.Distance(transform.position, targetPosition) > 0.1f)
            {
                transform.position = Vector2.MoveTowards(transform.position, targetPosition, bossSpeed * Time.deltaTime);
                anim.SetBool("IsWalking", true); // Activar animación de caminar
                FlipTowardsTarget(targetPosition); // Girar hacia el waypoint
                yield return null;
            }

            // Llegó al waypoint, esperar un tiempo
            anim.SetBool("IsWalking", false); // Desactivar animación de caminar
            yield return new WaitForSeconds(waitTime);

            // Avanzar al siguiente waypoint
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }

    // Girar hacia el waypoint actual
    private void FlipTowardsTarget(Vector2 targetPosition)
    {
        if ((targetPosition.x < transform.position.x && isFacingRight) || (targetPosition.x > transform.position.x && !isFacingRight))
        {
            isFacingRight = !isFacingRight;
            transform.Rotate(0f, 180f, 0f);
        }
    }

    // Dibuja Gizmos en el editor
    private void OnDrawGizmosSelected()
    {
        // Dibuja el área de detección (color amarillo)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Dibuja el área de ataque (color rojo)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Dibuja los waypoints (color azul)
        if (waypoints != null && waypoints.Length > 0)
        {
            Gizmos.color = Color.blue;
            for (int i = 0; i < waypoints.Length; i++)
            {
                if (waypoints[i] != null)
                {
                    Gizmos.DrawSphere(waypoints[i].position, 0.2f);
                    if (i < waypoints.Length - 1 && waypoints[i + 1] != null)
                    {
                        Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
                    }
                }
            }
        }
    }
}