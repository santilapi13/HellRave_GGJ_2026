using UnityEngine;

public class GenericNPC : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private NPCMovement[] movements;

    void Start()
    {
    }

    public void Die(Transform player)
    {
        AudioManager.Instance.PlaySFX("push",false);
        animator.SetTrigger("Pushed");
        Guard.Instance.ChangeToChase(player);
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

