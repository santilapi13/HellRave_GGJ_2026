using UnityEngine;

public class GenericNPC : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private NPCMovement[] movements;

    void Start()
    {
    }

    public void Die()
    {
        Debug.Log("me empujaron");
        animator.SetTrigger("Pushed");
    }

    public void DisableMovement()
    {
        foreach(NPCMovement mov in movements)
        {
            mov.enabled = false;
        }
    }

    public void EnabledMovement()
    {
        foreach(NPCMovement mov in movements)
        {
            mov.enabled = true;
        }
    }
}

