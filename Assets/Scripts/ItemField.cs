using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemField : MonoBehaviour
{
    [SerializeField] private ItemFactory itemFactory;
    [SerializeField] private Grid grid;
    [SerializeField] private PickedItemController pickedItemController;
    [SerializeField] private float width;
    [SerializeField] private float height;
    [SerializeField] private int count;

    private Rect constraints;
    private Item item;
    private int posCounter = 0;

    void Start()
    {
        constraints = new Rect(-width / 2, -height / 2, width, height);
        for (int i = 0; i < count; i++)
        {
            CreateNewItem();
        }
    }

    public void Add(Item item)
    {
        item.SetPlacedStatus(false);
        item.transform.SetParent(transform);
        item.transform.localPosition = GerRandomPosition();
        SetZPosition(item.transform);
        item.SetInputActive(true);
    }

    public void CreateNewItem()
    {
        Item item = itemFactory.Create(GerRandomPosition(), grid.GetCellSize(), ProcessClickOnItem);
        SetZPosition(item.transform);
    }

    private void SetZPosition(Transform item)
    {
        item.localPosition = new Vector3(item.localPosition.x, item.localPosition.y, -posCounter * 0.001f);
        posCounter++;
    }

    private Vector3 GerRandomPosition()
    {
        return new Vector3(
            Random.Range(constraints.xMin, constraints.xMax),
            Random.Range(constraints.yMin, constraints.yMax),
            0f);
    }

    private void ProcessClickOnItem(Item item)
    {
        pickedItemController.SetItem(item);
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0)) // 0 = left mouse button
        {
            Debug.Log("Left mouse button released!");
        }
    }
}
