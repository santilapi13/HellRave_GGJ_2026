using UnityEngine;

public class Guard : MonoBehaviour
{
    [SerializeField] private NPCErraticMovement erraticMovement;
    [SerializeField] private ChasePlayerMovement chasePlayerMovement;
    [SerializeField] private Animator animator;
    public static Guard Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        erraticMovement.enabled = false;
        chasePlayerMovement.enabled = false;
        animator.SetBool("isChasing", false);
        RisaAleatoria();
        
    }

    private void RisaAleatoria()
    {
        float randomTime = Random.Range(5f, 30f);
        Invoke("Reirse", randomTime);
    }

    private void Reirse()
    {
        AudioManager.Instance?.PlaySFX("risa_random", false);
        RisaAleatoria();
    }

    public void ChangeToChase(Transform playerToChase)
    {
        erraticMovement.enabled = false;
        chasePlayerMovement.SetTarget(playerToChase);
        chasePlayerMovement.enabled = true;
        AudioManager.Instance?.PlaySFX("smokin", false);
        animator.SetBool("isChasing", true);
    }

    public void ChangeToErratic()
    {
        chasePlayerMovement.enabled = false;
        //erraticMovement.enabled = true;
        animator.SetBool("isChasing", false);
    }

}
