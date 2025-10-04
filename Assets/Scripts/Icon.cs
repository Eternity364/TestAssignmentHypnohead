using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Icon : MonoBehaviour, IClearable
{
    [SerializeField] private ResourceTypeIcons[] icons;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private TextMeshProUGUI textMesh;

    public void SetIcon(ResourceType resourceType)
    {
        spriteRenderer.sprite = GetIcon(resourceType);
    }

    public void SetTextActive(bool active, string text)
    {
        textMesh.gameObject.SetActive(active);
        textMesh.text = text;
    }

    public void Clear()
    {
        textMesh.gameObject.SetActive(false);
        Color color = spriteRenderer.material.color;
        color.a = 1;
        spriteRenderer.material.color = color;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    private Sprite GetIcon(ResourceType resourceType)
    {
        foreach (var icon in icons)
        {
            if (icon.resourceType == resourceType)
            {
                return icon.icon;
            }
        }

        return null;
    }
}

[Serializable]
public struct ResourceTypeIcons
{
    public ResourceType resourceType;
    public Sprite icon;
}
