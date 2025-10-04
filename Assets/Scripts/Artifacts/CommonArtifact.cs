using UnityEngine;

[CreateAssetMenu(fileName = "Artifacts", menuName = "Artifacts/CommonArtifact")]
public class CommonArtifact : BaseArtifact
{   
    [SerializeField] int numberOfArtifacts = 3;
    public override ResourceType AffectedType => new();

    public override void Modify(ArtifactArgs args)
    {
        if (args.GetArtifactsNumber() >= numberOfArtifacts ) {
            var resourceTypes = System.Enum.GetValues(typeof(ResourceType));
            var randomIndex = Random.Range(0, resourceTypes.Length);
            var randomResourceType = (ResourceType)resourceTypes.GetValue(randomIndex);
            args.priceEntry = new PriceEntry { resourceType = randomResourceType, amount = args.priceEntry.amount };
            args.OnModify(args.priceEntry, args.iconPosition, new Vector2Int(0, -1));
        }
    }
}
