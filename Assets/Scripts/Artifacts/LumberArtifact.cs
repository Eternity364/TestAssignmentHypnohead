using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Artifacts", menuName = "Artifacts/LumberArtifact")]
public class LumberArtifact : BaseArtifact
{
    [SerializeField, Range(0f, 1f)] private float chance = 0.5f;

    public override ResourceType AffectedType => ResourceType.Lumber;

    public override void Modify(ArtifactArgs args)
    {
        if (args.item.ResourceType == AffectedType)
        {
            args.OnModify(AffectedType, args.iconPosition, (int)args.producedResources[AffectedType], false);
            if  (Random.value < chance)
            {
                args.OnModify(ResourceType.Wheat, args.iconPosition, (int)args.producedResources[ResourceType.Wheat], true);
            }
        }
    }
}
