using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

public class ItemFactory : MonoBehaviour
{
    [SerializeField] private Transform parent;
    [SerializeField] private Transform itemPrefab;
    [SerializeField] private List<Transform> resourceIconPrefabs;

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
        Transform resourceIcon = CreateResourceIcon(resourceType);
        item.Setup(color, shapeData, resourceIcon, cellSize, resourceType, OnMouseClick);

        return item;
    }

    public void Destroy(Item item)
    {
        item.Clear();
        ObjectPool.Instance.ReturnObject(item.gameObject);
    }

    public Transform CreateResourceIcon(ResourceType resourceType)
    {
        int resourceTypesCount = System.Enum.GetValues(typeof(ResourceType)).Length;
        Assert.AreEqual(resourceIconPrefabs.Count, resourceTypesCount);
        GameObject obj = ObjectPool.Instance.GetObject(resourceIconPrefabs[(int)resourceType].gameObject);
        return obj.transform;
    }

    public void DestroyResourceIcon(GameObject icon)
    {
        icon.GetComponentInChildren<TMPro.TextMeshProUGUI>(true).gameObject.SetActive(false);
        Color color = icon.GetComponent<SpriteRenderer>().material.color;
        color.a = 1;
        icon.GetComponent<SpriteRenderer>().material.color = color;
        ObjectPool.Instance.ReturnObject(icon);
    }
}
