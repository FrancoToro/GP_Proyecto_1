using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Terrain))]
public class DungeonTerrainGenerator : MonoBehaviour
{
    [Header("Terreno")]
    public int width = 256;
    public int height = 256;
    public int radius = 32;
    public float amplitude = 20f;
    public float treeHeightThreshold = 0.7f; // normalizado (0-1)

    [Header("Dungeon")]
    public int numRooms = 5;
    public int roomRadius = 8;

    [Header("Árboles")]
    public GameObject treePrefab; // modelo de árbol opcional
    public int maxTrees = 200;

    private scr_DiamondSquare diamondSquare;
    private List<Vector2Int> roomCenters = new List<Vector2Int>();
    private Terrain terrain;
    private TerrainData terrainData;

    void Start()
    {
        diamondSquare = GetComponent<scr_DiamondSquare>();
        terrain = GetComponent<Terrain>();
        terrainData = terrain.terrainData;

        GenerateDungeonTerrain();
    }

    void Update()
    {
        // Si presionas la tecla T, se regenera el terreno
        if (Input.GetKeyDown(KeyCode.T))
        {
            // Limpia centros de habitaciones previas
            roomCenters.Clear();

            // Elimina árboles instanciados previamente
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            // Regenerar todo
            GenerateDungeonTerrain();
        }
    }

    /// <summary>
    /// Genera el terreno de la mazmorra con heightmap, salas, pasillos, texturas y árboles.
    /// </summary>
    void GenerateDungeonTerrain()
    {
        // 1. Generar heightmap normalizado
        float[,] rawHeights = diamondSquare.Generate(width, height, radius, amplitude);
        float[,] heights = new float[height, width];
        float min = float.MaxValue, max = float.MinValue;

        foreach (float v in rawHeights)
        {
            if (v < min) min = v;
            if (v > max) max = v;
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
                heights[y, x] = Mathf.InverseLerp(min, max, rawHeights[x, y]);
        }

        // 2. Cavar salas y pasillos
        bool[,] dungeonMap = new bool[width, height];
        GenerateRooms(dungeonMap);
        ConnectRooms(dungeonMap);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (dungeonMap[x, y])
                    heights[y, x] = 0f; // plano (piso de la mazmorra)
            }
        }

        // 3. Aplicar heightmap
        terrainData.heightmapResolution = width + 1;
        terrainData.size = new Vector3(width, amplitude, height);
        terrainData.SetHeights(0, 0, heights);

        // 4. Instanciar árboles
        SpawnTrees(dungeonMap, heights);

        // 5. Pintar texturas
        PaintDungeonTextures(terrainData, dungeonMap);
    }

    /// <summary>
    /// Instancia árboles en zonas altas fuera de la mazmorra.
    /// </summary>
    void SpawnTrees(bool[,] dungeonMap, float[,] heights)
    {
        int currentTrees = 0;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (!dungeonMap[i, j])
                {
                    float h = heights[i, j];
                    if (h > treeHeightThreshold && Random.value > 0.85f && currentTrees < maxTrees)
                    {
                        Instantiate(treePrefab, new Vector3(i, h, j), Quaternion.identity, transform);
                        currentTrees++;
                    }
                }
            }
        }
    }

    void GenerateRooms(bool[,] map)
    {
        for (int i = 0; i < numRooms; i++)
        {
            int cx = Random.Range(roomRadius, width - roomRadius);
            int cy = Random.Range(roomRadius, height - roomRadius);
            roomCenters.Add(new Vector2Int(cx, cy));

            for (int x = -roomRadius; x <= roomRadius; x++)
            {
                for (int y = -roomRadius; y <= roomRadius; y++)
                {
                    int rx = cx + x;
                    int ry = cy + y;
                    if (rx >= 0 && ry >= 0 && rx < width && ry < height)
                        map[rx, ry] = true;
                }
            }
        }
    }

    void ConnectRooms(bool[,] map)
    {
        int corridorWidth = Mathf.Max(2, roomRadius / 2);
        for (int i = 0; i < roomCenters.Count - 1; i++)
        {
            Vector2Int start = roomCenters[i];
            Vector2Int end = roomCenters[i + 1];

            for (int x = Mathf.Min(start.x, end.x); x <= Mathf.Max(start.x, end.x); x++)
            {
                for (int dy = -corridorWidth; dy <= corridorWidth; dy++)
                {
                    int y = start.y + dy;
                    if (x >= 0 && y >= 0 && x < width && y < height)
                        map[x, y] = true;
                }
            }

            for (int y = Mathf.Min(start.y, end.y); y <= Mathf.Max(start.y, end.y); y++)
            {
                for (int dx = -corridorWidth; dx <= corridorWidth; dx++)
                {
                    int x = end.x + dx;
                    if (x >= 0 && y >= 0 && x < width && y < height)
                        map[x, y] = true;
                }
            }
        }
    }

    /// <summary>
    /// Pinta el alphamap del terreno con 3 texturas: suelo, pared y exterior.
    /// </summary>
    void PaintDungeonTextures(TerrainData terrainData, bool[,] dungeonMap)
    {
        int width = terrainData.alphamapWidth;
        int height = terrainData.alphamapHeight;
        int layers = terrainData.terrainLayers.Length;

        if (layers < 3) return; // Se requieren 3 Terrain Layers

        float[,,] alphamaps = new float[width, height, layers];

        int mapW = dungeonMap.GetLength(0);
        int mapH = dungeonMap.GetLength(1);
        int heightmapW = terrainData.heightmapResolution;
        int heightmapH = terrainData.heightmapResolution;
        float[,] heights = terrainData.GetHeights(0, 0, heightmapW, heightmapH);

        float floorHeightThreshold = 0.01f; // Umbral para considerar suelo plano

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float normX = (float)x / (width - 1);
                float normY = (float)y / (height - 1);

                int mapX = Mathf.Clamp(Mathf.RoundToInt(normX * (mapW - 1)), 0, mapW - 1);
                int mapY = Mathf.Clamp(Mathf.RoundToInt(normY * (mapH - 1)), 0, mapH - 1);

                int hX = Mathf.Clamp(Mathf.RoundToInt(normX * (heightmapW - 1)), 0, heightmapW - 1);
                int hY = Mathf.Clamp(Mathf.RoundToInt(normY * (heightmapH - 1)), 0, heightmapH - 1);

                alphamaps[y, x, 0] = 0f;
                alphamaps[y, x, 1] = 0f;
                alphamaps[y, x, 2] = 0f;

                bool isFloor = dungeonMap[mapX, mapY];
                float heightValue = heights[hY, hX];

                if (isFloor && heightValue < floorHeightThreshold)
                {
                    alphamaps[y, x, 0] = 1f; // Suelo mazmorra
                    continue;
                }

                bool isWall = false;
                for (int dx = -1; dx <= 1 && !isWall; dx++)
                {
                    for (int dy = -1; dy <= 1 && !isWall; dy++)
                    {
                        int nx = mapX + dx;
                        int ny = mapY + dy;
                        if (nx >= 0 && ny >= 0 && nx < mapW && ny < mapH)
                        {
                            if (dungeonMap[nx, ny])
                                isWall = true;
                        }
                    }
                }

                if (isWall)
                    alphamaps[y, x, 1] = 1f; // Pared
                else
                    alphamaps[y, x, 2] = 1f; // Exterior
            }
        }

        terrainData.SetAlphamaps(0, 0, alphamaps);
    }
}
