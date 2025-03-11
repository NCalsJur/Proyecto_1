using UnityEngine;

public class Traps : MonoBehaviour
{

    private CharacterController player;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<CharacterController>().GetDamage(-(collision.transform.position - transform.position).normalized);
        }
    }
}
