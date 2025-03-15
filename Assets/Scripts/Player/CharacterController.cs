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
    private SpriteRenderer sp;

    private Vector2 direction;
    private Vector2 movementDirection;
    private Vector2 lastDirection;
    private Vector2 damgeDirection;
    private Vector2 lastSafePosition; // Guarda la última posición segura del jugador
    private float originalGravity;

    [Header("Stats")]
    public float MovementVelocity = 10;
    public float JumpStrenght = 15;
    public float dashVelocity = 10;
    public int lifes = 3;
    public float inmortalityTime;

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
    public bool isInmortal;
    public bool applyForce;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sp = GetComponent<SpriteRenderer>();

        trail = transform.Find("Trail").GetComponent<TrailRenderer>();
        if (trail != null) trail.enabled = false;
        originalGravity = rb.gravityScale;
        lastDirection = Vector2.right; // Dirección inicial por defecto

        // Ocultar el cursor del ratón
        Cursor.visible = false;
    }

    public void Dead()
    {
        if (lifes > 0)
        {
            return;
        }

        anim.SetBool("IsDeath", true); // Activar animación de muerte

        // Cambiar el personaje al layer de los NPCs
        gameObject.layer = LayerMask.NameToLayer("NPC_Background");
        GameManager.instance.GameOver();
        this.enabled = false; // Deshabilitar el script después de morir
    }




    public void GetDamage()
    {
        StartCoroutine(DamageImpact(Vector2.zero));
    }

    public void GetDamage(Vector2 damageDirection)
    {
        StartCoroutine(DamageImpact(damageDirection));
    }

    private IEnumerator DamageImpact(Vector2 damageDirection)
    {
        if (!isInmortal)
        {
            StartCoroutine(Inmortality());
            lifes--;
            float auxVelocity = MovementVelocity;
            this.damgeDirection = damageDirection;
            applyForce = true;
            Time.timeScale = 0.4f;
            yield return new WaitForSeconds(0.2f);
            Time.timeScale = 1;

            UpdateUILifes(1);
            MovementVelocity = auxVelocity;
            Dead();
        }
    }

    public void UpdateUILifes(int descountLifes)
    {
        int lifesDescount = descountLifes;

        for (int i = GameManager.instance.lifesUI.transform.childCount - 1; i >= 0; i--)
        {
            if (GameManager.instance.lifesUI.transform.GetChild(i).gameObject.activeInHierarchy && lifesDescount != 0)
            {
                GameManager.instance.lifesUI.transform.GetChild(i).gameObject.SetActive(false);
                lifesDescount--;
            }
            else
            {
                if (descountLifes == 0)
                {
                    break;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (applyForce)
        {
            MovementVelocity = 0;
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(-damgeDirection * 15, ForceMode2D.Impulse);
            applyForce = false;
        }
    }

    public void GiveInmortality()
    {
        StartCoroutine(Inmortality());
    }

    private IEnumerator Inmortality()
    {
        isInmortal = true;
        float timePass = 0;

        while (timePass < inmortalityTime)
        {
            sp.color = new Color(1, 1, 1, 0.5f);
            yield return new WaitForSeconds(inmortalityTime/20);
            sp.color = new Color(1, 1, 1, 1);
            yield return new WaitForSeconds(inmortalityTime / 20);
            timePass += inmortalityTime / 10;
        }

        isInmortal = false;
    }

    private void Update()
    {
        Movement();
        CheckGround();
        HandleAttack();

        if (ground)
        {
            lastSafePosition = transform.position;
        }
    }

    public void Respawn()
    {
        transform.position = lastSafePosition;
    }

    private void HandleAttack()
    {
        if (Input.GetMouseButtonDown(0)) // Click izquierdo
        {
            if (!attacking && !dash)
            {
                attacking = true;
                anim.SetFloat("Attack_X", lastDirection.x);
                anim.SetFloat("Attack_Y", lastDirection.y);
                anim.SetBool("Attack", true);
            }
        }
    }

    public void EndAttack()
    {
        anim.SetBool("Attack", false);
        attacking = false;
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

        direction = new Vector2(x, yRaw);
        if (direction.magnitude > 0) lastDirection = direction.normalized;

        Walk();
        BetterJump();

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
            rb.linearVelocity = new Vector2(direction.x * MovementVelocity, rb.linearVelocity.y);

            anim.SetBool("Walk", ground && Mathf.Abs(direction.x) > 0);
            if (Mathf.Abs(direction.x) > 0)
            {
                FlipSprite(direction.x);
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
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere((Vector2)transform.position + down, radioDetection);
    }
}