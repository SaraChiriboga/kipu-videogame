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

    [Header("Guion de la Escena")]
    public LineaDialogo[] lineasDeDialogo;

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
        foreach (LineaDialogo linea in lineasDeDialogo)
        {
            bool hablaNPC = linea.hablante.Contains("Dulce") || linea.hablante.Contains("Maria");
            animDulceMaria.SetBool("isTalking", hablaNPC);
            yield return StartCoroutine(EscribirTexto(linea.hablante + ": " + linea.texto));
            yield return new WaitForSeconds(tiempoParaLeer);
        }

        // 5. Fin del diálogo
        animDulceMaria.SetBool("isTalking", false);
        panelDialogo.style.display = DisplayStyle.None;

        // Volteamos a Amada para que mire hacia Dulce María
        Vector3 escala = animAmada.transform.localScale;
        escala.x = Mathf.Abs(escala.x) * -1f;
        animAmada.transform.localScale = escala;

        // 6. El Intercambio con tiempos exactos
        animAmada.SetTrigger("GiveFlower");

        // Esperamos exactamente 3.0 segundos para que Amada termine su animación
        yield return new WaitForSeconds(3.0f);

        animDulceMaria.SetTrigger("ReceiveFlower");

        // Esperamos 1.3 segundos para que Dulce María reciba la flor. 
        // (Nota: Si el 1:30 se refería a 1 segundo y 30 fotogramas a 60FPS, cambia este valor a 1.5f)
        yield return new WaitForSeconds(1.3f);

        // 7. Fin del intercambio
        animAmada.SetBool("NPC_Interacted", true);

        // Le damos 1 frame de cortesía al Animator para que procese el cambio a IdleAfterNPC
        yield return null;

        // 8. Liberar a Amada
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