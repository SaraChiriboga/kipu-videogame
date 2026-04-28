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
    public float balanceGiroscopio;
    private bool steamInicializado = false;

    private InputHandle_t[] handles = new InputHandle_t[Constants.STEAM_INPUT_MAX_COUNT];
    private int controllerCount = 0;

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
        if (steamInicializado)
        {
            SteamAPI.RunCallbacks();
            controllerCount = SteamInput.GetConnectedControllers(handles);
        }

        // ─── 1. MOVIMIENTO LATERAL ───────────────────────────────────────────
        float moveTeclado = 0f;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) moveTeclado = 1f;
            else if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) moveTeclado = -1f;
        }

        float moveMando = 0f;
        if (Gamepad.current != null)
        {
            moveMando = Gamepad.current.leftStick.x.ReadValue();

            if (Mathf.Abs(moveMando) > 0.1f)
                Debug.Log($"<color=cyan>STICK Unity: {moveMando:F3}</color>");
        }

        moveInput = Mathf.Abs(moveMando) > 0.1f ? moveMando : moveTeclado;

        // ─── 2. SALTO ────────────────────────────────────────────────────────
        bool intentandoSaltar = false;
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame) intentandoSaltar = true;
        if (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame) intentandoSaltar = true;

        if (intentandoSaltar && Mathf.Abs(rb.linearVelocity.y) < 0.01f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaSalto);
        }

        // ─── 3. BALANCE (MOUSE + GYRO) ───────────────────────────────────────
        float balanceMouse = 0f;
        if (Mouse.current != null && Mouse.current.leftButton.isPressed)
        {
            balanceMouse = Mouse.current.delta.x.ReadValue() * sensibilidadMouse;
        }

        if (steamInicializado && controllerCount > 0)
        {
            InputMotionData_t motion = SteamInput.GetMotionData(handles[0]);

            float gyroX = motion.rotVelX;
            float gyroY = motion.rotVelY;
            float gyroZ = motion.rotVelZ;
            float rawGyro = gyroX + gyroY + gyroZ;

            balanceGiroscopio = rawGyro * sensibilidadGiro;

            if (Mathf.Abs(rawGyro) > 0.0001f)
            {
                Debug.Log($"<color=yellow>GYRO — X: {gyroX:F4} | Y: {gyroY:F4} | Z: {gyroZ:F4} | Total: {rawGyro:F4}</color>");
            }
        }

        // ─── 4. ANIMACIÓN ────────────────────────────────────────────────────
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