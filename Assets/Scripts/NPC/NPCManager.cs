using UnityEngine;

public class NPCManager : MonoBehaviour
{
    [SerializeField] private Transform npcContainer;
    [SerializeField] private Transform[] characters;
    
    [Header("Grid Configuration")]
    [SerializeField] private int gridRows = 3;
    [SerializeField] private int gridColumns = 3;
    [SerializeField] private Vector2 mapSize = new Vector2(30f, 30f);
    [SerializeField] private Vector2 mapCenter = Vector2.zero;
    
    [Header("Behavior Configuration")]
    [SerializeField] private int numberOfBehaviorTypes = 3;

    private int currentCharacterIndex = 0;
    private int[] behaviorAssignments;

    void Start()
    {
        InitializeCharacters();
        AssignBehaviors();
        initMap();
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

        float cellWidth = mapSize.x / gridColumns;
        float cellHeight = mapSize.y / gridRows;

        Vector2 mapBottomLeft = mapCenter - (mapSize / 2f);

        for (int row = 0; row < gridRows; row++)
        {
            for (int col = 0; col < gridColumns; col++)
            {
                if (currentCharacterIndex >= characters.Length)
                {
                    Debug.LogWarning("No more characters available in the pool.");
                    return;
                }

                Transform character = characters[currentCharacterIndex];
                currentCharacterIndex++;

                float cellMinX = mapBottomLeft.x + (col * cellWidth);
                float cellMinY = mapBottomLeft.y + (row * cellHeight);
                float cellMaxX = cellMinX + cellWidth;
                float cellMaxY = cellMinY + cellHeight;

                float randomX = Random.Range(cellMinX, cellMaxX);
                float randomY = Random.Range(cellMinY, cellMaxY);
                Vector3 randomPosition = new Vector3(randomX, randomY, 0f);

                character.position = randomPosition;
                character.position = new Vector3(character.position.x, character.position.y, 0f);

                GenericNPC npcComponent = character.GetComponent<GenericNPC>();
                if (npcComponent != null)
                {
                    NPCMovement[] allBehaviors = character.GetComponents<NPCMovement>();
                    
                    if (allBehaviors.Length > 0)
                    {
                        int assignedBehaviorIndex = behaviorAssignments[currentCharacterIndex - 1];
                        
                        for (int i = 0; i < allBehaviors.Length; i++)
                        {
                            if (i == assignedBehaviorIndex && i < allBehaviors.Length)
                            {
                                allBehaviors[i].enabled = true;
                            }
                            else
                            {
                                allBehaviors[i].enabled = false;
                            }
                        }
                        
                        Debug.Log($"NPC {character.name} assigned behavior type {assignedBehaviorIndex}");
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

}
