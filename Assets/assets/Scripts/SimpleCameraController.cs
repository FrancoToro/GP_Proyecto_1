using UnityEngine;

public class SimpleCameraController : MonoBehaviour
{
    public float speed = 10f;
    private Rigidbody rb;

    [Header("Vista automática")]
    public Terrain terrain;       // referencia al terreno
    public float viewAngle = 45f; // ángulo hacia abajo
    public float heightFactor = 1.5f; // cuánto más alto que el tamaño del terreno
    public float distanceFactor = 1.0f; // qué tan lejos (en Z) respecto al tamaño del terreno

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.freezeRotation = true;
        }
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            gameObject.AddComponent<CapsuleCollider>();
        }

        if (terrain != null)
        {
            PositionToViewTerrain();
        }
    }

    void Update()
    {
        float h = Input.GetAxis("Horizontal"); // A/D o flechas
        float v = Input.GetAxis("Vertical");   // W/S o flechas
        float y = 0f;
        if (Input.GetKey(KeyCode.E)) y += 1f;
        if (Input.GetKey(KeyCode.Q)) y -= 1f;

        Vector3 move = new Vector3(h, y, v) * speed * Time.deltaTime;
        rb.MovePosition(transform.position + move);

        // Zoom dinámico con Z (acercar) y X (alejar)
        if (Input.GetKey(KeyCode.Z))
        {
            distanceFactor = Mathf.Max(0.1f, distanceFactor - 0.5f * Time.deltaTime);
            if (terrain != null) PositionToViewTerrain();
        }
        if (Input.GetKey(KeyCode.X))
        {
            distanceFactor = Mathf.Min(10f, distanceFactor + 0.5f * Time.deltaTime);
            if (terrain != null) PositionToViewTerrain();
        }

        // Reacomodar con la tecla R
        if (Input.GetKeyDown(KeyCode.R) && terrain != null)
        {
            PositionToViewTerrain();
        }
    }

    /// <summary>
    /// Mueve la cámara para ver el terreno completo desde arriba y en ángulo.
    /// </summary>
    public void PositionToViewTerrain()
    {
        Vector3 terrainSize = terrain.terrainData.size;
        Vector3 terrainCenter = terrain.transform.position + terrainSize / 2f;

        // Determinar la altura según el tamaño mayor (ancho o largo del terreno)
    float maxDimension = Mathf.Max(terrainSize.x, terrainSize.z);
    float height = maxDimension * heightFactor;
    float distance = maxDimension * distanceFactor;

    // Colocar la cámara sobre el centro, a cierta altura, y a una distancia ajustable
    Vector3 newPos = terrainCenter + new Vector3(0, height, -distance);
    transform.position = newPos;
    transform.LookAt(terrainCenter + Vector3.up * (terrainSize.y / 2f));

    // Inclinar la cámara en el ángulo definido
    transform.rotation = Quaternion.Euler(viewAngle, transform.rotation.eulerAngles.y, 0f);
    }
}
