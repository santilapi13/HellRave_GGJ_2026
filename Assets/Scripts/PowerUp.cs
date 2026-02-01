using UnityEngine;
using System.Collections.Generic;

public class PowerUp : MonoBehaviour
{
    [SerializeField] private GameObject light;
    [SerializeField] private CapsuleCollider2D collider;
    [SerializeField] private float enableAfterSeconds = 10f;

    void Start()
    {
        light.SetActive(false);
        Invoke("EnablePowerUp", enableAfterSeconds);
    }

    private void EnablePowerUp()
    {
        AudioManager.Instance.PlaySFX("power",false);
        light.SetActive(true);
        collider.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        AudioManager.Instance.PlaySFX("powerUp",false);
        UnityEngine.InputSystem.PlayerInput[] activePlayers = FindObjectsByType<UnityEngine.InputSystem.PlayerInput>(FindObjectsSortMode.None);

        foreach (var player in activePlayers)
        {
            if (player.gameObject == collision.gameObject)
                continue;

            Transform indicator = player.transform.Find("PowerUpIndicator");
            if (indicator != null)
            {
                indicator.gameObject.SetActive(true);
            }
        }

        light.SetActive(false);
        collider.enabled = false;
    }
}
