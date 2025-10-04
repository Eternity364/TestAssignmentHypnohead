using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PurchaseManager : MonoBehaviour
{
    [SerializeField] private ResourceManager resourceManager;
    [SerializeField] private ItemField itemField;
    [SerializeField] private float multiplierGrowthRate = 1.2f;
    // prices should be stored as negative
    [SerializeField] private List<PriceEntry> initialPrice;

    private HashSet<BaseArtifact> purchasedArtifacts = new();
    private float multiplier = 1f;
    private List<PriceEntry> incrementedPrice;

    private void Start()
    {
        incrementedPrice = new List<PriceEntry>(initialPrice.Count);
    }

    public bool TryToPay()
    {
        CalculateIncrementedPrice();
        if (resourceManager.CheckIfEnoughResources(incrementedPrice))
        {
            resourceManager.ChangeResourceAmountBy(incrementedPrice);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Buy()
    {
        if (TryToPay())
        {
            HashSet<BaseArtifact> purchasableArtifacts = resourceManager.Artifacts;
            purchasableArtifacts.ExceptWith(purchasedArtifacts);
            if (purchasableArtifacts.Count > 0)
            {
                BaseArtifact artifact = purchasableArtifacts.ElementAt(UnityEngine.Random.Range(0, purchasableArtifacts.Count));
                purchasedArtifacts.Add(artifact);
                resourceManager.ToggleArtifact(artifact.AffectedType);
            }
            else
                itemField.CreateNewItem();
            multiplier *= multiplierGrowthRate;
        }
    }

    private void CalculateIncrementedPrice()
    {
        incrementedPrice.Clear();
        for (int i = 0; i < initialPrice.Count; i++)
        {
            PriceEntry entry = initialPrice[i];
            entry.amount = Mathf.CeilToInt(entry.amount * multiplier);
            incrementedPrice.Add(entry);
        }
    }
}

[Serializable]
public struct PriceEntry
{
    public ResourceType resourceType;
    public int amount;
}
