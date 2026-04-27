using UnityEngine;
using Steamworks;
using UnityEngine.InputSystem;

public class AmadaMovement : MonoBehaviour
{
    [Header("Configuración Física")]
    public float velocidad = 7f;
    public float fuerzaSalto = 12f;

    [Header("Configuración Equilibrio")]
    public float sensibilidadGiro = 1.5f;
    public float sensibilidadMouse = 0.2f;

    private Rigidbody2D rb;
    private Animator anim;
    private float moveInput;
    private float balanceGiroscopio;
    private bool steamInicializado = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        try
        {
            steamInicializado = SteamAPI.Init();
            if (steamInicializado)
            {
                Debug.Log("<color=green>Steamworks: Conectado.</color>");
                // Forzamos a Steam a que empiece a mandar datos de mando
                SteamInput.Init(false);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error Steamworks: " + e.Message);
        }
    }

    void Update()
    {
        // 1. MOVIMIENTO LATERAL
        float moveTeclado = 0f;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) moveTeclado = 1f;
            else if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) moveTeclado = -1f;
        }

        float moveMando = (Gamepad.current != null) ? Gamepad.current.leftStick.x.ReadValue() : 0f;
        moveInput = Mathf.Abs(moveMando) > 0.1f ? moveMando : moveTeclado;

        // 2. SALTO (EL MÉTODO QUE ME COMÍ)
        bool intentandoSaltar = false;
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame) intentandoSaltar = true;
        if (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame) intentandoSaltar = true;

        if (intentandoSaltar && Mathf.Abs(rb.linearVelocity.y) < 0.01f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaSalto);
        }

        // 3. BALANCE (MOUSE + GYRO)
        float balanceMouse = 0f;
        if (Mouse.current != null && Mouse.current.leftButton.isPressed) // Solo balancea si haces clic o mueves mucho
        {
            balanceMouse = Mouse.current.delta.x.ReadValue() * sensibilidadMouse;
        }

        if (steamInicializado)
        {
            SteamAPI.RunCallbacks();
            InputHandle_t[] handles = new InputHandle_t[Constants.STEAM_INPUT_MAX_COUNT];
            int count = SteamInput.GetConnectedControllers(handles);

            if (count > 0)
            {
                // SteamInput.GetMotionData es lo que necesitamos
                InputMotionData_t motion = SteamInput.GetMotionData(handles[0]);

                // Sumamos los tres ejes de rotación para encontrar cuál es el tuyo
                // rotVelX, rotVelY, rotVelZ (Velocidad Angular)
                float rawGyro = motion.rotVelX + motion.rotVelY + motion.rotVelZ;

                balanceGiroscopio = rawGyro * sensibilidadGiro;

                // Debug ultra sensible para ver si el mando respira
                if (Mathf.Abs(rawGyro) > 0.0001f)
                {
                    Debug.Log($"<color=yellow>GYRO VIVO -> Val: {rawGyro:F4}</color>");
                }
            }
        }

        // 4. ANIMACIÓN
        if (anim != null)
        {
            float balanceTotal = Mathf.Clamp(moveInput + balanceGiroscopio + balanceMouse, -1f, 1f);
            anim.SetFloat("Inclinacion", balanceTotal);
            anim.SetBool("isMoving", Mathf.Abs(moveInput) > 0.1f);

            if (moveInput > 0.1f) transform.localScale = Vector3.one;
            else if (moveInput < -0.1f) transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveInput * velocidad, rb.linearVelocity.y);
    }

    void OnDestroy()
    {
        if (steamInicializado) SteamAPI.Shutdown();
    }
}