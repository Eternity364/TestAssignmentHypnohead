using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "Artifacts", menuName = "Artifacts/WheatArtifact")]
public class WheatArtifact : BaseArtifact
{
    public override ResourceType AffectedType => ResourceType.Wheat;

    public override void Modify(Item item, ref Dictionary<ResourceType, float> producedResources, System.Action<ResourceType, Vector3, float, bool> OnModify)
    {
        if (item.ResourceType == AffectedType)
        {
            for (int i = 0; i < item.Size; i++)
                item.StartCoroutine(InvokeWithDelay(item, OnModify, delay * i, producedResources[ResourceType.Wheat]));
        }
    }
}
