using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PillarExit : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Referencias")]
    [SerializeField] private Image textImage; // La imagen hija (el texto)
    
    [Header("Colores")]
    [SerializeField] private Color hoverColor = Color.white;
    [SerializeField] private Color normalColor = Color.gray;

    private void Start()
    {
        if (textImage != null)
            textImage.color = normalColor;
    }

    // Cuando el mouse entra al área de la imagen del pilar
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (textImage != null)
            textImage.color = hoverColor;
    }

    // Cuando el mouse sale
    public void OnPointerExit(PointerEventData eventData)
    {
        if (textImage != null)
            textImage.color = normalColor;
    }

    // Cuando haces click
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Saliendo del juego...");
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
