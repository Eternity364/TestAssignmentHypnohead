using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PurchaseManager : MonoBehaviour
{
    [SerializeField] ResourceManager resourceManager;
    [SerializeField] ItemField itemField;

    private HashSet<BaseArtifact> purchasedArtifacts = new();

    public void Buy()
    {
        HashSet<BaseArtifact> purchasableArtifacts = resourceManager.Artifacts;
        purchasableArtifacts.ExceptWith(purchasedArtifacts);
        if (purchasableArtifacts.Count > 0)
        {
            BaseArtifact artifact = purchasableArtifacts.ElementAt(Random.Range(0, purchasableArtifacts.Count));
            purchasedArtifacts.Add(artifact);
            resourceManager.ToggleArtifact(artifact.AffectedType);
        }
        else
            itemField.CreateNewItem();
    }
}
