using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickedItemController : MonoBehaviour
{
    [SerializeField] ItemField field;
    [SerializeField] Grid grid;

    private Item pickedItem;

    public void SetItem(Item item)
    {
        pickedItem = item;
        item.transform.SetParent(transform);
    }

    private bool IsItemInsideGrid()
    {
        return grid.IsItemFitIntoGrid(pickedItem);
    }

    private void ReleaseItemToGrid()
    {
        grid.AddItem(pickedItem);
        pickedItem = null;
    }

    private void ReleaseItemBackToField()
    {
        field.Add(pickedItem);
        pickedItem = null;
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
                if (IsItemInsideGrid())
                    ReleaseItemToGrid();
                else
                    ReleaseItemBackToField();
            }
            if (Input.GetMouseButtonUp(1))
            {
                pickedItem.Rotate();
            }
        }
    }
}
