using UnityEngine;
using System.Collections.Generic;

public class DungeonGenerator : MonoBehaviour
{
    [Header("Terreno")]
    public int width = 64;
    public int height = 64;
    public int radius = 16;
    public float amplitude = 20f;
    public float treeHeightThreshold = 5f;

    [Header("Prefabs")]
    public GameObject floorPrefab;
    public GameObject wallPrefab;
    public GameObject roomPrefab;
    public GameObject treePrefab;

    private scr_DiamondSquare diamondSquare;
    private List<Vector2Int> roomCenters = new List<Vector2Int>();

    void Start()
    {
        diamondSquare = GetComponent<scr_DiamondSquare>();
        GenerateDungeon();
    }

    void GenerateDungeon()
    {
        float[,] heightmap = diamondSquare.Generate(width, height, radius, amplitude);
        bool[,] dungeonMap = new bool[width, height];

        // 1. Generar salas
        GenerateRooms(dungeonMap, 5, 6);

        // 2. Conectar salas con pasillos
        ConnectRoomsWithCorridors(dungeonMap);

        // 3. Instanciar visualmente
        DrawDungeon(dungeonMap, heightmap);
    }

    void GenerateRooms(bool[,] map, int numRooms, int roomRadius)
    {
        for (int i = 0; i < numRooms; i++)
        {
            int cx = Random.Range(roomRadius, width - roomRadius);
            int cy = Random.Range(roomRadius, height - roomRadius);
            roomCenters.Add(new Vector2Int(cx, cy));

            // Crear una sala como un solo prefab escalado
            Vector3 roomPos = new Vector3(cx, 0, cy);
            GameObject room = Instantiate(roomPrefab, roomPos, Quaternion.identity, transform);
            room.transform.localScale = new Vector3(roomRadius * 2, 1, roomRadius * 2);

            // Marcar área como "camino/sala" en el mapa lógico
            for (int x = -roomRadius; x <= roomRadius; x++)
                for (int y = -roomRadius; y <= roomRadius; y++)
                {
                    int rx = cx + x;
                    int ry = cy + y;
                    if (rx >= 0 && ry >= 0 && rx < width && ry < height)
                        map[rx, ry] = true;
                }
        }
    }

    void ConnectRoomsWithCorridors(bool[,] map)
    {
        for (int i = 0; i < roomCenters.Count - 1; i++)
        {
            Vector2Int start = roomCenters[i];
            Vector2Int end = roomCenters[i + 1];

            // Pasillo en L: primero en X, luego en Y
            for (int x = Mathf.Min(start.x, end.x); x <= Mathf.Max(start.x, end.x); x++)
                map[x, start.y] = true;
            for (int y = Mathf.Min(start.y, end.y); y <= Mathf.Max(start.y, end.y); y++)
                map[end.x, y] = true;

            // Dibujar un piso largo como prefab escalado
            Vector3 corridorPos = new Vector3((start.x + end.x) / 2f, 0, (start.y + end.y) / 2f);
            GameObject corridor = Instantiate(floorPrefab, corridorPos, Quaternion.identity, transform);

            float corridorLength = Vector2Int.Distance(start, end);
            corridor.transform.localScale = new Vector3(corridorLength, 1, 1); // ancho fijo, largo variable
        }
    }

    void DrawDungeon(bool[,] map, float[,] heightmap)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (!map[x, y]) // no es sala ni pasillo
                {
                    float h = heightmap[x, y];
                    Vector3 elevatedPos = new Vector3(x, h, y);
                    Instantiate(wallPrefab, elevatedPos, Quaternion.identity, transform);

                    // Árboles en zonas altas
                    if (h > treeHeightThreshold && Random.value > 0.98f)
                    {
                        Vector3 jitter = new Vector3(Random.Range(-0.3f, 0.3f), 0, Random.Range(-0.3f, 0.3f));
                        Instantiate(treePrefab, elevatedPos + jitter, Quaternion.identity, transform);
                    }
                }
            }
        }
    }
}
