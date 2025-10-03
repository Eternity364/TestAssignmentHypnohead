using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Artifacts", menuName = "Artifacts/IronArtifact")]
public class IronArtifact : BaseArtifact
{
    [SerializeField, Range(0f, 1f)] private float chance = 0.1f;
    [SerializeField] private int multiplier = 10;

    public override ResourceType AffectedType => ResourceType.Iron;
    public Grid grid;

    public override void Modify(ArtifactArgs args)
    {
        if (args.item.ResourceType == AffectedType)
        {
            bool currChance = Random.value < chance;
            if (currChance)
                grid.RemoveItem(args.item, true);
            else {
                args.OnModify(AffectedType, args.iconPosition, multiplier * (int)args.producedResources[AffectedType], false);
            }
        }
    }
}
