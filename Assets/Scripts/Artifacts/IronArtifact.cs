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
                args.RemoveItem(args.item);
            else {
                args.priceEntry.amount *= multiplier;
                args.OnModify(args.priceEntry, args.iconPosition, new Vector2Int(0, 1));
            }
        }
    }
}
