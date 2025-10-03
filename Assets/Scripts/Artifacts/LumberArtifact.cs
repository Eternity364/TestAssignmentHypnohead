using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Artifacts", menuName = "Artifacts/LumberArtifact")]
public class LumberArtifact : BaseArtifact
{
    [SerializeField, Range(0f, 1f)] private float chance = 0.5f;

    public override ResourceType AffectedType => ResourceType.Lumber;

    public override void Modify(Item item, ref Dictionary<ResourceType, float> producedResources, System.Action<ResourceType, Vector3, float, bool> OnModify)
    {
        
        if (item.ResourceType == AffectedType)
        {
            OnModify?.Invoke(AffectedType, item.transform.position, producedResources[AffectedType], false);
            if  (Random.value < chance)
            {
                OnModify?.Invoke(ResourceType.Wheat, item.transform.position, producedResources[AffectedType], true);
            }
        }
    }
}
