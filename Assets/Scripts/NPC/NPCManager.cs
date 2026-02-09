using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
using UnityEngine.UI;

public class NPCManager : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform npcContainer;
    [SerializeField] private Transform[] characters;
    [SerializeField] private GameObject playerCubePrefab;
    [SerializeField] private GameObject npcPrefab;
    
    [Header("Behavior Configuration")]
    [SerializeField] private int npcCount = 60;

    private GridManager gridManager;
    private List<Vector2Int> walkableTiles;
    private List<Vector2Int> spawnTiles;
    private int currentSpawnIndex = 0;

    [SerializeField] private List<Image> playerUI;

    private Color[] hellPalette = new Color[]
    {
        new Color32(196, 30, 58, 255),   // #C41E3A (Rojo Sangre)
        new Color32(255, 215, 0, 255),   // #FFD700 (Amarillo Azufre)
        new Color32(112, 128, 144, 255), // #708090 (Gris Ceniza)
        new Color32(128, 0, 128, 255)    // #800080 (Morado Magia)
    };

    void Start()
    {
        gridManager = FindFirstObjectByType<GridManager>();
        if (gridManager == null)
        {
            return;
        }

        CollectWalkableTiles();
        FilterSpawnTiles();
        InstantiatePlayers();
        InitializeCharacters();
        initMap();
        if (GameManager.Instance != null)
            GameManager.Instance.GetPlayers();
    }

    private void CollectWalkableTiles()
    {
        walkableTiles = new List<Vector2Int>();

        for (int x = 0; x < gridManager.width; x++)
        {
            for (int y = 0; y < gridManager.height; y++)
            {
                Vector2Int gridPos = new Vector2Int(x, y);
                if (gridManager.IsWalkable(gridPos))
                {
                    walkableTiles.Add(gridPos);
                }
            }
        }

        // Randomizar la lista de tiles caminables
        for (int i = walkableTiles.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            Vector2Int temp = walkableTiles[i];
            walkableTiles[i] = walkableTiles[randomIndex];
            walkableTiles[randomIndex] = temp;
        }
    }

    private void FilterSpawnTiles()
    {
        spawnTiles = new List<Vector2Int>();

        // Solo incluir tiles que tengan al menos 4 vecinos caminables
        // Esto evita spawns en bordes o zonas con poco espacio
        foreach (Vector2Int tile in walkableTiles)
        {
            int walkableNeighbors = 0;
            
            // Verificar los 8 vecinos adyacentes
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0) continue; // Saltar el centro
                    
                    Vector2Int neighbor = new Vector2Int(tile.x + dx, tile.y + dy);
                    if (gridManager.IsWalkable(neighbor))
                    {
                        walkableNeighbors++;
                    }
                }
            }
            
            // Si tiene al menos 4 vecinos caminables, es un buen lugar para spawn
            if (walkableNeighbors >= 4)
            {
                spawnTiles.Add(tile);
            }
        }

        // Randomizar la lista de spawn tiles
        for (int i = spawnTiles.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            Vector2Int temp = spawnTiles[i];
            spawnTiles[i] = spawnTiles[randomIndex];
            spawnTiles[randomIndex] = temp;
        }

        if (spawnTiles.Count == 0)
        {
            Debug.LogWarning("No se encontraron tiles de spawn válidos. Usando todos los tiles caminables.");
            spawnTiles = new List<Vector2Int>(walkableTiles);
        }
    }

    private void InitializeCharacters()
    {
        if (npcContainer == null || npcPrefab == null)
        {
            return;
        }

        characters = new Transform[npcCount];

        for (int i = 0; i < npcCount; i++)
        {
            GameObject npcInstance = Instantiate(npcPrefab, npcContainer);
            npcInstance.SetActive(false); // Inicialmente desactivado
            characters[i] = npcInstance.transform;
        }

        // Randomizar el array de personajes
        for (int i = characters.Length - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            Transform temp = characters[i];
            characters[i] = characters[randomIndex];
            characters[randomIndex] = temp;
        }
    }



    private void initMap()
    {
        if (characters == null || characters.Length == 0)
        {
            return;
        }

        if (spawnTiles == null || spawnTiles.Count == 0)
        {
            return;
        }

        // Continuar desde donde quedó el índice de spawn de los jugadores
        int tileIndex = currentSpawnIndex;

        for (int i = 0; i < characters.Length; i++)
        {
            if (tileIndex >= spawnTiles.Count)
            {
                Debug.LogWarning($"Not enough spawn tiles for all characters. Placed {i} out of {characters.Length}.");
                break;
            }

            Transform character = characters[i];

            // Obtener un tile aleatorio de la lista ya randomizada
            Vector2Int gridPos = spawnTiles[tileIndex];
            tileIndex++;

            // Convertir la posición de grid a posición del mundo
            Vector3 worldPosition = gridManager.GridToWorld(gridPos);
            worldPosition.z = 0f;

            character.position = worldPosition;

            SpriteRenderer sr = characters[i].GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.color = hellPalette[Random.Range(0, hellPalette.Length)];
            }

            character.gameObject.SetActive(true);
        }
    }

    private void InstantiatePlayers()
    {
        if (!PlayerConfigurationManager.Instance)
        {
            return;
        }
        var playerConfigs = PlayerConfigurationManager.Instance.GetPlayerConfigs();
        foreach (var config in playerConfigs)
        {
            SpawnPlayer(config);
        }
    }

     private void SpawnPlayer(PlayerConfigurationManager.PlayerData config)
    {
        // Obtener una posición de spawn única usando el índice
        Vector3 spawnPos = Vector3.zero;
        
        if (spawnTiles != null && spawnTiles.Count > 0 && currentSpawnIndex < spawnTiles.Count)
        {
            Vector2Int spawnTile = spawnTiles[currentSpawnIndex];
            currentSpawnIndex++;
            spawnPos = gridManager.GridToWorld(spawnTile);
            spawnPos.z = 0f;
        }
        
        var playerInstance = PlayerInput.Instantiate(
            playerCubePrefab,
            playerIndex: config.PlayerIndex,
            controlScheme: config.ControlScheme, // Esto asegura que sea WASD o Arrows
            splitScreenIndex: -1,
            pairWithDevice: config.Device // Esto asegura que use el teclado correcto o el gamepad
        );
        playerInstance.name = $"Jugador {config.PlayerIndex + 1}";
        playerInstance.transform.position = spawnPos;
        playerInstance.transform.SetParent(npcContainer.transform);
        
        // Asignar color aleatorio al jugador
        SpriteRenderer playerSr = playerInstance.GetComponent<SpriteRenderer>();
        if (playerSr != null)
        {
            playerSr.color = hellPalette[Random.Range(0, hellPalette.Length)];
        }
        
        PlayerMovement playerM = playerInstance.GetComponent<PlayerMovement>();
        
        if(playerM != null)
        {
            Image i = playerUI[config.PlayerIndex];
            playerM.initializeUI(i);
        }
    }
}
