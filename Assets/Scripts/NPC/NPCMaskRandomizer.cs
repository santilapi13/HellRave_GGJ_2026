using UnityEngine;
using UnityEngine.Rendering.Universal; // Necesario para Light 2D

public class NPCMaskRandomizer : MonoBehaviour
{
    [System.Serializable]
    public struct MaskProfile
    {
        public string name;
        public Sprite sprite;
        public Color lightColor;
    }

    [Header("Referencias")]
    public SpriteRenderer maskRenderer;
    public Light2D maskLight;

    [Header("Colecci�n")]
    public MaskProfile[] maskCollection;

    [Header("Probabilidad")]
    [Range(0, 100)] public int chanceOfNoMask = 25;

    [Header("Configuraci�n de Luz")]
    [Tooltip("Intensidad/Brillo")]
    [Range(0.5f, 500f)] public float lightIntensity = 500.0f;

    [Tooltip("Radio de la luz (Funciona en ambos modos)")]
    [Range(0.5f, 10f)] public float lightRadius = 2.0f;

    [Header("Modo Difuminado")]
    [Tooltip("TRUE: Luz redonda (Point). FALSE: Luz con forma de m�scara (Sprite).")]
    public bool useSoftFalloff = false;

    [Tooltip("Suavizado de bordes. En modo Sprite afecta la transparencia del borde.")]
    [Range(0f, 1f)] public float blurAmount = 0.5f;

    void Start()
    {
        ConfigureAndEquip();
    }

    void ConfigureAndEquip()
    {
        if (maskRenderer == null || maskLight == null) return;



        // 2. Elegir perfil
        if (maskCollection.Length > 0)
        {
            MaskProfile profile = maskCollection[Random.Range(0, maskCollection.Length)];

            // --- VISUAL (Sprite Renderer) ---
            maskRenderer.sprite = profile.sprite;
            maskRenderer.color = Color.white;

            // --- LUZ (Light 2D) ---
            maskLight.enabled = true;

            Color finalColor = profile.lightColor;
            finalColor.a = 1f;
            maskLight.color = finalColor;
            maskLight.intensity = lightIntensity;

            // --- APLICAR RADIO (Com�n a ambos) ---
            // Esto le dice a Unity hasta d�nde llega la luz f�sicamente
            maskLight.pointLightOuterRadius = lightRadius;

            if (useSoftFalloff)
            {
                // MODO POINT (C�rculo perfecto)
                maskLight.lightType = Light2D.LightType.Point;
                maskLight.lightCookieSprite = null;

                // En modo Point, el "blur" controla el radio interior
                maskLight.pointLightInnerRadius = lightRadius * (1f - blurAmount);
            }
            else
            {
                // MODO SPRITE (Forma de la m�scara)
                maskLight.lightType = Light2D.LightType.Sprite;
                maskLight.lightCookieSprite = profile.sprite;

                // IMPORTANTE: En modo Sprite, a veces Unity necesita que escalemos el objeto
                // para que coincida visualmente con el radio deseado.
                // Esta l�nea sincroniza el tama�o visual con el radio num�rico.
                maskLight.transform.localScale = Vector3.one * lightRadius;

                // En modo Sprite, usamos el "blur" para controlar la opacidad del borde (Falloff)
                // 0 = Borde duro, 1 = Muy transparente en los bordes
                maskLight.falloffIntensity = blurAmount;
            }

        }
    }
}