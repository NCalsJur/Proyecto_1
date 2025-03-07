using UnityEngine;

public class AttackArea : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if(collision.name == "Bat")
            {
                collision.GetComponent<Bat>().GetDamage();
            }
            else if(collision.name == "Skeleton")
            {
                collision.GetComponent<Skeleton>().GetDamage();
            }
        }
    }
 
}
