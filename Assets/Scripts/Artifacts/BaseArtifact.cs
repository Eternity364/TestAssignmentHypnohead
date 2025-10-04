using System;
using UnityEngine;

public abstract class BaseArtifact : ScriptableObject
{
    public abstract ResourceType AffectedType { get; }
    public abstract void Modify(ArtifactArgs args);
    public bool IsStandalone => isStandalone;

    // Means it works by itself, not for each item
    [SerializeField] private bool isStandalone = false;
}

public struct ArtifactArgs
{
    public Item item;
    public Vector3 iconPosition;
    public PriceEntry priceEntry;
    public Func<int> GetArtifactsNumber;
    public Action<Item> RemoveItem;
    public Action<PriceEntry, Vector3, Vector2Int> OnModify;
}
