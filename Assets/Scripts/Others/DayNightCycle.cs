using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DayNightLight : MonoBehaviour
{
    public Light2D globalLight; // Referencia al Global Light 2D
    public Color nightColor = new Color(0x28 / 255f, 0x20 / 255f, 0x41 / 255f); // Color #282041
    public float nightIntensity = 1.3f;

    public GameObject lanternsParent; // Objeto padre que contiene las lámparas
    public GameObject playerLantern;  // GameObject de la lámpara del jugador
    private Light2D playerLight;      // Componente Light2D de la lámpara del jugador

    private Color dayColor;
    private float dayIntensity;
    private bool isNight = false;

    void Start()
    {
        if (globalLight == null)
        {
            Debug.LogError("No se ha asignado el Global Light 2D.");
            return;
        }

        dayColor = globalLight.color;
        dayIntensity = globalLight.intensity;

        ToggleLanterns(false); // Apagar lámparas al inicio

        if (playerLantern != null)
        {
            playerLantern.SetActive(false); // Apagar lámpara del jugador al inicio
            playerLight = playerLantern.GetComponent<Light2D>(); // Obtener Light2D
            if (playerLight != null) playerLight.enabled = false; // Asegurar que la luz empiece apagada
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N)) // Presiona "N" para alternar día/noche
        {
            isNight = !isNight;
            ApplyLightSettings();
        }
    }

    void ApplyLightSettings()
    {
        if (globalLight != null)
        {
            globalLight.color = isNight ? nightColor : dayColor;
            globalLight.intensity = isNight ? nightIntensity : dayIntensity;
        }

        ToggleLanterns(isNight); // Encender lámparas fijas

        // Activar/desactivar la lámpara del jugador
        if (playerLantern != null)
        {
            playerLantern.SetActive(isNight);
            if (playerLight != null) playerLight.enabled = isNight;
        }
    }

    void ToggleLanterns(bool state)
    {
        if (lanternsParent != null)
        {
            foreach (Transform lantern in lanternsParent.transform)
            {
                Light2D lanternLight = lantern.GetComponentInChildren<Light2D>();
                if (lanternLight != null)
                {
                    lanternLight.enabled = state; // Encender o apagar la luz
                }
            }
        }
    }
}




