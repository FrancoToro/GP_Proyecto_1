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
    public float treeHeightThreshold = 0.3f; // normalizado (0-1)

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
            for (int y = 0; y < height; y++)
                heights[y, x] = Mathf.InverseLerp(min, max, rawHeights[x, y]);

        // 2. Cavar salas
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

        terrainData.heightmapResolution = width + 1;
        terrainData.size = new Vector3(width, amplitude, height);
        terrainData.SetHeights(0, 0, heights);

        // 3. Colocar árboles del sistema de Terrain
        List<TreeInstance> treeInstances = new List<TreeInstance>();
        for (int i = 0; i < maxTrees; i++)
        {
            int tx = Random.Range(0, width);
            int ty = Random.Range(0, height);
            float h = heights[ty, tx];

            if (!dungeonMap[tx, ty] && h > treeHeightThreshold)
            {
                TreeInstance tree = new TreeInstance
                {
                    position = new Vector3(
                        (float)tx / width,
                        h,
                        (float)ty / height
                    ),
                    prototypeIndex = 0,
                    widthScale = Random.Range(0.8f, 1.2f),
                    heightScale = Random.Range(0.8f, 1.5f),
                    color = Color.white,
                    lightmapColor = Color.white
                };
                treeInstances.Add(tree);
            }
        }

        TreePrototype[] prototypes = new TreePrototype[1];
        prototypes[0] = new TreePrototype();
        prototypes[0].prefab = treePrefab;
        terrainData.treePrototypes = prototypes;

    }

    void GenerateRooms(bool[,] map)
    {
        for (int i = 0; i < numRooms; i++)
        {
            int cx = Random.Range(roomRadius, width - roomRadius);
            int cy = Random.Range(roomRadius, height - roomRadius);
            roomCenters.Add(new Vector2Int(cx, cy));

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

    void ConnectRooms(bool[,] map)
    {
        for (int i = 0; i < roomCenters.Count - 1; i++)
        {
            Vector2Int start = roomCenters[i];
            Vector2Int end = roomCenters[i + 1];

            for (int x = Mathf.Min(start.x, end.x); x <= Mathf.Max(start.x, end.x); x++)
                map[x, start.y] = true;
            for (int y = Mathf.Min(start.y, end.y); y <= Mathf.Max(start.y, end.y); y++)
                map[end.x, y] = true;
        }
    }
}

