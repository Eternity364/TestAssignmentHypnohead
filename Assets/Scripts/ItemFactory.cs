using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

public class ItemFactory : MonoBehaviour
{
    [SerializeField] private Transform parent;
    [SerializeField] private Transform itemPrefab;
    [SerializeField] private Icon resourceIconPrefab;

    public Item Create(Vector3 position, float cellSize, UnityAction<Item> OnMouseClick)
    {
        Transform itemTransform = ObjectPool.Instance.GetObject(itemPrefab.gameObject).transform;
        itemTransform.SetParent(parent);
        itemTransform.localPosition = position;
        Item item = itemTransform.GetComponent<Item>();
        Color color = new Color(Random.value, Random.value, Random.value);
        ShapeData shapeData = ShapeStorage.GetRandom;
        int resourceTypesCount = System.Enum.GetValues(typeof(ResourceType)).Length;
        ResourceType resourceType = (ResourceType)Random.Range(0, resourceTypesCount);
        Transform resourceIcon = CreateResourceIcon(resourceType).transform;
        item.Setup(color, shapeData, resourceIcon, cellSize, resourceType, OnMouseClick);

        return item;
    }

    public void Destroy(IClearable obj)
    {
        obj.Clear();
        ObjectPool.Instance.ReturnObject(obj.GetGameObject());
    }

    public Icon CreateResourceIcon(ResourceType resourceType)
    {
        Icon icon = ObjectPool.Instance.GetObject(resourceIconPrefab.gameObject).GetComponent<Icon>();
        icon.SetIcon(resourceType);
        return icon;
    }
}
