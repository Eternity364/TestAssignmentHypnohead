using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ShapesManager : MonoBehaviour
{
    [SerializeField] private ShapeSettingBlock shapeSettingBlockPrefab;
    [SerializeField] Transform parent;

    private List<ShapeSettingBlock> blocks = new List<ShapeSettingBlock>();

    private string SavePath => Path.Combine(Application.persistentDataPath, "shapes_save.json");

    [Serializable]
    private class SerializableShape
    {
        public int rows;
        public int cols;
        public int[] flat;

        public static SerializableShape FromShapeData(ShapeData sd)
        {
            int r = sd.shape.GetLength(0);
            int c = sd.shape.GetLength(1);
            var s = new SerializableShape { rows = r, cols = c, flat = new int[r * c] };
            for (int y = 0; y < r; y++)
                for (int x = 0; x < c; x++)
                    s.flat[y * c + x] = sd.shape[y, x];
            return s;
        }

        public ShapeData ToShapeData()
        {
            var sd = new ShapeData();
            sd.shape = new int[rows, cols];
            for (int y = 0; y < rows; y++)
                for (int x = 0; x < cols; x++)
                    sd.shape[y, x] = flat[y * cols + x];
            return sd;
        }
    }

    [Serializable]
    private class SaveFile
    {
        public List<SerializableShape> shapes = new List<SerializableShape>();
    }

    private void Start()
    {
        SaveFile loaded = null;
        if (File.Exists(SavePath))
        {
            try
            {
                string json = File.ReadAllText(SavePath);
                loaded = JsonUtility.FromJson<SaveFile>(json);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Failed to read save file: {e.Message}");
                loaded = null;
            }
        }

        for (int i = 0; i < 6; i++)
        {
            ShapeSettingBlock block = Instantiate(shapeSettingBlockPrefab, parent);
            ShapeData data;
            if (loaded != null && i < loaded.shapes.Count)
            {
                data = loaded.shapes[i].ToShapeData();
            }
            else
            {
                data = ShapeStorage.GetRandom();
            }

            block.Init(data, i + 1);
            block.transform.localPosition = block.transform.localPosition + new Vector3(3.55f, 0, 0) * i;
            blocks.Add(block);
        }
    }

    public void Save()
    {
        var file = new SaveFile();
        foreach (var b in blocks)
        {
            var sd = b.Data;
            file.shapes.Add(SerializableShape.FromShapeData(sd));
        }

        try
        {
            string json = JsonUtility.ToJson(file);
            File.WriteAllText(SavePath, json);
            Debug.Log($"Shapes saved to {SavePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save shapes: {e.Message}");
        }

        List<ShapeData> list = new();
        for (int i = 0; i < blocks.Count; i++)
        {
            list.Add(blocks[i].Data);
        }
        ShapeStorage.SetShapeDataList(list);
    }
}
