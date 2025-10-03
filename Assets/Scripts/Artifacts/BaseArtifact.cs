using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseArtifact : ScriptableObject
{
    public abstract ResourceType AffectedType { get; }
    public abstract void Modify(Item item, ref Dictionary<ResourceType, float> producedResources, System.Action<ResourceType, Vector3, float, bool> OnModify);

    protected float delay = 0.2f;
    
    protected IEnumerator InvokeWithDelay(Item item, System.Action<ResourceType, Vector3, float, bool> OnModify, float delay, float amount)
    {
        yield return new WaitForSeconds(delay);
        OnModify?.Invoke(AffectedType, item.transform.position, amount, false);
    }
}
