using UnityEngine;

[CreateAssetMenu(fileName = "Artifacts", menuName = "Artifacts/ExplosionArtifact")]
public class ExplosionArtifact : BaseArtifact
{   
    [SerializeField] int multiplier = 50;
    [SerializeField, Range(0, 1)] float chance = 0.5f;
    public override ResourceType AffectedType => new();

    public override void Modify(ArtifactArgs args)
    {           
        bool currChance = Random.value < chance;
        if (currChance && args.item != null)
        {
            args.priceEntry = new PriceEntry { resourceType = args.item.ResourceType, amount = args.priceEntry.amount * multiplier };
            args.OnModify(args.priceEntry, args.iconPosition, new Vector2Int(0, -1));
            args.RemoveItem(args.item);
        }
    }
}
