using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class CharacterController : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private TrailRenderer trail;

    private Vector2 direction;
    private Vector2 movementDirection;

    private float originalGravity;

    [Header("Stats")]
    public float MovementVelocity = 10;
    public float JumpStrenght = 15;
    public float dashVelocity = 10;

    [Header("Jump Gravity Settings")]
    public float FallMultiplier = 2.5f;
    public float LowJumpMultiplier = 2f;

    [Header("Collisions")]
    public Vector2 down;
    public float radioDetection;
    public LayerMask layerFloor;

    [Header("Booleans")]
    public bool canMove = true;
    public bool ground = true;
    public bool canDash;
    public bool dash;
    public bool groundTouched;
    public bool hasDashedInAir;
    public bool attacking;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        trail = transform.Find("Trail").GetComponent<TrailRenderer>();
        if (trail != null) trail.enabled = false;
        originalGravity = rb.gravityScale;
    }

    private void Update()
    {
        Movement();
        CheckGround();
    }

    private void Attack(Vector2 direction)
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (!attacking && !dash)
            {
                attacking = true;

                anim.SetFloat("Attack_X", direction.x);
                anim.SetFloat("Attack_Y", direction.y);

                anim.SetBool("Attack", true);
            }
        }
    }

    public void EndAttack()
    {
        anim.SetBool("Attack", false);
        attacking = false;
    }

    private Vector2 AttackDirection(Vector2 movementDirection, Vector2 direction)
    {
        if (rb.linearVelocity.x == 0 && direction.y != 0)
        {
            return new Vector2(0, direction.y);
        }

        return new Vector2(movementDirection.x, direction.y);
    }

    private void Dash(float x, float y)
    {
        if (!ground && hasDashedInAir) return; // Permitir solo un dash en el aire

        anim.SetBool("Roll", true);
        if (trail != null) trail.enabled = true; // Activar Trail

        canDash = true;
        rb.linearVelocity = Vector2.zero;
        rb.linearVelocity = new Vector2(x, y).normalized * dashVelocity;

        if (!ground) hasDashedInAir = true;

        FlipSprite(x); // Voltear el sprite en la dirección del dash

        StartCoroutine(PrepareDash());
    }

    private IEnumerator PrepareDash()
    {
        rb.gravityScale = 0;
        dash = true;

        yield return new WaitForSeconds(0.3f);
        rb.gravityScale = originalGravity;
        dash = false;
        EndDash();
    }

    public void EndDash()
    {
        anim.SetBool("Roll", false);
        if (trail != null) trail.enabled = false; // Desactivar Trail
    }

    private void TouchGround()
    {
        canDash = false;
        dash = false;
        hasDashedInAir = false; // Resetear el dash al tocar el suelo
        anim.SetBool("Jump", false);
    }

    private void Movement()
    {
        float x = Input.GetAxis("Horizontal");
        float xRaw = Input.GetAxisRaw("Horizontal");
        float yRaw = Input.GetAxisRaw("Vertical");

        direction = new Vector2(x, 0);
        Vector2 directionRaw = new Vector2(xRaw, yRaw);
        Walk();
        BetterJump();
        Attack(AttackDirection(movementDirection, directionRaw));


        if (Input.GetKeyDown(KeyCode.Space) && ground)
        {
            anim.SetBool("Jump", true);
            anim.SetBool("Fall", false);
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.X) && !dash)
        {
            Dash(xRaw, 0); // Dash solo en horizontal
        }

        if (ground && !groundTouched)
        {
            TouchGround();
            groundTouched = true;
        }
        else if (!ground)
        {
            groundTouched = false;
        }
    }

    private void Walk()
    {
        if (canMove && !dash)
        {
            movementDirection = AttackDirection(Vector2.left, direction);

            rb.linearVelocity = new Vector2(direction.x * MovementVelocity, rb.linearVelocity.y);

            anim.SetBool("Walk", ground && Mathf.Abs(direction.x) > 0);
            if (Mathf.Abs(direction.x) > 0)
            {
                movementDirection = AttackDirection(Vector2.right, direction);
                FlipSprite(direction.x);
            }
            else
            {
                if (direction.y > 0 && direction.x == 0)
                {
                    movementDirection = AttackDirection(Vector2.right, Vector2.up);
                }
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
        }
        else if (rb.linearVelocity.y < 0 && !ground)
        {
            anim.SetBool("Jump", false);
            anim.SetBool("Fall", true);
        }

        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (FallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.linearVelocity.y > 0 && !Input.GetKey(KeyCode.Space))
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

















