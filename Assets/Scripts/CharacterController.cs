using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class CharacterController : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private Vector2 direction;

    [Header("Stats")]
    public float MovementVelocity = 10;
    public float JumpStrenght = 5;

    [Header("Jump Gravity Settings")]
    public float FallMultiplier = 2.5f;      // Multiplicador de gravedad al caer
    public float LowJumpMultiplier = 2f;    // Multiplicador de gravedad si se suelta el botón de salto antes de tiempo

    [Header("Collisions")]
    public Vector2 down;
    public float radioDetection;
    public LayerMask layerFloor;

    [Header("Booleans")]
    public bool canMove = true;
    public bool ground = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        Movement();
        CheckGround();
    }

    private void Movement()
    {
        float x = Input.GetAxis("Horizontal");
        direction = new Vector2(x, 0);
        Walk();
        BetterJump();

        if (Input.GetKeyDown(KeyCode.Space) && ground)
        {
            anim.SetBool("Jump", true);
            anim.SetBool("Fall", false);
            Jump();
        }
    }

    public void EndJump()
    {
        anim.SetBool("Jump", false);
    }

    private void Walk()
    {
        if (canMove)
        {
            rb.linearVelocity = new Vector2(direction.x * MovementVelocity, rb.linearVelocity.y);

            if (ground && Mathf.Abs(direction.x) > 0)
            {
                anim.SetBool("Walk", true);
                FlipSprite(direction.x);
            }
            else
            {
                anim.SetBool("Walk", false);
            }
        }
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        rb.linearVelocity += Vector2.up * JumpStrenght;
    }

    private void BetterJump()
    {
        if (rb.linearVelocity.y > 0 && !ground)
        {
            anim.SetBool("Jump", true);
            anim.SetBool("Fall", false);
            FlipSprite(rb.linearVelocity.x);
        }
        else if (rb.linearVelocity.y < 0 && !ground)
        {
            anim.SetBool("Jump", false);
            anim.SetBool("Fall", true);
            FlipSprite(rb.linearVelocity.x);
        }

        // 🛠️ Aplicación de gravedad ajustable:
        if (rb.linearVelocity.y < 0)  // Si está cayendo, aplicar multiplicador de caída
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (FallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.linearVelocity.y > 0 && !Input.GetKey(KeyCode.Space))  // Si suelta el botón de salto, aplicar caída más rápida
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (LowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    private void CheckGround()
    {
        bool wasGrounded = ground;
        ground = Physics2D.OverlapCircle((Vector2)transform.position + down, radioDetection, layerFloor);

        if (ground && !wasGrounded)
        {
            anim.SetBool("Fall", false);
            anim.SetBool("Jump", false);
        }
    }

    private void FlipSprite(float direction)
    {
        if (direction != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(direction), transform.localScale.y, transform.localScale.z);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere((Vector2)transform.position + down, radioDetection);
    }
}











