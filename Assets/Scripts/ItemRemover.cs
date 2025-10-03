using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemRemover : MonoBehaviour
{
    [SerializeField] GameObject enteredStatus;

    public bool IsEntered => isEntered;

    private bool isEntered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Item item = collision.GetComponentInParent<Item>();
        if (item != null && item.IsPlaced)
        {
            enteredStatus.SetActive(true);
            isEntered = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Item item = collision.GetComponentInParent<Item>();
        if (item != null && item.IsPlaced)
        {
            enteredStatus.SetActive(false);
            isEntered = false;
        }
    }
}
