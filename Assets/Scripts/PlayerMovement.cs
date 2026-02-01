using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    [Header("Configuración del Ataque")]
    public float radioDeAtaque = 0.5f;
    [Header("Filtros")]
    public LayerMask capaJugadores;

    [SerializeField] private float speed = 5f;
    [SerializeField] private Animator animator;
    [SerializeField] private bool canMove = true;
    private GameObject child;
    private SpriteRenderer sprite;
    
    private Vector2 moveInput;

    private void Awake()
    {
        child = transform.GetChild(0).gameObject;
        sprite = GetComponent<SpriteRenderer>();
    }

    // Esta función se llama automáticamente si el PlayerInput está en "Send Messages"
    // y tu acción en el Input Asset se llama "Move".
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnAct(InputValue value)
    {
        if (value.isPressed)
        {
            animator.SetTrigger("Empujar");
        }
    }

    void FixedUpdate()
    {
        if(!canMove) return;
        // Movimiento simple modificando el transform
        Vector3 movement = new Vector3(moveInput.x,moveInput.y, 0) * speed * Time.deltaTime;
        transform.Translate(movement);
    }

    public void EjecutarGolpe()
    {
        Vector2 origen = (Vector2)transform.position;
        Collider2D[] todosLosTocados = Physics2D.OverlapCircleAll(origen, radioDeAtaque, capaJugadores);

        foreach (Collider2D col in todosLosTocados)
        {
            // 1. FILTRO DE IDENTIDAD: Si soy yo mismo, paso al siguiente
            if (col.gameObject == gameObject) continue;

            // 2. FILTRO DE COMPONENTE: ¿Es realmente un jugador?
            // Esto evita que un enemigo golpee a otro enemigo si comparten capa
            PlayerMovement jugador = col.GetComponent<PlayerMovement>();

            if (jugador != null)
            {
                jugador.Die();
                return; 
            }
        }
        
        if(todosLosTocados.Length >= 2){
            Debug.Log($"Entre");
            GenericNPC npc = todosLosTocados[1].GetComponent<GenericNPC>();
            npc.Die();
        }
    }

    public void Die()
    {
       animator.SetTrigger("Die");
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    public void DestoyMask()
    {
        sprite.color = Color.white;
        Destroy(child);
    }

     
}
