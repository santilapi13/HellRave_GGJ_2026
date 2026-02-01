using UnityEngine;

public class Guard : MonoBehaviour
{
    [SerializeField] private NPCErraticMovement erraticMovement;
    [SerializeField] private ChasePlayerMovement chasePlayerMovement;

    void Start()
    {
        erraticMovement.enabled = true;
        chasePlayerMovement.enabled = false;
    }

    public void ChangeToChase(Transform playerToChase)
    {
        erraticMovement.enabled = false;
        chasePlayerMovement.SetTarget(playerToChase);
        chasePlayerMovement.enabled = true;
    }

    public void ChangeToErratic()
    {
        chasePlayerMovement.enabled = false;
        erraticMovement.enabled = true;
    }

}
