using UnityEngine;

public class Platform : MonoBehaviour
{

    private bool applyForce;
    private bool detectPlayer;
    private CharacterController player;

    public bool giveJump;
    public BoxCollider2D platformCollider;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            detectPlayer = true; 
            if (giveJump)
            {
                applyForce = true;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            detectPlayer = false;
        }
    }

    private void Update()
    {
        if (giveJump)
        {
            if (player.transform.position.y - 0.8f > transform.position.y)
            {
                platformCollider.isTrigger = false;
            }
            else
            {
                platformCollider.isTrigger = true;
            }
        }
    }

    private void FixedUpdate()
    {
        if (applyForce)
        {
            player.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            player.GetComponent<Rigidbody2D>().AddForce(Vector2.up * 15, ForceMode2D.Impulse);
            applyForce = false;
        }
    }
}
