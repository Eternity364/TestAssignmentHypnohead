using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Item : MonoBehaviour, IClearable
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
    private Vector2Int resourceCellcoord = new Vector2Int(-1, -1);
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
        shapeData.Rotate(ref resourceCellcoord);
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

    public void Clear()
    {
        foreach (Transform cell in cells)
        {
            cell.GetComponent<Cell>().OnMouseClick -= ProcessClickOnThis;
            ObjectPool.Instance.ReturnObject(cell.gameObject);
        }
        cells.Clear();
        ObjectPool.Instance.ReturnObject(resourceIcon.gameObject);
        placed = false;
        resourceCellcoord = new Vector2Int(-1, -1);
        resourceCell = null;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
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
                    Transform cell = ObjectPool.Instance.GetObject(cellPrefab.gameObject).transform;
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
        if (resourceCellcoord != new Vector2Int(-1, -1))
            random = -1;
        int length = shapeData.shape.GetLength(0);

        for (int x = 0, i = 0; x < shapeData.shape.GetLength(0); x++)
        {
            for (int y = 0; y < shapeData.shape.GetLength(1); y++)
            {
                if (shapeData.shape[x, y] == 1)
                {
                    Vector3 position = new Vector3((x - length / 2) * cellSize, (y - length / 2) * cellSize, 0f);
                    Cell cellComp = cells[i].GetComponent<Cell>();
                    cellComp.Init(color, position, cellSize);

                    if (i == random || resourceCellcoord == new Vector2Int(x, y))
                    {
                        SetResourceCell(cellComp, new Vector2Int(x, y));
                    }
                    i++;
                }
            }
        }
    }

    private void SetResourceCell(Cell cell, Vector2Int cellCoord) {
        resourceCellcoord = cellCoord;
        resourceCell = cell;
        Vector3 position = cell.transform.localPosition;
        position.z = -0.0001f;
        resourceIcon.localPosition = position;
    }
}
