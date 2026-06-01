using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public struct LineaDialogo
{
    public string hablante;
    [TextArea(2, 4)]
    public string texto;
}

public class IntroCinematic : MonoBehaviour
{
    [Header("Personajes")]
    public Transform npcDulceMaria;
    public Animator animDulceMaria;
    public Animator animAmada;
    public PlayerMovement amadaMovement;

    [Header("Ruta del NPC")]
    public Transform puntoDeEncuentro;
    public float velocidadNPC = 2.5f;

    [Header("Interfaz de Diálogo")]
    public UIDocument uiDocument;
    public float velocidadEscritura = 0.04f;
    public float tiempoParaLeer = 2.5f;
    public float tiempoFade = 0.5f; // NUEVO: Cuánto tarda en aparecer/desaparecer el cuadro

    [Header("Guion de la Escena")]
    public LineaDialogo[] lineasDeDialogo;

    private VisualElement panelDialogo;
    private Label textoDialogo;

    private void Awake()
    {
        var root = uiDocument.rootVisualElement;
        panelDialogo = root.Q<VisualElement>("ContenedorDialogo");
        textoDialogo = root.Q<Label>("TextoDialogo");

        if (panelDialogo != null)
        {
            // Iniciamos el panel apagado y totalmente transparente
            panelDialogo.style.display = DisplayStyle.None;
            panelDialogo.style.opacity = 0f;
        }
    }

    private void Start()
    {
        StartCoroutine(EjecutarCinematica());
    }

    private IEnumerator EjecutarCinematica()
    {
        // 1. Bloqueo inicial
        amadaMovement.enabled = false;

        // 2. Dulce María camina
        animDulceMaria.SetBool("isWalking", true);
        while (Vector2.Distance(npcDulceMaria.position, puntoDeEncuentro.position) > 0.1f)
        {
            npcDulceMaria.position = Vector2.MoveTowards(npcDulceMaria.position, puntoDeEncuentro.position, velocidadNPC * Time.deltaTime);
            yield return null;
        }

        // 3. Llega a la meta
        animDulceMaria.SetBool("isWalking", false);
        yield return new WaitForSeconds(0.5f);

        // 4. Inicia la Conversación Dinámica
        textoDialogo.text = ""; // Limpiamos texto viejo antes de mostrar el panel
        yield return StartCoroutine(FadePanel(true)); // Animación de aparición (Fade-In)

        foreach (LineaDialogo linea in lineasDeDialogo)
        {
            bool hablaNPC = linea.hablante.Contains("Dulce") || linea.hablante.Contains("Maria");
            animDulceMaria.SetBool("isTalking", hablaNPC);

            yield return StartCoroutine(EscribirTexto(linea.hablante, linea.texto));

            yield return new WaitForSeconds(tiempoParaLeer);
        }

        // 5. Fin del diálogo
        animDulceMaria.SetBool("isTalking", false);
        yield return StartCoroutine(FadePanel(false)); // Animación de desaparición (Fade-Out)

        // Volteamos a Amada para que mire hacia Dulce María
        Vector3 escala = animAmada.transform.localScale;
        escala.x = Mathf.Abs(escala.x) * -1f;
        animAmada.transform.localScale = escala;

        // 6. El Intercambio con tiempos exactos
        animAmada.SetTrigger("GiveFlower");
        yield return new WaitForSeconds(3.0f);

        animDulceMaria.SetTrigger("ReceiveFlower");
        yield return new WaitForSeconds(1.3f);

        // 7. Fin del intercambio
        animAmada.SetBool("NPC_Interacted", true);
        yield return null;

        // 8. Liberar a Amada
        amadaMovement.enabled = true;
    }

    private IEnumerator EscribirTexto(string hablante, string textoFinal)
    {
        string colorHex = hablante.Contains("Amada") ? "#000000" : "#000000";
        textoDialogo.text = $"<color={colorHex}><b>{hablante}:</b></color> ";

        foreach (char letra in textoFinal.ToCharArray())
        {
            textoDialogo.text += letra;
            yield return new WaitForSeconds(velocidadEscritura);
        }
    }

    // NUEVA FUNCIONALIDAD: Controla la opacidad suavemente
    private IEnumerator FadePanel(bool fadeIn)
    {
        if (fadeIn)
        {
            panelDialogo.style.opacity = 0f;
            panelDialogo.style.display = DisplayStyle.Flex;
        }

        float timer = 0f;
        float startOpacity = fadeIn ? 0f : 1f;
        float endOpacity = fadeIn ? 1f : 0f;

        while (timer < tiempoFade)
        {
            timer += Time.deltaTime;
            // Interpola el valor de 0 a 1 (o de 1 a 0) basado en el tiempo
            panelDialogo.style.opacity = Mathf.Lerp(startOpacity, endOpacity, timer / tiempoFade);
            yield return null;
        }

        // Aseguramos el valor final exacto
        panelDialogo.style.opacity = endOpacity;

        if (!fadeIn)
        {
            panelDialogo.style.display = DisplayStyle.None;
        }
    }
}