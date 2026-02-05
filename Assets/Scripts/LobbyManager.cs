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
    [SerializeField] private List<Image> controls;
    [SerializeField] private List<Sprite> sprites;
    [SerializeField] private Color colorEmpty = Color.gray;
    [SerializeField] private Color colorJoined = Color.red;
    [SerializeField] private Color colorReady = Color.green;

    [SerializeField] private TMPro.TextMeshProUGUI winnerText;
    [SerializeField] private TMPro.TextMeshProUGUI winnerTextShadow;

    [Header("Countdown")]
    [SerializeField] private float countdownTime = 3f;
    private Coroutine countdownCoroutine;

    private Dictionary<int, (InputAction action, Action<InputAction.CallbackContext> callback)> playerEvents 
        = new Dictionary<int, (InputAction action, Action<InputAction.CallbackContext> callback)>();

    private InputAction joinAction;

    private void Awake()
    {
        InitializeSlots();
    }

    private void Start()
    {
        AudioManager.Instance?.StopMusic();
        AudioManager.Instance?.PlayMusic("Lobby");
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

    private void RestoreExistingPlayers(){
    var existingConfigs = PlayerConfigurationManager.Instance.GetPlayerConfigs();

    foreach (var config in existingConfigs)
    {
        // Forzamos que IsReady sea false al volver al lobby para que deban confirmar de nuevo
        config.IsReady = false;

        // Reconectamos el PlayerInput a la lógica del Lobby
        SetupPlayerInLobby(config.PlayerInput);
    }
    }


    public void OnPlayerJoined(PlayerInput pi)
    {
        AudioManager.Instance?.PlaySFX("join",false);
        // 2. Registrar en el Manager
        PlayerConfigurationManager.Instance.AddPlayer(pi);
        SetupPlayerInLobby(pi);
        
    }

    private void SetupPlayerInLobby(PlayerInput pi)
    {
        pi.SwitchCurrentActionMap("Player");

        var readyAction = pi.actions.FindAction(confirmActionName);
        if (readyAction != null)
        {
            var config = PlayerConfigurationManager.Instance.GetPlayerConfigs()
                            .Find(p => p.PlayerIndex == pi.playerIndex);
            config.IsReady = false;

            // --- SOLUCIÓN AL ERROR DE KEY DUPLICADA ---
            // Si por alguna razón el índice ya está, desuscribimos lo viejo y removemos
            if (playerEvents.ContainsKey(pi.playerIndex))
            {
                playerEvents[pi.playerIndex].action.performed -= playerEvents[pi.playerIndex].callback;
                playerEvents.Remove(pi.playerIndex);
            }

            Action<InputAction.CallbackContext> myCallback = ctx => TogglePlayerReady(config, ctx);
            
            // Ahora es seguro agregar
            playerEvents.Add(pi.playerIndex, (readyAction, myCallback));
            readyAction.performed += myCallback;
        }

        if (pi.playerIndex < players.Count)
        {
            switch (pi.currentControlScheme)
            {
                case "KeyboardWASD":
                    controls[pi.playerIndex].sprite = sprites[0]; // Asume que el 0 es WASD
                    break;

                case "KeyboardArrows":
                    controls[pi.playerIndex].sprite = sprites[1]; // Asume que el 1 es Flechas
                    break;

                case "Gamepad":
                    controls[pi.playerIndex].sprite = sprites[2]; // Asume que el 2 es Mando
                    break;

                default:
                    controls[pi.playerIndex].sprite = sprites[2]; // Asume que el 2 es Mando
                    break;
            }
           
            players[pi.playerIndex].SetActive(true);
            UpdateSlotUI(pi.playerIndex, false);
        }
    }

    private void TogglePlayerReady(PlayerConfigurationManager.PlayerData player,InputAction.CallbackContext ctx)
    {
        if (ctx.control.device != player.Device) return;
        player.IsReady = !player.IsReady;
        UpdateSlotUI(player.PlayerIndex, player.IsReady);

        if (!player.IsReady && countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
            countdownCoroutine = null;
            winnerText.text = ""; // Limpia el texto
            winnerTextShadow.text = "";
        }
        Animator anim =  players[player.PlayerIndex].GetComponent<Animator>();
        anim.SetTrigger("ChangeState");
        CheckIfAllReady();
    }

    private void UpdateSlotUI(int index, bool isReady)
    {
        if (index < playerSlots.Count)
        {
            playerSlots[index].color = isReady ? colorReady : colorJoined;
            if(isReady) AudioManager.Instance?.PlaySFX("risa",false);
        }
    }

    private void CheckIfAllReady()
    {
        var configs = PlayerConfigurationManager.Instance.GetPlayerConfigs();
        // Si todos están listos (mínimo 2) y no hay una cuenta atrás ya corriendo
        if (configs.Count >= 2 && configs.All(p => p.IsReady))
        {
            if (countdownCoroutine == null)
            {
                countdownCoroutine = StartCoroutine(StartCountdownRoutine());
            }
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene("MergeScene");
        AudioManager.Instance?.StopMusic();
        AudioManager.Instance?.PlayMusic("Game");
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

    private System.Collections.IEnumerator StartCountdownRoutine()
    {
        float timer = countdownTime;
        while (timer > 0)
        {
            // Actualiza el texto (puedes usar winnerText si no tienes otro)
            AudioManager.Instance?.PlaySFX("countdown",false);
            winnerText.text = Mathf.Ceil(timer).ToString();
            winnerTextShadow.text = winnerText.text;
            
            yield return new WaitForSeconds(1f);
            timer--;
        }
        AudioManager.Instance?.PlaySFX("risa_random",false);
        StartGame();
    }

}