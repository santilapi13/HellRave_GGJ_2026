using UnityEngine;
using UnityEngine.Rendering.Universal; // Necesario para controlar luces 2D

public class NPCMaskRandomizer : MonoBehaviour
{
    [System.Serializable]
    public struct MaskProfile
    {
        public string name;      // Solo para que te ordenes en el inspector
        public Sprite sprite;    // La imagen de la máscara
        public Color lightColor; // El color de luz que emitirá
    }

    [Header("Referencias")]
    public SpriteRenderer maskRenderer;
    public Light2D maskLight; // ¡Arrastra aquí el componente Light 2D!

    [Header("Colección de Máscaras y sus Luces")]
    public MaskProfile[] maskCollection; // Aquí configurarás tus parejas Sprite-Color

    [Header("Configuración")]
    [Range(0, 100)] public int chanceOfNoMask = 25;
    [Range(0.5f, 5f)] public float lightIntensity = 1.5f;

    void Start()
    {
        EquipRandomLightMask();
    }

    void EquipRandomLightMask()
    {
        if (maskRenderer == null || maskLight == null) return;


        // 2. Elegir un perfil aleatorio
        if (maskCollection.Length > 0)
        {
            MaskProfile selectedProfile = maskCollection[Random.Range(0, maskCollection.Length)];

            // Asignar Sprite
            maskRenderer.sprite = selectedProfile.sprite;

            // Asignar Luz
            maskLight.enabled = true;
            maskLight.color = selectedProfile.lightColor;
            maskLight.intensity = lightIntensity;

            // (Opcional) Si quieres que la máscara brille un poco visualmente también
            maskRenderer.color = Color.white;
        }
    }
}