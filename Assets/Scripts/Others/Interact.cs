using UnityEngine;

public class Interact : MonoBehaviour
{
    private GameObject interactIndicator;
    private SpriteRenderer sp;
    private BoxCollider2D bc;
    private Animator anim;

    public GameObject[] objects;

    public bool isChest;
    public bool canInteract;

    private void Awake()
    {
        bc = GetComponent<BoxCollider2D>();
        sp = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        if(transform.GetChild(0) != null)
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
            Instantiate(objects[Random.Range(0, objects.Length)],transform.position, Quaternion.identity);
            anim.SetBool("OpenChest", true);
            bc.enabled = false;
        }
    }

    private void Update()
    {
        if (canInteract && Input.GetKeyDown(KeyCode.F))
        {
            Chest();
        }
    }
}
