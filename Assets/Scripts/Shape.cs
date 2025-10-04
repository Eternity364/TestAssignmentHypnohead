using System;
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

    private static List<ShapeData> shapeDataList = new List<ShapeData>();

    public static void SetShapeDataList(List<ShapeData> shapeDataList)
    {
        ShapeStorage.shapeDataList = shapeDataList;
    }

    public static ShapeData GetRandom()
    {
        if (shapeDataList.Count == 0)
            return new ShapeData { shape = shapes[UnityEngine.Random.Range(0, shapes.Count)] };
        else
            return shapeDataList[UnityEngine.Random.Range(0, shapeDataList.Count)];
    }
}

[Serializable]
public struct ShapeData
{
    public int[,] shape;

    public void Rotate(ref Vector2Int cellOfInterest)
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

        cellOfInterest = new Vector2Int(cellOfInterest.y, size - 1 - cellOfInterest.x);

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


