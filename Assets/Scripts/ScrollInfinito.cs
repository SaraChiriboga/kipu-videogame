using UnityEngine;

public class ScrollInfinito : MonoBehaviour
{
    // Ajusta esto según la velocidad de cada capa para el Parallax
    public float velocidadParallax;
    private Material mat;
    private Vector2 offset;

    void Start()
    {
        // El objeto debe tener un Mesh Renderer o Sprite Renderer
        mat = GetComponent<Renderer>().material;
    }

    void Update()
    {
        // Desplazamos la textura basándonos en el tiempo
        offset.x += velocidadParallax * Time.deltaTime;
        mat.mainTextureOffset = offset;
    }
}