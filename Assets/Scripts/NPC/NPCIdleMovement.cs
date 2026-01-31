using UnityEngine;

public class NPCIdleMovement : NPCMovement
{
    void OnEnable()
    {
        if (anim != null)
        {
            anim.Play("Idle");
        } else
        {
            Debug.LogWarning("Animator not assigned in NPCIdleMovement.");
        }
    }
}
