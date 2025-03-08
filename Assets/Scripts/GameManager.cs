using UnityEngine;

public class GameManager: MonoBehaviour
{
    public GameObject lifesUI;
    public static GameManager instance;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
