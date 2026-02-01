using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class GameLevelInitializer : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private GameObject playerCubePrefab; // Arrastra tu prefab del cubo aquí
    [SerializeField] private Transform[] spawnPoints; // Puntos donde aparecerán (opcional)

    private void Start()
    {
        // 1. Recuperamos la lista de jugadores que se configuraron en el Lobby
        var playerConfigs = PlayerConfigurationManager.Instance.GetPlayerConfigs();

        // 2. Recorremos la lista e instanciamos a cada uno
        foreach (var config in playerConfigs)
        {
            SpawnPlayer(config);
        }
    }

    private void SpawnPlayer(PlayerConfigurationManager.PlayerData config)
    {
        // Calculamos la posición de spawn (si hay puntos definidos, si no en (0,0,0))
        Vector3 spawnPos = Vector3.zero;
        if (spawnPoints != null && spawnPoints.Length > config.PlayerIndex)
        {
            spawnPos = spawnPoints[config.PlayerIndex].position;
        }

        // 3. LA MAGIA: Instanciamos el prefab pasándole el DISPOSITIVO y el ESQUEMA guardados
        var playerInstance = PlayerInput.Instantiate(
            playerCubePrefab,
            playerIndex: config.PlayerIndex,
            controlScheme: config.ControlScheme, // Esto asegura que sea WASD o Arrows
            splitScreenIndex: -1,
            pairWithDevice: config.Device // Esto asegura que use el teclado correcto o el gamepad
        );

        // Movemos el jugador a su posición de inicio
        playerInstance.transform.position = spawnPos;

        // (Opcional) Si quieres pasarle el PlayerIndex al script de movimiento para algo:
        // playerInstance.GetComponent<PlayerMovement>().SetIndex(config.PlayerIndex);
    }
}
