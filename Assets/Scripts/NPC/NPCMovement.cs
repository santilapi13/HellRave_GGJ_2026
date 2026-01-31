using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    [SerializeField] protected Animator anim;
    [SerializeField] protected float speed = 2f;
    [SerializeField] protected SpriteRenderer spriteRenderer;

    protected GridManager grid;
    protected Pathfinder pathfinder;

    private Vector3 _lastPosition;
    private Vector2 _lastDirection = Vector2.down;


    protected virtual void OnEnable()
    {
        // Reiniciamos la posición al activar el script para evitar "saltos" de animación
        _lastPosition = transform.position;
    }

    protected virtual void LateUpdate()
    {
        AnimateMovement();
    }

    private void AnimateMovement()
    {
        if (anim == null) return;

        // 1. Calcular el desplazamiento real desde el último frame
        Vector3 movementDelta = transform.position - _lastPosition;

        // Usamos una magnitud pequeña para filtrar ruido numérico
        bool isMoving = movementDelta.magnitude > 0.001f;

        // 2. Enviar datos al Animator (Blend Tree)
        anim.SetBool("IsMoving", isMoving);

        if (isMoving)
        {
            // Normalizamos para obtener dirección pura (-1 a 1)
            Vector2 direction = new Vector2(movementDelta.x, movementDelta.y).normalized;

            anim.SetFloat("InputX", direction.x);
            anim.SetFloat("InputY", direction.y);

            // Guardamos la última dirección para cuando se detenga
            _lastDirection = direction;

            // 3. Manejo de FlipX (Si usas animación lateral compartida)
            if (spriteRenderer != null && Mathf.Abs(direction.x) > 0.01f)
            {
                spriteRenderer.flipX = direction.x < 0;
            }
        }
        else
        {
            // Mantenemos la última dirección conocida para el Idle
            anim.SetFloat("InputX", _lastDirection.x);
            anim.SetFloat("InputY", _lastDirection.y);
        }

        // Actualizamos para el siguiente frame
        _lastPosition = transform.position;
    }

    protected virtual void Awake()
    {
            anim = GetComponent<Animator>();

        grid = FindFirstObjectByType<GridManager>();
        pathfinder = FindFirstObjectByType<Pathfinder>();
    }

    public void ChangeToIdle()
    {
        EnableMovement<NPCIdleMovement>();
    }

    public void ChangeToErratic()
    {
        EnableMovement<NPCErraticMovement>();
    }

    public void ChangeToMirror()
    {
        EnableMovement<NPCMirrorMovement>();
    }

    void EnableMovement<T>() where T : NPCMovement
    {
        NPCMovement newMovement = GetComponent<T>();
        newMovement.enabled = true;
        this.enabled = false;
    }
}
