using UnityEngine;
using System.Collections.Generic;

public class PowerUp : MonoBehaviour
{
    [SerializeField] private GameObject light;
    [SerializeField] private CapsuleCollider2D collider;
    [SerializeField] private float enableAfterSeconds = 45f;

    void Start()
    {
        light.SetActive(false);
        Invoke("EnablePowerUp", enableAfterSeconds);
    }

    private void EnablePowerUp()
    {
        light.SetActive(true);
        collider.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

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
