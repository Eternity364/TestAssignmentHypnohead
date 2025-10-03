using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "Artifacts", menuName = "Artifacts/WheatArtifact")]
public class WheatArtifact : BaseArtifact
{
    public override ResourceType AffectedType => ResourceType.Wheat;

    public override void Modify(ArtifactArgs args)
    {
        if (args.item.ResourceType == AffectedType)
        {
            args.OnModify(AffectedType, args.iconPosition, args.item.Size * (int)args.producedResources[AffectedType], false);
        }
    }
}
