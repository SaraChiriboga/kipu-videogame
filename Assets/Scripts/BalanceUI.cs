using UnityEngine;
using UnityEngine.UI;

public class BalanceUI : MonoBehaviour
{
    [Header("Referencias")]
    public BalanceSystem balanceSystem;
    public RectTransform indicador;       // la bolita
    public Image timerborde;              // borde que se vacía
    public GameObject flechaIzquierda;
    public GameObject flechaDerecha;
    public CanvasGroup grupoUI;           // para fade in/out

    [Header("Configuración")]
    public float rangoMovimiento = 110f;  // cuántos px se mueve la bolita

    private Color _colorVerde = new Color(0.30f, 0.69f, 0.31f);
    private Color _colorRojo = new Color(0.90f, 0.23f, 0.23f);

    void Start()
    {
        grupoUI = GetComponent<CanvasGroup>();
        if (grupoUI == null) grupoUI = gameObject.AddComponent<CanvasGroup>();
        grupoUI.alpha = 0f;              // empieza invisible
    }

    void Update()
    {
        if (balanceSystem == null) return;

        bool activo = balanceSystem.EstaDesequilibrado();

        // Fade in/out suave
        grupoUI.alpha = Mathf.MoveTowards(grupoUI.alpha, activo ? 1f : 0f, Time.deltaTime * 4f);

        if (!activo) return;

        // Mueve la bolita según el balance (-1..1)
        float balance = balanceSystem.EstaDesequilibrado() ?
            (Random.value > 0.5f ? 1f : -1f) : 0f;  // dirección del desequilibrio actual
        indicador.anchoredPosition = new Vector2(balance * rangoMovimiento, 0f);

        // Timer: color verde → rojo según tiempo restante
        float t = balanceSystem.GetTimerNormalizado();
        timerborde.fillAmount = t;
        timerborde.color = Color.Lerp(_colorRojo, _colorVerde, t);

        // Flechas: muestra solo la que hay que presionar
        flechaIzquierda.SetActive(balance < 0f);
        flechaDerecha.SetActive(balance > 0f);
    }
}