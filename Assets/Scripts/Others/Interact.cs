using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.Events;

public class Interact : MonoBehaviour
{
    private GameObject interactIndicator;
    private SpriteRenderer sp;
    private BoxCollider2D bc;
    private Animator anim;
    public UnityEvent evento;

    public GameObject[] objects;
    public bool isChest;
    public bool isCheckPoint;
    public bool canInteract;

    private void Awake()
    {
        bc = GetComponent<BoxCollider2D>();
        sp = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        if (transform.GetChild(0) != null)
        {
            interactIndicator = transform.GetChild(0).gameObject;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canInteract = true;
            interactIndicator.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canInteract = false;
            interactIndicator.SetActive(false);
        }
    }

    private void Chest()
    {
        if (isChest)
        {
            GameObject item = Instantiate(objects[Random.Range(0, objects.Length)], transform.position + Vector3.up * 1.5f, Quaternion.identity);
            anim.SetBool("OpenChest", true);
            bc.enabled = false;
        }
    }

    private void CheckPoint()
    {
        if (isCheckPoint)
        {
            evento.Invoke();
        }
    }

    private void Update()
    {
        if (canInteract && Input.GetKeyDown(KeyCode.F))
        {
            Chest();
            CheckPoint();
        }
    }
}
