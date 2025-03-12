using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPC_s : MonoBehaviour
{
    private Animator animator;
    private int actualPosition = 0; // Índice del waypoint actual
    private bool isWalking = false; // Estado de movimiento

    [SerializeField] private bool isNPC2 = false; // Activar si es NPC_2 (usa waypoints)
    [SerializeField] private float idleTime = 3f; // Tiempo en Idle al llegar a un waypoint
    [SerializeField] private float movementSpeed = 2f; // Velocidad de movimiento
    [SerializeField] private List<Transform> waypoints = new List<Transform>(); // Lista de waypoints

    void Start()
    {
        animator = GetComponent<Animator>();

        // Ignorar colisiones entre NPCs (asumiendo que los NPCs están en la capa "NPC")
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("NPC_Background"), LayerMask.NameToLayer("NPC_Background"));

        // Para NPC_2, iniciar la rutina de waypoints si aplica
        if (isNPC2)
        {
            StartCoroutine(WaypointRoutine());
        }
    }


    private IEnumerator WaypointRoutine()
    {
        while (true)
        {
            // **1️⃣ Cambia a Idle y espera en el waypoint**
            animator.SetBool("Walk", false);
            yield return new WaitForSeconds(idleTime);

            // **2️⃣ Cambia a Walk y se mueve al siguiente waypoint**
            animator.SetBool("Walk", true);
            yield return MoveToWaypoint();
        }
    }

    private IEnumerator MoveToWaypoint()
    {
        Transform targetPoint = waypoints[actualPosition];

        while (Vector2.Distance(transform.position, targetPoint.position) > 0.1f)
        {
            // Mueve al NPC hacia el waypoint
            transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, movementSpeed * Time.deltaTime);

            // **3️⃣ Rota el sprite según la dirección del movimiento**
            FlipSprite(targetPoint.position);

            yield return null;
        }

        // **4️⃣ Llega al waypoint, cambia de waypoint**
        actualPosition++;
        if (actualPosition >= waypoints.Count)
        {
            actualPosition = 0;
        }
    }

    private void FlipSprite(Vector2 targetPosition)
    {
        // Compara la posición actual del NPC con la posición del waypoint
        if (targetPosition.x > transform.position.x)
        {
            // Mueve a la derecha: escala X positiva
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (targetPosition.x < transform.position.x)
        {
            // Mueve a la izquierda: escala X negativa
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }
}
