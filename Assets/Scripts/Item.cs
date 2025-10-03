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
    private Vector2Int resourceCellcoord;
    private Cell resourceCell;

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

    public Vector2Int GetResourceCell()
    {
        return resourceCellcoord;
    }

    public void SetResourceGatheringAnimationActive(bool active, float speedMultiplier = 0)
    {
        resourceCell.SetAnimationActive(active, speedMultiplier);
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
        int random = Random.Range(0, shapeData.Size());
        int length = shapeData.shape.GetLength(0);

        for (int x = 0, i = 0; x < shapeData.shape.GetLength(0); x++)
        {
            for (int y = 0; y < shapeData.shape.GetLength(1); y++)
            {
                if (shapeData.shape[x, y] == 1)
                {
                    Vector3 position = new Vector3((x - length / 2) * cellSize, (y - length / 2) * cellSize, 0f);
                    cells[i].GetComponent<Cell>().Init(color, position, cellSize);

                    if (i == random)
                    {
                        SetResourceCell(new Vector2Int(x, y), position, cells[i].gameObject);
                    }
                    i++;
                }
            }
        }
    }

    private void SetResourceCell(Vector2Int coord, Vector3 position, GameObject cell) {

        resourceCellcoord = coord;
        position.z = -0.0001f;
        resourceIcon.localPosition = position;
        resourceCell = cell.GetComponent<Cell>();
    }
}
