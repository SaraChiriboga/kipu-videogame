using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Configuraci�n de Movimiento")]
    public float moveSpeed = 5f;
    public float jumpForce = 12f;

    [Header("Detecci�n de Suelo")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    // Componentes
    private Rigidbody2D rb;
    private Animator anim;
    private PlayerControls controls; // La clase autogenerada del Input System

    // Variables de Estado
    private Vector2 moveInput;
    private bool isGrounded;
    private bool facingRight = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        // Inicializamos los controles
        controls = new PlayerControls();

        // Suscribimos eventos de Input
        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        controls.Player.Jump.performed += ctx => Jump();
    }

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    private void Update()
    {
        CheckGround();
        ManageAnimation();
    }

    private void FixedUpdate()
    {
        // Aplicamos el movimiento f�sico aqu�
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);

        // Volteamos el sprite dependiendo de la direcci�n
        if (moveInput.x > 0 && !facingRight) Flip();
        else if (moveInput.x < 0 && facingRight) Flip();
    }

    private void Jump()
    {
        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    private void CheckGround()
    {
        // Crea un peque�o c�rculo en los pies para detectar la capa "Ground"
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void ManageAnimation()
    {
        // Verificamos si hay input horizontal
        bool isWalking = Mathf.Abs(moveInput.x) > 0.1f;

        // Pasamos las variables al Animator
        anim.SetBool("isWalking", isWalking);
        anim.SetBool("isGrounded", isGrounded);
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    private void OnDrawGizmosSelected()
    {
        // Esto dibuja un c�rculo en el editor para que puedas ajustar el tama�o del GroundCheck visualmente
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}