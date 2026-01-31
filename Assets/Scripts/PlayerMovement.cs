using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    private Vector2 moveInput;
    private Rigidbody rb; // Si usas física, o transform si no.

    // Esta función se llama automáticamente si el PlayerInput está en "Send Messages"
    // y tu acción en el Input Asset se llama "Move".
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void Update()
    {
        // Movimiento simple modificando el transform
        Vector3 movement = new Vector3(moveInput.x,moveInput.y, 0) * speed * Time.deltaTime;
        transform.Translate(movement);
    }
}
