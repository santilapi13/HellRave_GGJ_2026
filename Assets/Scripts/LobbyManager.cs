using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System;

public class LobbyManager : MonoBehaviour
{
   [Header("Configuración")]
    [SerializeField] private PlayerInputManager playerInputManager;
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private GameObject characterPrefab;
    [SerializeField] private string confirmActionName = "Act";

    [SerializeField] private Key joinKeyP1 = Key.Space; // Para KeyboardWASD
    [SerializeField] private Key joinKeyP2 = Key.Enter;
    
    [Header("UI")]
    [SerializeField] private List<Image> playerSlots;
    [SerializeField] private List<GameObject> players;
    [SerializeField] private Color colorEmpty = Color.gray;
    [SerializeField] private Color colorJoined = Color.red;
    [SerializeField] private Color colorReady = Color.green;

    private Dictionary<int, (InputAction action, Action<InputAction.CallbackContext> callback)> playerEvents 
        = new Dictionary<int, (InputAction action, Action<InputAction.CallbackContext> callback)>();

    private InputAction joinAction;

    private void Awake()
    {
        InitializeSlots();
    }


    private void Update()
    {
        if (Keyboard.current != null)
        {
            // 1. Detectar Jugador WASD
            if (Keyboard.current[joinKeyP1].wasPressedThisFrame)
            {
                AttemptJoin("KeyboardWASD", Keyboard.current);
            }

            // 2. Detectar Jugador Flechas
            if (Keyboard.current[joinKeyP2].wasPressedThisFrame)
            {
                AttemptJoin("KeyboardArrows", Keyboard.current);
            }
        }

        // 3. Detectar Gamepads (Cualquier mando conectado)
        foreach (var gamepad in Gamepad.all)
        {
            // Usamos buttonSouth (X en PS, A en Xbox) para unirse
            if (gamepad.buttonSouth.wasPressedThisFrame)
            {
                AttemptJoin("Gamepad", gamepad);
            }
        }
    }

    private string GetSchemeFromControl(InputAction action, InputControl control)
    {
        // Recorremos todos los bindings de esta acción para encontrar cuál coincide con el control presionado
        foreach (var binding in action.bindings)
        {
            if (InputControlPath.Matches(binding.effectivePath, control))
            {
                return binding.groups.Split(';')[1];
            }
        }

        return null;
    }

    private void AttemptJoin(string scheme, InputDevice device)
    {
        // Usamos tu Manager para ver si ya está ocupado
        if (!PlayerConfigurationManager.Instance.IsDeviceUsed(device, scheme))
        {
            var p = PlayerInput.Instantiate(
                characterPrefab,        // El prefab que arrastraste al inspector
                playerIndex: -1,        // -1 = Auto asignar ID
                controlScheme: scheme,  // El esquema (WASD o Arrows)
                splitScreenIndex: -1,   
                pairWithDevice: device  // El teclado
            );
        }
    }

    private void InitializeSlots()
    {
        foreach (var slot in playerSlots)
        {
            slot.color = colorEmpty;
        }
    }


    public void OnPlayerJoined(PlayerInput pi)
    {
        pi.SwitchCurrentActionMap("Player");
        // 1. Validar duplicados (Teclado compartido)
        if (PlayerConfigurationManager.Instance.IsDeviceUsed(pi.devices[0], pi.currentControlScheme))
        {
            Destroy(pi.gameObject);
            return;
        }

        // 2. Registrar en el Manager
        PlayerConfigurationManager.Instance.AddPlayer(pi);

        // 3. Suscribir la acción de "Listo"
        // Buscamos la acción "Act" dentro de este PlayerInput específico
        var readyAction = pi.actions.FindAction(confirmActionName);
    
        if (readyAction != null)
        {
            var config = PlayerConfigurationManager.Instance.GetPlayerConfigs()
                        .Find(p => p.PlayerIndex == pi.playerIndex);

            Action<InputAction.CallbackContext> myCallback = ctx => TogglePlayerReady(config, ctx);
            playerEvents.Add(pi.playerIndex, (readyAction, myCallback));

            readyAction.performed += myCallback;
        }
        
        players[pi.playerIndex].SetActive(true);
        UpdateSlotUI(pi.playerIndex, false);
    }


    private void TogglePlayerReady(PlayerConfigurationManager.PlayerData player,InputAction.CallbackContext ctx)
    {
        if (ctx.control.device != player.Device) return;
        player.IsReady = !player.IsReady;
        UpdateSlotUI(player.PlayerIndex, player.IsReady);
        Animator anim =  players[player.PlayerIndex].GetComponent<Animator>();
        anim.SetTrigger("ChangeState");
        CheckIfAllReady();
    }

    private void UpdateSlotUI(int index, bool isReady)
    {
        if (index < playerSlots.Count)
        {
            playerSlots[index].color = isReady ? colorReady : colorJoined;
        }
    }

    private void CheckIfAllReady()
    {
        var players = PlayerConfigurationManager.Instance.GetPlayerConfigs();
        if (players.Count >= 2 && players.All(p => p.IsReady))
        {
            Debug.Log("¡TODOS LISTOS! INICIANDO JUEGO...");
            StartGame();
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene("MergeScene");
    }

    private void OnDestroy(){
    // Recorremos todos los eventos registrados
    foreach (var entry in playerEvents)
    {
        var action = entry.Value.action;
        var callback = entry.Value.callback;

        // VERIFICAMOS SI LA ACCIÓN SIGUE VIVA (El InputSystem podría haberse destruido antes)
        if (action != null)
        {
            action.performed -= callback; // ¡Aquí ocurre la desuscripción mágica!
        }
    }

    // Limpiamos el diccionario
    playerEvents.Clear();
    }

}