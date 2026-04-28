using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class BalanceSystem : MonoBehaviour
{
    [Header("Referencias")]
    public AmadaMovement amadaMovement;
    public Rigidbody2D rb;
    public Animator anim;

    [Header("Configuración")]
    public float intervaloDesequilibrio = 6f;   // cada cuánto se inclina
    public float tiempoParaCorregir = 1.8f;     // segundos para presionar la tecla
    public float fuerzaKnockback = 3f;
    public float tiempoPausaMovimiento = 0.9f;
    [Range(0f, 1f)] public float penalizacionSync = 0.1f;

    [Header("Eventos UI")]
    public UnityEvent<float> onBalanceCambia;   // valor -1 (izq) a 1 (der)
    public UnityEvent<float> onSyncCambia;
    public UnityEvent onCaida;
    public UnityEvent onEquilibrado;

    // -- estado --
    private float _balance = 0f;           // -1 izq, 0 centro, 1 der
    private float _timerCorreccion = 0f;
    private float _timerDesequilibrio = 0f;
    private float _sync = 1f;
    private bool _esperandoCorreccion = false;

    void Start()
    {
        _timerDesequilibrio = intervaloDesequilibrio;
    }

    void Update()
    {
        TickDesequilibrio();

        if (_esperandoCorreccion)
            TickCorreccion();
    }

    void TickDesequilibrio()
    {
        if (_esperandoCorreccion) return;

        _timerDesequilibrio -= Time.deltaTime;
        if (_timerDesequilibrio <= 0f)
        {
            // Inclina al personaje a un lado aleatorio
            _balance = Random.value > 0.5f ? 1f : -1f;
            _timerCorreccion = tiempoParaCorregir;
            _esperandoCorreccion = true;
            onBalanceCambia?.Invoke(_balance);
            _timerDesequilibrio = intervaloDesequilibrio + Random.Range(-1f, 1.5f);
        }
    }

    void TickCorreccion()
    {
        _timerCorreccion -= Time.deltaTime;

        bool presionoIzquierda = Keyboard.current.aKey.wasPressedThisFrame ||
                                 Keyboard.current.leftArrowKey.wasPressedThisFrame;
        bool presionoDerecha = Keyboard.current.dKey.wasPressedThisFrame ||
                                 Keyboard.current.rightArrowKey.wasPressedThisFrame;

        bool corrigio = (_balance < 0 && presionoIzquierda) ||
                        (_balance > 0 && presionoDerecha);

        if (corrigio)
        {
            Equilibrar();
            return;
        }

        if (_timerCorreccion <= 0f)
            Caer();
    }

    void Equilibrar()
    {
        _balance = 0f;
        _esperandoCorreccion = false;
        onBalanceCambia?.Invoke(0f);
        onEquilibrado?.Invoke();
    }

    void Caer()
    {
        _esperandoCorreccion = false;
        _balance = 0f;

        _sync = Mathf.Max(0f, _sync - penalizacionSync);
        onSyncCambia?.Invoke(_sync);
        onCaida?.Invoke();

        float dir = _balance > 0f ? -1f : 1f;
        rb.linearVelocity = new Vector2(dir * fuerzaKnockback, rb.linearVelocity.y);

        amadaMovement.enabled = false;
        anim?.SetBool("isStumbling", true);
        anim?.SetTrigger("Stumble");

        Invoke(nameof(TerminarCaida), tiempoPausaMovimiento);
    }

    void TerminarCaida()
    {
        amadaMovement.enabled = true;
        anim?.SetBool("isStumbling", false);
        onBalanceCambia?.Invoke(0f);
    }

    public float GetTimerNormalizado() =>
        _esperandoCorreccion ? _timerCorreccion / tiempoParaCorregir : 1f;

    public bool EstaDesequilibrado() => _esperandoCorreccion;
}