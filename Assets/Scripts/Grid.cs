using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Grid : MonoBehaviour
{
    [SerializeField] private CommonParameters commonParameters;
    [SerializeField] private ItemField itemField;
    [SerializeField] private ItemFactory itemFactory;
    [SerializeField] private Transform tilePrefab;
    [SerializeField] private int referenceSize = 8;
    [SerializeField] private int width = 8;
    [SerializeField] private int height = 8;
    [SerializeField] private List<ResourceZoneParameters> resourceZoneParameters;

    public UnityAction OnItemsNumberChanged;
    public IReadOnlyList<Item> Items => items.AsReadOnly();
    public Dictionary<Item, Vector2Int> ResourceCells => new Dictionary<Item, Vector2Int>(resourceCells);

    private List<Transform> tiles = new List<Transform>();
    // List of all items
    private List<Item> items = new List<Item>();
    // List of which item occupies which cell
    private List<Item> values = new List<Item>();
    private Dictionary<Item, Vector2Int> resourceCells = new Dictionary<Item, Vector2Int>();
    private List<ResourceModificator> resourceModificators = new List<ResourceModificator>();
    private List<Item> itemsToDestroy = new List<Item>();
    private float lastCellSize;
    private int lastWidth;
    private int lastHeight;
    private bool destroyBlock = false;

    [Serializable]
    private struct ResourceZoneParameters
    {
        public ResourceModificator modificator;
        public int width;
    }

    public void AddItem(Item item, Vector2Int cell)
    {
        items.Add(item);
        item.transform.SetParent(transform);
        item.SetPlacedStatus(true);
        if (cell == new Vector2Int(-1, -1))
            cell = GetCellUnderMouse();
        Vector2Int centralCell = item.GetCentralCell();

        int startX = cell.x - centralCell.x;
        int startY = cell.y - centralCell.y;

        item.transform.localPosition = GetCellPosition(new Vector2Int(startX, startY));

        for (int x = 0; x < item.Shape.shape.GetLength(0); x++)
        {
            for (int y = 0; y < item.Shape.shape.GetLength(1); y++)
            {
                if (item.Shape.shape[x, y] == 1)
                {
                    int gridX = startX + x;
                    int gridY = startY + y;

                    if (gridX < 0 || gridX >= width || gridY < 0 || gridY >= height)
                        continue;

                    int index = gridX * height + gridY;
                    if (values[index] != null)
                    {
                        RemoveItem(values[index], false, false);
                    }

                    if (item.GetResourceCell() == new Vector2Int(x, y))
                    {
                        resourceCells[item] = new Vector2Int(gridX, gridY);
                    }

                    values[index] = item;
                }
            }
        }
        float modificatorValue = GetResourceModificatorAtCell(resourceCells[item]).value;
        item.SetResourceGatheringAnimationActive(modificatorValue > 0, modificatorValue);
        OnItemsNumberChanged?.Invoke();
    }

    public Vector2Int GetItemCell(Item item)
    {
        return GetCellCoordFromPosition(item.transform.localPosition);
    }

    public Vector3 GetResourceCellPosition(Item item)
    {
        if (!resourceCells.ContainsKey(item)) return Vector3.zero;
        return GetCellPosition(resourceCells[item] - new Vector2Int(1, 1)) + transform.position;
    }

    public float GetCellSize()
    {
        float multiplicator = 1f;
        if (width > referenceSize || height > referenceSize)
        {
            multiplicator = (float)referenceSize / (float)Mathf.Max(width, height);
        }
        return commonParameters.CellSize * multiplicator;
    }

    public void RemoveItem(Item item, bool destroy, bool callback = true, bool returnToItemField = true)
    {
        item.SetResourceGatheringAnimationActive(false, 0);
        if (destroy)
            ScheduleItemForDestroying(item);
        else
        {
            RemoveItemFromGrid(item, callback);
            if (returnToItemField)
                itemField.Add(item);
        }
    }

    public ResourceModificator GetResourceModificatorAtCell(Vector2Int cell)
    {
        if (cell.x < 0 || cell.x >= width || cell.y < 0 || cell.y >= height)
            return new ResourceModificator();

        return resourceModificators[cell.x * height + cell.y];
    }
    
    public bool DoesItemFitIntoGrid(Item item)
    {
        Vector2Int cell = GetCellUnderMouse();

        if (cell.x == -1 || cell.y == -1)
            return false;

        for (int x = 0; x < item.Shape.shape.GetLength(0); x++)
        {
            for (int y = 0; y < item.Shape.shape.GetLength(1); y++)
            {
                if (item.Shape.shape[x, y] == 1)
                {
                    int gridX = cell.x + x - 1;
                    int gridY = cell.y + y - 1;

                    if (gridX < 0 || gridX >= width || gridY < 0 || gridY >= height)
                        return false;
                }
            }
        }

        return true;
    }

    public void IterateThroughResourceCells(Action<Item> Action)
    {
        destroyBlock = true;
        foreach (var kvp in resourceCells)
        {
            Action(kvp.Key);
        }
        destroyBlock = false;
    }

    private void ScheduleItemForDestroying(Item item)
    {
        itemsToDestroy.Add(item);
    }

    private void DestroyItems()
    {
        foreach (Item item in itemsToDestroy)
        {
            itemFactory.Destroy(item);
            RemoveItemFromGrid(item);
            itemField.CreateNewItem();
        }
        itemsToDestroy.Clear();
    }

    private void RemoveItemFromGrid(Item item, bool callback = true)
    {
        items.Remove(item);
        resourceCells.Remove(item);
        for (int i = 0; i < values.Count; i++)
        {
            if (values[i] == item)
                values[i] = null;
        }
        if (callback)
            OnItemsNumberChanged?.Invoke();
    }

    private Vector3 GetCellUnderMousePosition()
    {
        Vector2Int cell = GetCellUnderMouse();
        if (cell.x == -1 || cell.y == -1)
            return Vector3.zero;

        return GetCellPosition(cell);
    }

    private Vector3 GetCellPosition(Vector2Int cell)
    {
        return new Vector3((cell.x + 1) * GetCellSize(), (cell.y + 1) * GetCellSize(), 0f);
    }

    private Vector2Int GetCellCoordFromPosition(Vector3 pos) {
        int x = Mathf.FloorToInt(pos.x / GetCellSize() + 0.5f);
        int y = Mathf.FloorToInt(pos.y / GetCellSize() + 0.5f);

        if (x < 0 || x >= width || y < 0 || y >= height)
            return new Vector2Int(-1, -1);

        return new Vector2Int(x, y);
    }

    private Vector2Int GetCellUnderMouse()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        return GetCellCoordFromPosition(mouseWorldPos);
    }

    private void Update()
    {
        CreateTiles();
        SetGridParameters();

        if (Input.GetMouseButtonUp(0))
        {
            Vector2Int cell = GetCellUnderMouse();
            print(cell);
        }

        if (!destroyBlock)
            DestroyItems();
    }

    private void CreateTiles()
    {
        if (width == lastWidth && height == lastHeight)
            return;

        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }

        tiles.Clear();
        values.Clear();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Transform tile = Instantiate(tilePrefab);
                tile.SetParent(transform);
                tiles.Add(tile);
                values.Add(null);
            }
        }

        lastWidth = width;
        lastHeight = height;

        PopulateResourceModificators();
        SetGridParameters(false);
    }

    private void SetGridParameters(bool useConditions = true)
    {
        if (useConditions && (GetCellSize() == lastCellSize))
            return;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Transform tile = tiles[x * height + y];

                SpriteRenderer sr = tile.GetComponent<SpriteRenderer>();
                if (sr != null && sr.sprite != null)
                {
                    sr.color = resourceModificators[x * height + y].color;
                    float spritePixels = sr.sprite.rect.width;
                    float spriteUnits = spritePixels / sr.sprite.pixelsPerUnit;
                    float scale = GetCellSize() / spriteUnits;
                    tile.localPosition = new Vector3(x * GetCellSize(), y * GetCellSize(), 0f);
                    tile.localScale = new Vector3(scale, scale, 1f);
                }
            }
        }

        lastCellSize = GetCellSize();
    }

    private void PopulateResourceModificators()
    {
        resourceModificators.Clear();
        int tilesCount = width * height;
        resourceModificators.Capacity = tilesCount;

        List<int> cum = new List<int>();
        int sum = 0;
        for (int i = 0; i < resourceZoneParameters.Count; i++)
        {
            int w = (i < resourceZoneParameters.Count) ? Mathf.Max(0, resourceZoneParameters[i].width) : 0;
            sum += w;
            cum.Add(sum);
        }

        int rings = (Mathf.Min(width, height) + 1) / 2;

        bool zeroCoverage = (cum.Count == 0 || cum[cum.Count - 1] == 0);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int ring = Mathf.Min(Mathf.Min(x, y), Mathf.Min(width - 1 - x, height - 1 - y));
                int typeIndex = resourceZoneParameters.Count - 1;

                if (!zeroCoverage)
                {
                    for (int i = 0; i < cum.Count; i++)
                    {
                        if (ring < cum[i])
                        {
                            typeIndex = i;
                            break;
                        }
                    }
                }

                ResourceModificator chosen = resourceZoneParameters[typeIndex].modificator;
                resourceModificators.Add(chosen);
            }
        }
    }
}

[Serializable]
public struct ResourceModificator
{
    public Color color;
    public float value;
}
