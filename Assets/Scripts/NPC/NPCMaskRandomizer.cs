using UnityEngine;

public class NPCMaskRandomizer : MonoBehaviour
{
    [Header("Referencias")]
    public SpriteRenderer maskRenderer;

    [Header("Colección de Máscaras")]
    [Tooltip("Tus sprites originales a color")]
    public Sprite[] maskOptions;


    [Space(10)]
    [Tooltip("Intensidad del brillo. 1 es normal. Más de 1 es NEON.")]
    [Range(0f, 10f)]
    public float neonIntensity = 0.2f; // Un valor entre 3 y 5 suele verse bien

    void Start()
    {
        EquipRandomNeonMask();
    }

    void EquipRandomNeonMask()
    {
        if (maskRenderer == null) return;


        // 2. Asignar Sprite Aleatorio
        if (maskOptions.Length > 0)
        {
            maskRenderer.sprite = maskOptions[Random.Range(0, maskOptions.Length)];
        }


        Color hdrGlow = new Color(neonIntensity, neonIntensity, neonIntensity, 1f);

        maskRenderer.color = hdrGlow;
    }
}