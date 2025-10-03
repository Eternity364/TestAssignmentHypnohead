using System.Collections.Generic;
using UnityEngine;

public static class ShapeStorage
{
    private static List<int[,]> shapes = new List<int[,]>
    {
            new int[,]
                {
                    { 0, 1, 0},
                    { 0, 1, 0},
                    { 0, 1, 1},
                },
            new int[,]
                {
                    { 0, 1, 0},
                    { 0, 1, 0},
                    { 0, 1, 0},
                },
            new int[,]
                {
                    { 0, 0, 0},
                    { 0, 1, 0},
                    { 0, 0, 0},
                },
            new int[,]
                {
                    { 0, 1, 0},
                    { 1, 1, 0},
                    { 0, 1, 0},
                },
            new int[,]
                {
                    { 0, 0, 0},
                    { 1, 1, 0},
                    { 0, 1, 0},
                },
            new int[,]
                {
                    { 0, 0, 0},
                    { 1, 1, 0},
                    { 1, 1, 0},
                }
    };

    public static ShapeData GetRandom => new ShapeData { shape = shapes[Random.Range(0, shapes.Count)] };
}

public struct ShapeData
{
    public int[,] shape;

    public void Rotate()
    {
        int size = shape.GetLength(0);
        int[,] result = new int[size, size];

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                result[x, size - y - 1] = shape[y, x];
            }
        }

        shape = result;
    }
    
    public int Size()
    {
        int count = 0;
        int rows = shape.GetLength(0);
        int cols = shape.GetLength(1);
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                if (shape[y, x] == 1)
                    count++;
            }
        }
        return count;
    }
}


