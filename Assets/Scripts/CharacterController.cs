using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 direction;

    [Header("Stats")]
    public float MovementVelocity = 10f;
    public float JumpStrength = 5f;
    public float FallMultiplier = 2.5f;
    public float LowJumpMultiplier = 2f;

    [Header("Collisions")]
    public Vector2 down = Vector2.down;
    public float radioDetection = 0.2f;
    public LayerMask layerFloor;

    [Header("Booleans")]
    public bool canMove = true;
    public bool ground = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        CheckGround();
        if (canMove)
        {
            HandleJump();
        }
    }

    private void FixedUpdate()
    {
        if (canMove)
        {
            MoveCharacter();
            ApplyBetterJumpPhysics();
        }
    }

    private void MoveCharacter()
    {
        float x = Input.GetAxis("Horizontal");
        direction = new Vector2(x, 0);

        rb.linearVelocity = new Vector2(direction.x * MovementVelocity, rb.linearVelocity.y);

        // Voltear personaje si cambia de dirección
        if (direction.x != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(direction.x) * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    private void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && ground)
        {
            Jump();
        }
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, JumpStrength);
    }

    private void ApplyBetterJumpPhysics()
    {
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
        ground = Physics2D.OverlapCircle((Vector2)transform.position + down, radioDetection, layerFloor);
    }

    // Método especial para dibujar en la vista de escena (no en el juego)
    private void OnDrawGizmos()
    {
        Gizmos.color = ground ? Color.green : Color.red;
        Gizmos.DrawWireSphere((Vector2)transform.position + down, radioDetection);
    }
}

