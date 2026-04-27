using UnityEngine;
using UnityEngine.InputSystem;

public class AmadaMovement : MonoBehaviour
{
    [Header("Configuración")]
    public float velocidad = 7f;
    public float fuerzaSalto = 12f;
    public float sensibilidadGiro = 5.0f;

    private Rigidbody2D rb;
    private Animator anim;
    private float inputX;
    private float inclinacionGiroscopio;

    public bool recibirHueso = false;
    public bool isStumbling = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        // 1. ACTIVAR SENSORES DE FORMA SEGÚRA (Sin ambigüedades)
        // Usamos el nombre completo para que Unity no se confunda
        if (UnityEngine.InputSystem.Accelerometer.current != null)
            InputSystem.EnableDevice(UnityEngine.InputSystem.Accelerometer.current);

        if (UnityEngine.InputSystem.GravitySensor.current != null)
            InputSystem.EnableDevice(UnityEngine.InputSystem.GravitySensor.current);
    }

    public void OnMove(InputValue value)
    {
        inputX = value.Get<Vector2>().x;
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed && !isStumbling && Mathf.Abs(rb.linearVelocity.y) < 0.01f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaSalto);
        }
    }

    // Asegúrate de que en el Action Map, el binding de Giroscopio 
    // apunte a "Gravity [Sensor]" o "Accelerometer [Sensor]"
    public void OnGiroscopio(InputValue value)
    {
        Vector3 gravityData = value.Get<Vector3>();

        // En el mando de PS5, el eje X suele representar la inclinación lateral
        inclinacionGiroscopio = gravityData.x;
    }

    void FixedUpdate()
    {
        if (isStumbling)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        rb.linearVelocity = new Vector2(inputX * velocidad, rb.linearVelocity.y);

        if (anim != null)
        {
            anim.SetBool("isMoving", inputX != 0);

            // Combinamos el input del stick y el giro físico del mando
            float balanceFinal = inputX + (inclinacionGiroscopio * sensibilidadGiro);
            balanceFinal = Mathf.Clamp(balanceFinal, -1f, 1f);

            anim.SetFloat("Inclinacion", balanceFinal);

            if (inputX > 0.1f) transform.localScale = Vector3.one;
            else if (inputX < -0.1f) transform.localScale = new Vector3(-1, 1, 1);
        }
    }
}