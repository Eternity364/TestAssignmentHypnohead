using TMPro;
using UnityEngine;

public class ShapeSettingBlock : MonoBehaviour
{
    public int size = 3;
    public float cellSize = 0.6f;
    public Transform tilePrefab;
    public Transform cellPrefab;
    public TextMeshProUGUI text;

    public ShapeData Data => data;

    private ShapeData data;

    public void Init(ShapeData data, int index)
    {
        this.data = data;
        text.text = index.ToString();

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                Transform tile = Instantiate(tilePrefab);
                tile.SetParent(transform);
                tile.localPosition = new Vector3(x * cellSize, y * cellSize, 0f);
                Transform cell = Instantiate(cellPrefab);
                cell.SetParent(transform);
                cell.localPosition = tile.localPosition;
                Cell cellComp = cell.GetComponent<Cell>();
                cellComp.SetVisible(this.data.shape[x, y] == 1);
                Vector2Int coord = new Vector2Int(x, y);
                cellComp.OnMouseClick += () =>
                {
                    if (this.data.shape[coord.x, coord.y] == 1)
                    {
                        if (this.data.Size() > 1)
                            this.data.shape[coord.x, coord.y] = 0;
                    }
                    else
                        this.data.shape[coord.x, coord.y] = 1;

                    cellComp.SetVisible(this.data.shape[coord.x, coord.y] == 1);
                };
            }
        }
    }
}
