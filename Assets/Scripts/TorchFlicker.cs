using UnityEngine;
using UnityEngine.Rendering.Universal; // Necesario para Light 2D

public class TorchFlicker : MonoBehaviour
{
    [Header("Referencias")]
    public Light2D fireLight;

    [Header("Configuración del Fuego")]
    [Tooltip("Qué tan rápido parpadea")]
    public float flickSpeed = 3.0f;

    [Tooltip("Intensidad Mínima y Máxima")]
    public float minIntensity = 0.8f;
    public float maxIntensity = 1.5f;

    [Header("Movimiento (Jitter)")]
    [Tooltip("Si activas esto, la luz se moverá un poquito, haciendo bailar las sombras")]
    public bool shakeLight = true;
    public float shakeAmount = 0.05f; // Mueve la luz 0.05 unidades

    // Semilla aleatoria para que no todas las antorchas parpadeen igual
    private float randomOffset;
    private Vector3 initialPos;

    void Start()
    {
        if (fireLight == null)
            fireLight = GetComponent<Light2D>();

        initialPos = transform.localPosition;

        // Generamos un número al azar único para esta antorcha.
        // Así, si pones 10 antorchas juntas, cada una tendrá su propio ritmo.
        randomOffset = Random.Range(0f, 100f);
    }

    void Update()
    {
        if (fireLight == null) return;

        // 1. CÁLCULO DE INTENSIDAD (Perlin Noise)
        // Usamos el tiempo + el offset. El ruido de Perlin devuelve valor entre 0 y 1.
        float noise = Mathf.PerlinNoise((Time.time * flickSpeed) + randomOffset, 0f);

        // Convertimos ese 0-1 al rango que queremos (ej: de 0.8 a 1.5)
        fireLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, noise);

        // 2. MOVIMIENTO (Opcional pero recomendado)
        if (shakeLight)
        {
            // Usamos otro Perlin Noise diferente para X e Y
            float xNoise = Mathf.PerlinNoise((Time.time * flickSpeed) + randomOffset, 10f) - 0.5f;
            float yNoise = Mathf.PerlinNoise((Time.time * flickSpeed) + randomOffset, 20f) - 0.5f;

            transform.localPosition = initialPos + new Vector3(xNoise, yNoise, 0) * shakeAmount;
        }
    }
}