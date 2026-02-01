using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    [SerializeField] protected float speed = 2f;
    [SerializeField] protected SpriteRenderer spriteRenderer;

    protected GridManager grid;
    protected Pathfinder pathfinder;

    private Vector3 _lastPosition;
    private Vector2 _lastDirection = Vector2.down;


    protected virtual void OnEnable()
    {
        // Reiniciamos la posici�n al activar el script para evitar "saltos" de animaci�n
        _lastPosition = transform.position;
    }


    

    protected virtual void Awake()
    {

        grid = FindFirstObjectByType<GridManager>();
        pathfinder = FindFirstObjectByType<Pathfinder>();
    }

    public void ChangeToIdle()
    {
        EnableMovement<NPCIdleMovement>();
    }

    public void ChangeToErratic()
    {
        EnableMovement<NPCErraticMovement>();
    }

    public void ChangeToMirror()
    {
        EnableMovement<NPCMirrorMovement>();
    }

    void EnableMovement<T>() where T : NPCMovement
    {
        NPCMovement newMovement = GetComponent<T>();
        newMovement.enabled = true;
        this.enabled = false;
    }
}
