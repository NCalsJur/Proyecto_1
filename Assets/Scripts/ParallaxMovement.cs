using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    private Vector3 lastCameraPosition;
    private Camera cam;

    public Vector2 parallaxMultiplier; // Controla la velocidad de desplazamiento en X e Y

    void Start()
    {
        cam = Camera.main;
        lastCameraPosition = cam.transform.position;
    }

    void LateUpdate()
    {
        Vector3 deltaMovement = cam.transform.position - lastCameraPosition;
        transform.position += new Vector3(deltaMovement.x * parallaxMultiplier.x, deltaMovement.y * parallaxMultiplier.y, 0);
        lastCameraPosition = cam.transform.position;
    }
}
