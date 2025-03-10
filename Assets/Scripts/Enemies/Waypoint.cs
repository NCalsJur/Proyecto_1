using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Waypoint : MonoBehaviour
{

    private Vector3 direction;
    private CharacterController player;
    private Rigidbody2D rb;
    private SpriteRenderer sp;
    private int actualPosition = 0;
    private bool applyForce;

    public Vector2 headPosition;
    public int life = 3;
    public float movementSpeed;
    public List<Transform> points = new List<Transform>();

    private void Awake()
    {
        sp = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>();
    }

    private void Start()
    {
        if (gameObject.CompareTag("Enemy"))
        {
            gameObject.name = ("Spider");
        }
    }

    private void FixedUpdate()
    {
        MovementWaypoints();

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
                player.GetComponent<Rigidbody2D>().linearVelocity = Vector2.up *player.JumpStrenght;
                Destroy(this.gameObject, 0.2f);
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
        direction = (points[actualPosition].position  - transform.position).normalized;
        transform.position = (Vector2.MoveTowards(transform.position, points[actualPosition].position, movementSpeed * Time.deltaTime));
        if (Vector2.Distance(transform.position, points[actualPosition].position) <= 0.7f)
        {
            StartCoroutine(Wait());
        }
    }

    private IEnumerator Wait()
    {
        yield return null;
        actualPosition++;

        if (actualPosition >= points.Count)
        {
            actualPosition = 0;
        }
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
            movementSpeed = 0;
            rb.linearVelocity = Vector2.zero;
            Destroy(gameObject, 0.2f);
        }
    }

    private IEnumerator DamageEffect()
    {
        sp.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        sp.color = Color.white;
    }

}
