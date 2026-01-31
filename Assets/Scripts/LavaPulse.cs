using UnityEngine;
// Necesario para acceder a las luces 2D de URP
using UnityEngine.Rendering.Universal;

public class LavaPulse : MonoBehaviour
{
    private Light2D _lavaLight;

    [Header("Configuración del Pulso")]
    public float minIntensity = 1.0f;
    public float maxIntensity = 1.5f;
    public float pulseSpeed = 2.0f;

    [Header("Variación de Radio (Opcional)")]
    public bool pulseRadius = true;
    public float minRadius = 5f;
    public float maxRadius = 5.5f;

    // Semilla aleatoria para que no todas las lavas pulsen igual
    private float _randomOffset;

    void Start()
    {
        _lavaLight = GetComponent<Light2D>();
        _randomOffset = Random.Range(0f, 100f);
    }

    void Update()
    {
        if (_lavaLight == null) return;

        // Usamos Mathf.Sin para crear una onda suave que sube y baja
        float time = Time.time * pulseSpeed + _randomOffset;
        float sineValue = (Mathf.Sin(time) + 1f) / 2f; // Normalizamos entre 0 y 1

        // Interpolamos la intensidad
        _lavaLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, sineValue);

        // Interpolamos el radio (Outer Radius) si está activado
        if (pulseRadius)
        {
            _lavaLight.pointLightOuterRadius = Mathf.Lerp(minRadius, maxRadius, sineValue);
        }
    }
}