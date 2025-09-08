using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class SimpleLSystemTurtle2 : MonoBehaviour
{
    [Header("Configuración del L-System")]
    [SerializeField] public string axiom = "F";
    [SerializeField] private string rule = "F[+F]F[-F*F]F[/F]";

    [Header("Generaciones")]
    [SerializeField, Range(1, 6)] public int minGenerations = 1;
    [SerializeField, Range(1, 10)] public int maxGenerations = 6;

    [Header("Ángulo")]
    [SerializeField, Range(0f, 90f)] public float minAngle = 20f;
    [SerializeField, Range(0f, 90f)] public float maxAngle = 30f;

    [Header("Líneas")]
    [SerializeField, Range(0.1f, 2f)] public float lineLength = 0.5f;
    [SerializeField, Range(0.01f, 0.2f)] public float lineWidth = 0.05f;

    [Header("Material")]
    [SerializeField] private Material lineMaterial;

    private string currentSentence;
    private LineRenderer lineRenderer;
    private float chosenAngleZ;
    private float chosenAngleX;
    private int chosenGenerations;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.material = lineMaterial;
        lineRenderer.useWorldSpace = true; // usar coordenadas globales

        // Valores aleatorios
        chosenAngleZ = Random.Range(minAngle, maxAngle);
        chosenAngleX = Random.Range(minAngle, maxAngle);
        chosenGenerations = Random.Range(minGenerations, maxGenerations + 1);

        GenerateSentence();
        DrawSentence();
    }

    [ContextMenu("Regenerar L-System Aleatorio")]
    public void RegenerateRandom()
    {
        chosenAngleZ = Random.Range(minAngle, maxAngle);
        chosenAngleX = Random.Range(minAngle, maxAngle);
        chosenGenerations = Random.Range(minGenerations, maxGenerations + 1);

        GenerateSentence();
        DrawSentence();
    }

    private void GenerateSentence()
    {
        currentSentence = axiom;

        for (int i = 0; i < chosenGenerations; i++)
        {
            string newSentence = "";
            foreach (char c in currentSentence)
            {
                if (c == 'F')
                    newSentence += rule;
                else
                    newSentence += c.ToString();
            }
            currentSentence = newSentence;
        }
    }

    private void DrawSentence()
    {
        Vector3 position = transform.position; // arranca desde donde se instanció
        float currentAngleZ = 90f;
        float currentAngleX = 90f;
        Stack<(Vector3 pos, float angleZ, float AngleX)> stack = new Stack<(Vector3, float, float)>();

        List<Vector3> points = new List<Vector3>();
        points.Add(position);

        foreach (char c in currentSentence)
        {
            switch (c)
            {
                case 'F':
                    position += Quaternion.Euler(0, currentAngleX, currentAngleZ) * Vector3.right * lineLength;
                    points.Add(position);
                    break;

                case '+':
                    currentAngleZ += chosenAngleZ;
                    break;

                case '-':
                    currentAngleZ -= chosenAngleZ;
                    break;

                case '*':
                    currentAngleX += chosenAngleX;
                    break;

                case '/':
                    currentAngleX -= chosenAngleX;
                    break;

                case '[':
                    stack.Push((position, currentAngleZ, currentAngleX));
                    break;

                case ']':
                    var state = stack.Pop();
                    position = state.pos;
                    currentAngleZ = state.angleZ;
                    currentAngleX = state.AngleX;
                    points.Add(position);
                    break;
            }
        }

        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
    }
}
