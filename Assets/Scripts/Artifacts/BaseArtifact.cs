using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseArtifact : ScriptableObject
{
    public abstract ResourceType AffectedType { get; }
    public abstract void Modify(ArtifactArgs args);
}

public struct ArtifactArgs
{
    public Item item;
    public Vector3 iconPosition;
    public PriceEntry priceEntry;
    public System.Action<PriceEntry, Vector3, bool> OnModify;
}
