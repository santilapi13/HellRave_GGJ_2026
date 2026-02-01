using UnityEngine;

public class PowerUpIndicator : MonoBehaviour
{
    [SerializeField] private float deactivateAfterSeconds = 5f;

    void OnEnable()
    {
        Invoke("DeactivateIndicator", deactivateAfterSeconds);
    }

    void OnDisable()
    {
        CancelInvoke();
    }

    private void DeactivateIndicator()
    {
        gameObject.SetActive(false);
    }
}
