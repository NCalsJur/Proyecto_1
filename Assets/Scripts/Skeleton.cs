using System.Collections;
using Unity.Cinemachine;
using Unity.Hierarchy;
using Unity.VisualScripting;
using UnityEngine;

public class Skeleton : MonoBehaviour
{
    private CharacterController characterController;
    private Rigidbody2D rb;
    private SpriteRenderer sp;
    private Animator anim;
    private CinemachineCamera cm;
    private bool applyForce;


    public float playerDetection = 15;
    public float arrowDetection = 10;
    public float arrowStrength;
    public float skeletonSpeed;
    public int skeletonLife = 3;
    public bool shootingArrow;
    public GameObject arrow;


    private void Awake()
    {
        characterController = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody2D>();
        sp = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        cm = GetComponent<CinemachineCamera>();
    }

    void Start()
    {
        gameObject.name = "Skeleton";
    }

    
    void Update()
    {
        Vector2 direction = (characterController.transform.position - transform.position).normalized * arrowDetection;
        Debug.DrawRay(transform.position, direction, Color.red);

        float actualDistance = Vector2.Distance(transform.position, characterController.transform.position);

        if (actualDistance <= arrowDetection)
        {
            rb.linearVelocity = Vector2.zero;
            anim.SetBool("Walk", false);

            Vector2 normalizedDirection = direction.normalized;
            ChangeView(normalizedDirection.x);

            if (!shootingArrow)
            {
                StartCoroutine(ShootingArrow(direction, actualDistance));
            }
            else
            {
                if(actualDistance <= arrowDetection)
                {
                    Vector2 movement = new Vector2(direction.x, 0);
                    movement = movement.normalized;

                    rb.linearVelocity = movement * skeletonSpeed;
                    anim.SetBool("Walk", true);
                    ChangeView(movement.x);

                }
                else
                {
                    anim.SetBool("Walk", false);
                }
            }
        }
    }

    private void ChangeView(float directionX)
    {
        if(directionX < 0 && transform.localScale.x > 0)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        else if (directionX > 0 && transform.localScale.x <0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, playerDetection);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, arrowDetection);
    }

    private IEnumerator ShootingArrow(Vector2 arrowDirection, float distance)
    {
        shootingArrow = true;
        anim.SetBool("Shoot", true);
        yield return new WaitForSeconds(1.42f);
        anim.SetBool("Shoot", false);
        arrowDirection = arrowDirection.normalized;

        GameObject arrowGo = Instantiate(arrow, transform.position, Quaternion.identity);
        //arrowGo.transform.GetComponent<Arrow>().arrowDirection = arrowDirection;
        //arrowGo.transform.GetComponent<Arrow>().skeleton = this.gameObject;

        //arrowGo.transform.GetComponent<Rigidbody2D>().linearVelocity = arrowDirection * arrowStrength;
        shootingArrow = false;
    }

    public void GetDamage()
    {
        if(skeletonLife > 0)
        {
            StartCoroutine(DamageEffect());
            StartCoroutine(ShakeCamera(0.1f));
            applyForce = true;
            skeletonLife--;
        }
        else
        {
            StartCoroutine(ShakeCamera(0.1f));
            skeletonSpeed = 0;
            rb.linearVelocity = Vector2.zero;
            Destroy(this.gameObject, 0.2f);
        }
    }
    private IEnumerator ShakeCamera(float time)
    {
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = cm.GetComponent<CinemachineBasicMultiChannelPerlin>();
        cinemachineBasicMultiChannelPerlin.AmplitudeGain = 5;
        yield return new WaitForSeconds(time);
        cinemachineBasicMultiChannelPerlin.AmplitudeGain = 0;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //player.GetDamage((transform.position - player.transform.position).normalized);
        }
    }

    private IEnumerator DamageEffect()
    {
        sp.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        sp.color = Color.white;
    }

    private void FixedUpdate()
    {
        if (applyForce)
        {
            rb.AddForce((transform.position - characterController.transform.position).normalized * 100, ForceMode2D.Impulse);
            applyForce = false;
        }
    }
}
