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
            
            args.OnModify(args.priceEntry, args.iconPosition, new Vector2Int(0, 1));
            if  (Random.value < chance)
            {
                args.priceEntry.resourceType = ResourceType.Wheat;
                args.OnModify(args.priceEntry, args.iconPosition, new Vector2Int(-1, 1));
            }
        }
    }
}
