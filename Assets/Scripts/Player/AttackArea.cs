using UnityEngine;

public class AttackArea : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if(collision.name == "Bat")
            {
                collision.GetComponent<BatWaypoints>().GetDamage();
            }
            else if(collision.name == "Skeleton")
            {
                collision.GetComponent<Skeleton>().GetDamage();
            }
            else if (collision.name == "Spider")
            {
                collision.GetComponent<SpiderWaypoints>().GetDamage();
            }
            else if (collision.name == "Boos_1")
            {
                collision.GetComponent<Boss>().GetDamage();
            }
        }
    }
}