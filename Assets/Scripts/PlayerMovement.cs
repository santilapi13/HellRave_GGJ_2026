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
    private Vector2 moveInput;
    private Rigidbody rb; // Si usas física, o transform si no.

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

    void Update()
    {
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
        //PlayerController jugador = col.GetComponent<PlayerController>();

        if (col != null)
        {
            Debug.Log($"¡Golpeaste a {col.name}!");
            // jugador.RecibirDano(daňo);

            // 3. IMPORTANTE: 'break' para golpear solo al primero que encontremos y salir
            break; 
        }
    }
    }
}
