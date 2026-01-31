using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    [SerializeField] protected Animator anim;
    [SerializeField] protected float speed = 2f;

    protected GridManager grid;
    protected Pathfinder pathfinder;

    protected virtual void Awake()
    {
        if (anim == null)
            anim = GetComponent<Animator>();

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
