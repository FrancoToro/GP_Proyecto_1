using System;
using UnityEngine;

public class scr_DiamondSquare : MonoBehaviour
{
    private System.Random rnd = new System.Random();
    private int w, h;

    // Método principal: genera el heightmap
    public float[,] Generate(int width, int height, int radius, float amplitude)
    {
        w = width;
        h = height;

        float[,] heightmap = InitializeArray(w, h);
        heightmap = DiamondSquare(heightmap, radius, amplitude);

        return heightmap;
    }

    // Inicializa el array con valores aleatorios en todos los bordes
    // Ahora los valores pueden ser positivos o negativos para mayor variedad
    private float[,] InitializeArray(int width, int height)
    {
        float[,] nArr = new float[height, width];

        // Inicializa todo el array en cero
        for (int x = 0; x < w; x++)
            for (int y = 0; y < h; y++)
                nArr[x, y] = 0;

        // Esquinas aleatorias
        nArr[0, 0] = RandomAmplitude();
        nArr[width - 1, 0] = RandomAmplitude();
        nArr[0, height - 1] = RandomAmplitude();
        nArr[width - 1, height - 1] = RandomAmplitude();

        return nArr;
    }

    // Genera un valor aleatorio entre -amplitude y +amplitude, pero nunca menor a -15
    private float RandomAmplitude(float amplitude = 10f)
    {
        float val = (float)(rnd.NextDouble() * 2.0 - 1.0) * amplitude;
        return Mathf.Max(val, -15f); // Limita el mínimo a -15
    }

    // Algoritmo recursivo
    private float[,] DiamondSquare(float[,] hm, int rad, float amp)
    {
        hm = Squares(hm, rad, amp);
        hm = Diamonds(hm, rad, amp);

        return (rad / 2 >= 1) ? DiamondSquare(hm, rad / 2, amp * 0.5f) : hm;
    }

    // Square step
    private float[,] Squares(float[,] hm, int rad, float amp)
    {
        for (int x = rad; x < w; x += (rad * 2))
            for (int y = rad; y < h; y += (rad * 2))
                hm = SquareStep(hm, x, y, rad, amp);

        return hm;
    }

    // Diamond step
    private float[,] Diamonds(float[,] hm, int rad, float amp)
    {
        int yIteration = 0;
        for (int y = 0; y < h; y += rad)
        {
            int shift = (yIteration % 2 == 0) ? rad : 0;
            for (int x = shift; x < w; x += (rad * 2))
                hm = DiamondStep(hm, x, y, rad, amp);

            yIteration++;
        }
        return hm;
    }

    // Calcula un punto central
    private float[,] SquareStep(float[,] hm, int x, int y, int rad, float amp)
    {
        // Calcula el promedio de las esquinas y suma un valor aleatorio limitado
        float rand = (float)(rnd.NextDouble() * 2.0 - 1.0) * amp;
        rand = Mathf.Max(rand, -5f); // Limita el mínimo a -5

        float nHeight = Average4(
            hm[Math.Max(x - rad, 0), Math.Max(y - rad, 0)],
            hm[Math.Max(x - rad, 0), Math.Min(y + rad, h - 1)],
            hm[Math.Min(x + rad, w - 1), Math.Max(y - rad, 0)],
            hm[Math.Min(x + rad, w - 1), Math.Min(y + rad, h - 1)]
        ) + rand;

        hm[x, y] = nHeight;
        return hm;
    }

    // Calcula un punto en el borde
    private float[,] DiamondStep(float[,] hm, int x, int y, int rad, float amp)
    {
        // Calcula el promedio de los bordes y suma un valor aleatorio limitado
        float rand = (float)(rnd.NextDouble() * 2.0 - 1.0) * amp;
        rand = Mathf.Max(rand, -5f); // Limita el mínimo a -5

        float nHeight = Average4(
            hm[Math.Max(x - rad, 0), y],
            hm[Math.Min(x + rad, w - 1), y],
            hm[x, Math.Max(y - rad, 0)],
            hm[x, Math.Min(y + rad, h - 1)]
        ) + rand;

        hm[x, y] = nHeight;
        return hm;
    }

    // Promedia 4 valores
    private float Average4(float a, float b, float c, float d)
    {
        return (a + b + c + d) * 0.25f;
    }
}