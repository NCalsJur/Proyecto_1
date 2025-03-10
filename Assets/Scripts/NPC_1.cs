using UnityEngine;

public class NPC_1 : MonoBehaviour
{
    private Animator animator;
    private bool isWorking = false; // Estado actual de la animación

    [SerializeField] private float idleTime = 3f; // Tiempo en Idle
    [SerializeField] private float workTime = 5f; // Tiempo en Work

    void Start()
    {
        animator = GetComponent<Animator>();
        StartCoroutine(SwitchAnimation());
    }

    private System.Collections.IEnumerator SwitchAnimation()
    {
        while (true)
        {
            yield return new WaitForSeconds(isWorking ? workTime : idleTime);
            isWorking = !isWorking;
            animator.SetBool("Work", isWorking);
        }
    }
}

