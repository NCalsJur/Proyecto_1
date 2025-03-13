using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderWaypoints : MonoBehaviour
{
    private Vector3 direction;
    private CharacterController player;
    private Rigidbody2D rb;
    private SpriteRenderer sp;
    private Animator anim;
    private int actualPosition = 0;
    private bool applyForce;
    private bool isMoving = true; // Controla si la araña está en movimiento

    public Vector2 headPosition;
    public int life = 3;
    public float movementSpeed;
    public float waitTime = 2f; // Tiempo que espera en cada waypoint
    public float despawnTime = 3f; // Tiempo antes de que desaparezca tras morir
    public List<Transform> points = new List<Transform>();

    private void Awake()
    {
        sp = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>(); // Obtiene el Animator
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>();
    }

    private void Start()
    {
        if (gameObject.CompareTag("Enemy"))
        {
            gameObject.name = "Spider";
        }

        anim.SetBool("Run", true); // Empieza en movimiento
    }

    private void FixedUpdate()
    {
        if (isMoving)
        {
            MovementWaypoints();
        }

        if (gameObject.CompareTag("Enemy"))
        {
            ChangeScaleEnemy();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && gameObject.CompareTag("Enemy"))
        {
            if (player.transform.position.y - 0.7f > transform.position.y + headPosition.y)
            {
                player.GetComponent<Rigidbody2D>().linearVelocity = Vector2.up * player.JumpStrenght;
                StartCoroutine(Die());
            }
            else
            {
                player.GetDamage(-(player.transform.position - transform.position).normalized);
            }
        }
    }

    private void ChangeScaleEnemy()
    {
        if (direction.x < 0 && transform.localScale.x > 0)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        else if (direction.x > 0 && transform.localScale.x < 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    private void MovementWaypoints()
    {
        direction = (points[actualPosition].position - transform.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, points[actualPosition].position, movementSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, points[actualPosition].position) <= 0.7f)
        {
            StartCoroutine(WaitAtWaypoint());
        }
    }

    private IEnumerator WaitAtWaypoint()
    {
        isMoving = false;
        anim.SetBool("Run", false); // Cambia a animación de Idle
        yield return new WaitForSeconds(waitTime);

        actualPosition++;
        if (actualPosition >= points.Count)
        {
            actualPosition = 0;
        }

        anim.SetBool("Run", true); // Vuelve a la animación de Run
        isMoving = true;
    }

    public void GetDamage()
    {
        if (life > 0)
        {
            StartCoroutine(DamageEffect());
            applyForce = true;
            life--;
        }
        else
        {
            StartCoroutine(Die()); // Llama a la nueva función de muerte
        }
    }

    private IEnumerator Die()
    {
        movementSpeed = 0;
        rb.linearVelocity = Vector2.zero;
        isMoving = false;

        // Activar animación de muerte
        anim.SetBool("IsDeath", true);

        // Cambiar al layer NPC_Background para evitar colisiones
        gameObject.layer = LayerMask.NameToLayer("NPC_Background");

        // Esperar antes de desaparecer
        yield return new WaitForSeconds(despawnTime);

        // Destruir el objeto
        Destroy(gameObject);
    }

    private IEnumerator DamageEffect()
    {
        sp.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        sp.color = Color.white;
    }
}