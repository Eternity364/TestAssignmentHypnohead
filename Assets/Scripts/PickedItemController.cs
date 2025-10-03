using UnityEngine;

public class PickedItemController : MonoBehaviour
{
    [SerializeField] ItemField field;
    [SerializeField] Grid grid;
    [SerializeField] ItemRemover itemRemover;

    private Item pickedItem;
    private Vector2Int pickedItemCell = new Vector2Int(-1, -1);

    public void SetItem(Item item)
    {
        if (item.IsPlaced)
        {
            pickedItemCell = grid.GetItemCell(item);
            grid.RemoveItem(item, false, true, false);
        }
        pickedItem = item;
        item.transform.SetParent(transform);
    }

    private bool IsItemInsideGrid()
    {
        return grid.DoesItemFitIntoGrid(pickedItem);
    }

    private void ReleaseItemToGrid()
    {
        grid.AddItem(pickedItem, pickedItemCell);
        ClearVariables();
    }

    private void ReleaseItemBackToField()
    {
        field.Add(pickedItem);
        ClearVariables();
    }

    private void ClearVariables()
    {
        pickedItem = null;
        pickedItemCell = new Vector2Int(-1, -1);
    }

    private void Update()
    {
        if (pickedItem != null)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = transform.position.z;
            pickedItem.transform.position = mousePos;

            if (Input.GetMouseButtonUp(0))
            {
                if (pickedItem.IsPlaced) {
                    bool entered = itemRemover.IsEntered;
                    Item item = pickedItem;
                    ReleaseItemToGrid();
                    if (entered)
                        grid.RemoveItem(item, true);
                }
                else
                {
                    if (IsItemInsideGrid())
                        ReleaseItemToGrid();
                    else
                        ReleaseItemBackToField();
                }
            }

            if (Input.GetMouseButtonUp(1) && !pickedItem.IsPlaced)
            {
                pickedItem.Rotate();
            }
        }
    }
}
