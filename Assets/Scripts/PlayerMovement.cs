using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    [Header("Configuración del Ataque")]
    [SerializeField] private float radioDeAtaque = 0.5f;
    [SerializeField] private float atackCDR = 1.0f;
    [SerializeField] private float offsetAtaqueY = 0.3f;

    [Header("Filtros")]
    [SerializeField] private LayerMask capaJugadores;

    [SerializeField] private float speed = 5f;
    [SerializeField] private Animator animator;
    private bool canMove = true;
    private bool canAtack = true;
    [SerializeField] public bool CanMove
    {
        get { return canMove; }
        set 
        { 
            // Verificamos si el valor realmente cambió para no spamear
            if (canMove != value)
            {
                canMove = value;
            }
        }
    }
    private GameObject child;
    private SpriteRenderer sprite;
    private bool isDead = false;
    
    private Vector2 moveInput;

    private void Awake()
    {
        child = transform.GetChild(0).gameObject;
        sprite = GetComponent<SpriteRenderer>();
    }

    public void OnMove(InputValue value)
    {
        if (!CanMove)
        {
         moveInput = Vector2.zero;
         return;   
        }
        moveInput = value.Get<Vector2>();
    }

    public void OnAct(InputValue value)
    {
        if (value.isPressed && canAtack)
        {
            CanMove = false;
            canAtack = false;
            animator.SetTrigger("Empujar");
        }
    }

    public void EndAct()
    {
        CanMove = true;
        StartCoroutine(AtackCooldown());
    }

    private IEnumerator AtackCooldown()
    {
        // Esperamos los segundos definidos
        yield return new WaitForSeconds(atackCDR);

        // Volvemos a activar el ataque
        canAtack = true;
    }

    void FixedUpdate()
    {
        if(!CanMove) return;
        // Movimiento simple modificando el transform
        Vector3 movement = new Vector3(moveInput.x,moveInput.y, 0) * speed * Time.deltaTime;
        transform.Translate(movement);
    }

    public void EjecutarGolpe()
    {

        Vector2 origen = (Vector2)transform.position + new Vector2(0, offsetAtaqueY);
        Collider2D[] todosLosTocados = Physics2D.OverlapCircleAll(origen, radioDeAtaque, capaJugadores);

        foreach (Collider2D col in todosLosTocados)
        {
            // 1. FILTRO DE IDENTIDAD: Si soy yo mismo, paso al siguiente
            if (col.gameObject == gameObject) continue;

            // 2. FILTRO DE COMPONENTE: ¿Es realmente un jugador?
            // Esto evita que un enemigo golpee a otro enemigo si comparten capa
            PlayerMovement jugador = col.GetComponent<PlayerMovement>();
            if(isDead) return;

            if (jugador != null)
            {
                jugador.Die();
                return; 
            }
        }
        
        foreach (Collider2D col in todosLosTocados)
        {
            GenericNPC npc = col.GetComponent<GenericNPC>();
            if (npc != null)
            {
                npc.Die(transform);
                return; 
            }
        }
    }

    public void Die()
    {
       animator.speed = 0;
       CanMove = false;
       isDead = true;
       animator.SetTrigger("Die");
       animator.speed = 1;
    }

    public void Destroy()
    {
        GameManager.Instance.RemovePlayer(gameObject);
        Destroy(gameObject);
    }

    public void DestoyMask()
    {
        int randomIndex = Random.Range(1, 3);
        string sfxName = "muerte_jugador_" + randomIndex;
        AudioManager.Instance?.PlaySFX(sfxName,false);
        sprite.color = Color.white;
        Destroy(child);
    }

    private void OnDrawGizmosSelected()
    {
        // Dibujar el radio de ataque
        Gizmos.color = Color.red;
        Vector3 attackCenter = transform.position + new Vector3(0, offsetAtaqueY, 0);
        Gizmos.DrawWireSphere(attackCenter, radioDeAtaque);
    }
}
