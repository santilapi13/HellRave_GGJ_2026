using UnityEngine;
using System.Collections.Generic;

public class NPCManager : MonoBehaviour
{
    [SerializeField] private Transform npcContainer;
    [SerializeField] private Transform[] characters;
    
    [Header("Behavior Configuration")]
    [SerializeField] private int numberOfBehaviorTypes = 2;

    private GridManager gridManager;
    private List<Vector2Int> walkableTiles;
    private int currentCharacterIndex = 0;
    private int[] behaviorAssignments;

    void Start()
    {
        gridManager = FindFirstObjectByType<GridManager>();
        if (gridManager == null)
        {
            Debug.LogError("GridManager not found in scene!");
            return;
        }

        CollectWalkableTiles();
        InitializeCharacters();
        AssignBehaviors();
        initMap();
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

        Debug.Log($"Collected {walkableTiles.Count} walkable tiles.");
    }

    private void InitializeCharacters()
    {
        if (npcContainer == null)
        {
            Debug.LogWarning("NPC Container is not assigned.");
            return;
        }

        int childCount = npcContainer.childCount;
        characters = new Transform[childCount];

        for (int i = 0; i < childCount; i++)
        {
            characters[i] = npcContainer.GetChild(i);
        }

        for (int i = characters.Length - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            Transform temp = characters[i];
            characters[i] = characters[randomIndex];
            characters[randomIndex] = temp;
        }

        Debug.Log($"Initialized {characters.Length} characters from NPC Container.");
    }

    private void AssignBehaviors()
    {
        if (characters == null || characters.Length == 0)
        {
            Debug.LogWarning("No characters to assign behaviors to.");
            return;
        }

        if (numberOfBehaviorTypes <= 0)
        {
            Debug.LogWarning("Number of behavior types must be greater than 0.");
            return;
        }

        behaviorAssignments = new int[characters.Length];
        int npcsPerBehavior = characters.Length / numberOfBehaviorTypes;
        int remainingNpcs = characters.Length % numberOfBehaviorTypes;

        int assignmentIndex = 0;
        for (int behaviorType = 0; behaviorType < numberOfBehaviorTypes; behaviorType++)
        {
            int count = npcsPerBehavior;
            
            if (behaviorType < remainingNpcs)
            {
                count++;
            }

            for (int i = 0; i < count; i++)
            {
                behaviorAssignments[assignmentIndex] = behaviorType;
                assignmentIndex++;
            }
        }

        for (int i = behaviorAssignments.Length - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            int temp = behaviorAssignments[i];
            behaviorAssignments[i] = behaviorAssignments[randomIndex];
            behaviorAssignments[randomIndex] = temp;
        }

        Debug.Log($"Assigned {numberOfBehaviorTypes} behavior types to {characters.Length} characters.");
    }

    private void initMap()
    {
        if (characters == null || characters.Length == 0)
        {
            Debug.LogWarning("No characters available in the pool.");
            return;
        }

        if (walkableTiles == null || walkableTiles.Count == 0)
        {
            Debug.LogWarning("No walkable tiles available.");
            return;
        }

        int tileIndex = 0;

        for (int i = 0; i < characters.Length; i++)
        {
            if (tileIndex >= walkableTiles.Count)
            {
                Debug.LogWarning($"Not enough walkable tiles for all characters. Placed {i} out of {characters.Length}.");
                break;
            }

            Transform character = characters[i];

            // Obtener un tile aleatorio de la lista ya randomizada
            Vector2Int gridPos = walkableTiles[tileIndex];
            tileIndex++;

            // Convertir la posición de grid a posición del mundo
            Vector3 worldPosition = gridManager.GridToWorld(gridPos);
            worldPosition.z = 0f;

            character.position = worldPosition;

            // Asignar comportamiento al NPC
            GenericNPC npcComponent = character.GetComponent<GenericNPC>();
            if (npcComponent != null)
            {
                NPCMovement[] allBehaviors = character.GetComponents<NPCMovement>();
                
                if (allBehaviors.Length > 0)
                {
                    int assignedBehaviorIndex = behaviorAssignments[i];
                    
                    Debug.Log($"NPC {character.name} at {worldPosition} - Behaviors: {allBehaviors.Length}, Assigned: {assignedBehaviorIndex}");
                    
                    // Desactivar todos los comportamientos primero
                    for (int j = 0; j < allBehaviors.Length; j++)
                    {
                        allBehaviors[j].enabled = false;
                    }
                    
                    // Activar solo el comportamiento asignado
                    if (assignedBehaviorIndex < allBehaviors.Length)
                    {
                        allBehaviors[assignedBehaviorIndex].enabled = true;
                        Debug.Log($"  Enabled: {allBehaviors[assignedBehaviorIndex].GetType().Name}");
                    }
                    else
                    {
                        Debug.LogError($"Assigned behavior index {assignedBehaviorIndex} out of range for {allBehaviors.Length} behaviors");
                    }
                }
                else
                {
                    Debug.LogWarning($"NPC {character.name} has no NPCMovement components.");
                }
            }

            character.gameObject.SetActive(true);
        }
    }

}
