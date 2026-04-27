using UnityEngine;

public class TriggerCambioAmbiente : MonoBehaviour
{
    public BackgroundLooper generador;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            
        }
    }
}