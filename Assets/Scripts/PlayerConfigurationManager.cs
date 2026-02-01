using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerConfigurationManager : MonoBehaviour
{
    public static PlayerConfigurationManager Instance { get; private set; }

    // Clase simple para guardar la data de cada jugador
    public class PlayerData
    {
        public int PlayerIndex;
        public PlayerInput PlayerInput;
        public InputDevice Device;
        public string ControlScheme;
        public bool IsReady;            
    }

    private List<PlayerData> playerConfigs = new List<PlayerData>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool IsDeviceUsed(InputDevice device, string scheme)
    {
        // Si es teclado, verificamos también el esquema (para separar WASD de Flechas)
        if (device is Keyboard)
        {
            return playerConfigs.Any(p => p.Device == device && p.ControlScheme == scheme);
        }
        // Si es gamepad, solo verificamos el dispositivo físico
        return playerConfigs.Any(p => p.Device == device);
    }

    public void AddPlayer(PlayerInput pi)
    {

        // Si ya lo tenemos, no lo agregamos a la lista, pero nos aseguramos que sea hijo
        if (playerConfigs.Any(p => p.PlayerIndex == pi.playerIndex))
        {
            pi.transform.SetParent(this.transform);
            return;
        }

        // Hacerlo persistente
        pi.transform.SetParent(this.transform);

        PlayerData newPlayer = new PlayerData
        {
            PlayerIndex = pi.playerIndex,
            PlayerInput = pi,
            Device = pi.devices[0],
            ControlScheme = pi.currentControlScheme,
            IsReady = false
        };

        playerConfigs.Add(newPlayer);
    
    }

    public List<PlayerData> GetPlayerConfigs()
    {
        return playerConfigs;
    }

}
