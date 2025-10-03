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
    public Dictionary<ResourceType, float> producedResources;
    public System.Action<ResourceType, Vector3, int, bool> OnModify;
}
