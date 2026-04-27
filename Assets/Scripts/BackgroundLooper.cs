using UnityEngine;

public class BackgroundLooper : MonoBehaviour
{
    [Header("Fondos")]
    public Transform background;      // El background original

    [Header("Configuración")]
    public float backgroundWidth = 20.86f;  // 15.36 * 1.358325

    private Camera _cam;
    private Transform _copy;

    void Start()
    {
        _cam = Camera.main;

        // Duplica el background completo (sprite + BoxCollider2D incluidos)
        _copy = Instantiate(background, background.parent);
        _copy.name = "background_copy";
        _copy.position = new Vector3(
            background.position.x + backgroundWidth,
            background.position.y,
            background.position.z
        );
    }

    void LateUpdate()
    {
        RepositionIfNeeded(background, _copy);
        RepositionIfNeeded(_copy, background);
    }

    void RepositionIfNeeded(Transform current, Transform other)
    {
        if (_cam.transform.position.x > current.position.x + backgroundWidth)
        {
            current.position = new Vector3(
                other.position.x + backgroundWidth,
                other.position.y,
                other.position.z
            );
        }
    }
}