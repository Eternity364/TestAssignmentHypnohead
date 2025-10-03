using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Artifacts", menuName = "Artifacts/IronArtifact")]
public class IronArtifact : BaseArtifact
{
    [SerializeField, Range(0f, 1f)] private float chance = 0.1f;
    [SerializeField] private int multiplier = 10;
    
    protected new float delay = 0.1f;

    public override ResourceType AffectedType => ResourceType.Iron;
    public Grid grid;

    public override void Modify(Item item, ref Dictionary<ResourceType, float> producedResources, System.Action<ResourceType, Vector3, float, bool> OnModify)
    {
        if (item.ResourceType == AffectedType)
        {
            bool currChance = Random.value < chance;
            if (currChance)
                grid.RemoveItem(item, true);
            else {
                for (int i = 0; i < multiplier; i++)
                    item.StartCoroutine(InvokeWithDelay(item, OnModify, delay * i, producedResources[AffectedType]));
            }
        }
    }
}
