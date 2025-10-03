using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Item : MonoBehaviour
{
    [SerializeField] private Transform cellPrefab;

    private ShapeData shapeData;
    private Color color;
    private Transform resourceIcon;
    private ResourceType resourceType;
    private List<Transform> cells = new List<Transform>();
    private UnityAction<Item> OnMouseClick;
    private float cellSize;
    private bool placed = false;

    public ShapeData Shape => shapeData;
    public ResourceType ResourceType => resourceType;
    public int Size => shapeData.Size();
    public bool IsPlaced => placed;

    public void Setup(Color color, ShapeData shapeData, Transform resourceIcon, float cellSize, ResourceType resourceType, UnityAction<Item> OnMouseClick)
    {
        this.color = color;
        this.shapeData = shapeData;
        this.cellSize = cellSize;
        this.resourceIcon = resourceIcon;
        this.resourceType = resourceType;
        this.OnMouseClick = OnMouseClick;
        Init();
    }

    public void SetPlacedStatus(bool placed)
    {
        this.placed = placed;
    }

    public void Rotate()
    {
        shapeData.Rotate();
        SetPositions();
    }

    public void SetInputActive(bool isActive)
    {
        List<Collider2D> colliders = new List<Collider2D>(GetComponentsInChildren<Collider2D>());
        foreach (Collider2D collider in colliders)
        {
            collider.enabled = isActive;
        }
    }

    public Vector2Int GetCentralCell()
    {
        int length = shapeData.shape.GetLength(0);
        return new Vector2Int(length / 2, length / 2);
    }

    private void ProcessClickOnThis()
    {
        OnMouseClick?.Invoke(this);
    }

    private void Init()
    {
        for (int x = 0; x < shapeData.shape.GetLength(0); x++)
        {
            for (int y = 0; y < shapeData.shape.GetLength(1); y++)
            {
                if (shapeData.shape[x, y] == 1)
                {
                    Transform cell = Instantiate(cellPrefab);
                    cell.SetParent(transform);
                    cells.Add(cell);

                    cell.GetComponent<Cell>().OnMouseClick += ProcessClickOnThis;
                }
            }
        }

        Vector3 originalScale = resourceIcon.localScale;
        resourceIcon.localScale = originalScale * cellSize;
        resourceIcon.SetParent(transform);

        SetPositions();
    }

    private void SetPositions()
    {
        Vector3 resourceIconPosition = Vector3.zero;
        int length = shapeData.shape.GetLength(0);
        for (int x = 0, i = 0; x < shapeData.shape.GetLength(0); x++)
        {
            for (int y = 0; y < shapeData.shape.GetLength(1); y++)
            {
                if (shapeData.shape[x, y] == 1)
                {
                    cells[i].localPosition = new Vector3(x, y, 0);
                    SpriteRenderer sr = cells[i].GetComponent<SpriteRenderer>();
                    if (sr != null && sr.sprite != null)
                    {
                        sr.color = color;
                        float spritePixels = sr.sprite.rect.width;
                        float spriteUnits = spritePixels / sr.sprite.pixelsPerUnit;
                        float scale = cellSize / spriteUnits;
                        Vector3 position = new Vector3((x - length / 2) * cellSize, (y - length / 2) * cellSize, 0f);
                        cells[i].localPosition = position;
                        cells[i].localScale = new Vector3(scale, scale, 1f);

                        if (GetCentralCell() == new Vector2Int(x, y))
                            resourceIconPosition = position;
                    }
                    i++;
                }
            }
        }

        resourceIconPosition.z = -0.0001f;
        resourceIcon.localPosition = resourceIconPosition;
    }
}
