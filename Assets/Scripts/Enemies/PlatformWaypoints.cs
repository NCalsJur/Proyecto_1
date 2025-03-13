using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    private int actualPosition = 0;
    private bool isMoving = true;
    private bool movingForward = true; // Indica si la plataforma va hacia adelante o hacia atrás

    public float movementSpeed = 2f;  // Velocidad de la plataforma
    public float waitTime = 1f;       // Tiempo de espera en cada punto
    public List<Transform> waypoints = new List<Transform>(); // Lista de puntos de movimiento

    private void FixedUpdate()
    {
        if (isMoving)
        {
            MoveBetweenWaypoints();
        }
    }

    private void MoveBetweenWaypoints()
    {
        if (waypoints.Count < 2) return; // No mover si hay menos de 2 puntos

        Vector3 direction = (waypoints[actualPosition].position - transform.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, waypoints[actualPosition].position, movementSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, waypoints[actualPosition].position) <= 0.1f)
        {
            StartCoroutine(WaitAtWaypoint());
        }
    }

    private IEnumerator WaitAtWaypoint()
    {
        isMoving = false;
        yield return new WaitForSeconds(waitTime);

        if (movingForward)
        {
            actualPosition++;
            if (actualPosition >= waypoints.Count - 1)
            {
                movingForward = false;
            }
        }
        else
        {
            actualPosition--;
            if (actualPosition <= 0)
            {
                movingForward = true;
            }
        }

        isMoving = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform); // Hace que el jugador se mueva con la plataforma
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null); // Separa al jugador cuando salga de la plataforma
        }
    }
}