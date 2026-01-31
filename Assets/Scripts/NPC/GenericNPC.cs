using UnityEngine;

public class GenericNPC : MonoBehaviour
{
    [SerializeField] private Animator anim;

    void Start()
    {
        //anim.Play("Idle");
    }

    void Die()
    {
        //anim.Play("Death");
    }
}
