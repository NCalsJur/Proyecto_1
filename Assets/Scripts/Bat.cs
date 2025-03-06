using System.Collections;
using UnityEngine;


public class Bat : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private CharacterController player;
    private bool applyForce;


    public LayerMask playerLayer;
    public float movementVelocity = 3;
    public float detectionRadius = 15;

    public int batLife = 3;
    public string batName;
    public Vector2 headPosition;


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>();

    }

    
    void Start()
    {
        gameObject.name = batName;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.DrawCube((Vector2)transform.position + headPosition, new Vector2(1, 0.5f) * 0.7f);
    }

    void Update()
    {
        Vector2 direction = player.transform.position - transform.position;
        float distance = Vector2.Distance(transform.position, player.transform.position);

        if (distance < detectionRadius)
        {
            rb.linearVelocity = direction.normalized * movementVelocity;
            ChangeView(direction.normalized.x);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (transform.position.y + headPosition.y < player.transform.position.y - 0.8f)
            {
                player.GetComponent<Rigidbody2D>().linearVelocity = Vector2.up * player.JumpStrenght;
                Destroy(gameObject);
            }
            else
            {
                //player.GetDamage((transform.position - player.transform.position).normalized);
            }
        }

    }

    private void FixedUpdate()
    {
        if (applyForce)
        {
            rb.AddForce((transform.position - player.transform.position).normalized *100, ForceMode2D.Impulse);
            applyForce = false;
        }
    }

    public void GetDamage()
    {


        if (batLife > 0)
        {
            StartCoroutine(DamageEffect());
            applyForce = true;
            batLife--;
        }
        else
        {
            Destroy(gameObject, 0.2f);
        }
    }



    private IEnumerator DamageEffect()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = Color.white;
    }
}
