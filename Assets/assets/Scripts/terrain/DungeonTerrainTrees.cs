using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Terrain))]
public class DungeonTerrainTrees : MonoBehaviour
{
    public GameObject treePrefab; // el prefab de árbol que quieres usar

    private Terrain terrain;
    private TerrainData terrainData;

    void Start()
    {
        terrain = GetComponent<Terrain>();
        terrainData = terrain.terrainData;

        // 1. Crear TreePrototype con el prefab
        TreePrototype[] prototypes = new TreePrototype[1];
        prototypes[0] = new TreePrototype();
        prototypes[0].prefab = treePrefab;
        terrainData.treePrototypes = prototypes;

        // 2. Colocar árboles de prueba
        List<TreeInstance> trees = new List<TreeInstance>();

        for (int i = 0; i < 50; i++)
        {
            float x = Random.value; // normalizado 0–1
            float z = Random.value;
            float y = terrainData.GetInterpolatedHeight(x, z) / terrainData.size.y;

            TreeInstance tree = new TreeInstance
            {
                prototypeIndex = 0,
                position = new Vector3(x, y, z),
                widthScale = Random.Range(0.8f, 1.2f),
                heightScale = Random.Range(0.8f, 1.5f),
                color = Color.white,
                lightmapColor = Color.white
            };
            trees.Add(tree);
        }

        terrainData.treeInstances = trees.ToArray();
    }
}