using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

// Esta estructura nos permite configurar cada línea en el Inspector
[System.Serializable]
public struct LineaDialogo
{
    public string hablante; // "Dulce María" o "Amada"
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

    [Header("Guion de la Escena")]
    public LineaDialogo[] lineasDeDialogo; // Ahora es una lista de líneas

    private VisualElement panelDialogo;
    private Label textoDialogo;

    private void Awake()
    {
        var root = uiDocument.rootVisualElement;
        panelDialogo = root.Q<VisualElement>("ContenedorDialogo");
        textoDialogo = root.Q<Label>("TextoDialogo");

        if (panelDialogo != null) panelDialogo.style.display = DisplayStyle.None;
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
        panelDialogo.style.display = DisplayStyle.Flex;

        // Este bucle reproduce todas las líneas que configures en el Inspector
        foreach (LineaDialogo linea in lineasDeDialogo)
        {
            // Detecta si habla Dulce María para activar su animación
            bool hablaNPC = linea.hablante.Contains("Dulce") || linea.hablante.Contains("Maria");
            animDulceMaria.SetBool("isTalking", hablaNPC);

            yield return StartCoroutine(EscribirTexto(linea.hablante + ": " + linea.texto));

            yield return new WaitForSeconds(tiempoParaLeer);
        }

        // 5. Fin del diálogo
        animDulceMaria.SetBool("isTalking", false);
        panelDialogo.style.display = DisplayStyle.None;

        // 6. El Intercambio
        animAmada.SetTrigger("GiveFlower");

        // Esperamos a que la mano de Amada se extienda (ajusta este valor según tu animación)
        yield return new WaitForSeconds(0.6f);

        animDulceMaria.SetTrigger("ReceiveFlower");
        yield return new WaitForSeconds(0.5f); // Pequeña pausa dramática para que se vea el intercambio

        animAmada.SetBool("NPC_Interacted", true);

        // 7. Liberar a Amada
        amadaMovement.enabled = true;
    }

    private IEnumerator EscribirTexto(string textoFinal)
    {
        textoDialogo.text = "";
        foreach (char letra in textoFinal.ToCharArray())
        {
            textoDialogo.text += letra;
            yield return new WaitForSeconds(velocidadEscritura);
        }
    }
}