using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.UI;

public enum ResourceType
{
    Lumber,
    Iron,
    Wheat
}

public class ResourceManager : MonoBehaviour
{
    [SerializeField] private Grid grid;
    [SerializeField] private ItemFactory itemFactory;
    [SerializeField] private CommonParameters commonParameters;
    [SerializeField] private ArtifactsPanel artifactsPanel;
    [SerializeField] private List<BaseArtifact> artifacts = new List<BaseArtifact>();

    public HashSet<BaseArtifact> Artifacts => new HashSet<BaseArtifact>(artifactsSet);
    public UnityAction<BaseArtifact, bool> OnArtifactToggle;
    public UnityAction<PriceEntry> OnResourceChanged;


    private HashSet<BaseArtifact> artifactsSet = new HashSet<BaseArtifact>();
    private Dictionary<ResourceType, float> resources = new Dictionary<ResourceType, float>();
    private Dictionary<Item, float> gatherCurrentDelays = new Dictionary<Item, float>();
    private Dictionary<BaseArtifact, float> standaloneArtifactsCurrentDelays = new Dictionary<BaseArtifact, float>();
    private Dictionary<ResourceType, BaseArtifact> artifactByType = new Dictionary<ResourceType, BaseArtifact>();
    private HashSet<BaseArtifact> activeArtifacts = new HashSet<BaseArtifact>();
    private List<PriceEntry> priceEntries = new();

    public void ToggleArtifact(BaseArtifact artifact)
    {
        bool active = activeArtifacts.Contains(artifact);
        if (active)
        {
            activeArtifacts.Remove(artifact);
            standaloneArtifactsCurrentDelays.Remove(artifact);
        }
        else
        {
            activeArtifacts.Add(artifact);
            if (artifact.IsStandalone)
                standaloneArtifactsCurrentDelays[artifact] = commonParameters.ResourceGatherRate;
        }
        OnArtifactToggle?.Invoke(artifact, !active);
    }

    // Assumes that _priceEntries contains negative values
    public bool CheckIfEnoughResources(List<PriceEntry> _priceEntries)
    {
        for (int i = 0; i < _priceEntries.Count; i++)
        {
            Assert.IsTrue(_priceEntries[i].amount < 0);
            if (resources[_priceEntries[i].resourceType] < -_priceEntries[i].amount)
                return false;

        }
        return true;
    }

    public void ChangeResourceAmountBy(List<PriceEntry> _priceEntries)
    {
        for (int i = 0; i < _priceEntries.Count; i++)
        {
            ResourceType resourceType = _priceEntries[i].resourceType;
            int amount = _priceEntries[i].amount;
            resources[resourceType] += amount;
            OnResourceChanged?.Invoke(
                new PriceEntry()
                {
                    resourceType = resourceType,
                    amount = (int)resources[resourceType]
                });
        }
    }

    void Start()
    {
        for (int i = 0; i < System.Enum.GetValues(typeof(ResourceType)).Length; i++)
        {
            resources[(ResourceType)i] = 0;
        }
        foreach (BaseArtifact artifact in artifacts)
        {
            artifactByType[artifact.AffectedType] = artifact;
            artifactsSet.Add(artifact);
            activeArtifacts.Remove(artifact);
        }
        grid.OnItemsNumberChanged += OnItemCountChanged;
    }

    private void OnItemCountChanged()
    {
        List<Item> toRemove = new List<Item>();
        foreach (Item item in gatherCurrentDelays.Keys)
        {
            if (!grid.Items.Contains(item))
                toRemove.Add(item);
        }
        foreach (Item item in toRemove)
        {
            gatherCurrentDelays.Remove(item);
        }
        foreach (Item item in grid.Items)
        {
            if (!gatherCurrentDelays.ContainsKey(item))
                ResetGatherDelay(item);
        }
    }

    private void Update()
    {
        grid.IterateThroughResourceCells((item) =>
        {
            ProcessResourceItem(item);
        });
        ProcessStandaloneArtifacts();
        //ActivateArtifacts();
    }

    private void ProcessResourceItem(Item item)
    {
        gatherCurrentDelays[item] -= Time.deltaTime * GameSpeedController.Instance.Multiplier;
        if (gatherCurrentDelays[item] <= 0)
        {
            PriceEntry priceEntry = new PriceEntry { resourceType = item.ResourceType, amount = 1 };

            bool affectedByArtifact = false;
            foreach (BaseArtifact artifact in activeArtifacts)
            {
                if (artifact.IsStandalone) continue;
                artifact.Modify(new ArtifactArgs
                {
                    item = item,
                    iconPosition = grid.GetResourceCellPosition(item),
                    priceEntry = priceEntry,
                    OnModify = GatherResource,
                    RemoveItem = (item) =>
                        {
                            grid.RemoveItem(item, true);
                        }
                });
                affectedByArtifact = item.ResourceType == artifact.AffectedType || affectedByArtifact;
            }

            if (!affectedByArtifact)
                GatherResource(priceEntry, grid.GetResourceCellPosition(item), new Vector2Int(0, 1));
            ResetGatherDelay(item);
        }
    }

    private void ProcessStandaloneArtifacts()
    {
        PriceEntry priceEntry = new PriceEntry { amount = 1 };

        foreach (BaseArtifact artifact in activeArtifacts)
        {
            if (!artifact.IsStandalone) continue;

            standaloneArtifactsCurrentDelays[artifact] -= Time.deltaTime * GameSpeedController.Instance.Multiplier;
            if (standaloneArtifactsCurrentDelays[artifact] < 0)
            {
                Item item = null;
                if (grid.Items.Count > 0)
                    item = grid.Items[Random.Range(0, grid.Items.Count)];

                artifact.Modify(new ArtifactArgs
                {
                    item = item,
                    iconPosition = artifactsPanel.GetIconPosition(artifact),
                    priceEntry = priceEntry,
                    OnModify = GatherResource,
                    GetArtifactsNumber = () => activeArtifacts.Count,
                    RemoveItem = (item) =>
                        {
                            grid.RemoveItem(item, true);
                        }
                });
                standaloneArtifactsCurrentDelays[artifact] = commonParameters.ResourceGatherRate;
            }
        }
    }

    private void GatherResource(PriceEntry priceEntry, Vector3 position, Vector2Int animationDirection)
    {
        priceEntries.Clear();
        priceEntries.Add(priceEntry);
        ChangeResourceAmountBy(priceEntries);
        CreateOnResourceChangeAnimation(priceEntry, position, animationDirection);
        OnResourceChanged?.Invoke(
                new PriceEntry()
                {
                    resourceType = priceEntry.resourceType,
                    amount = (int)resources[priceEntry.resourceType]
                });
    }

    private void ResetGatherDelay(Item item)
    {
        if (!grid.ResourceCells.ContainsKey(item))
            return;

        ResourceModificator modificator = grid.GetResourceModificatorAtCell(grid.ResourceCells[item]);
        float delay = float.MaxValue;
        if (modificator.value > 0)
            delay = commonParameters.ResourceGatherRate / modificator.value;
        gatherCurrentDelays[item] = delay;
    }

    private void CreateOnResourceChangeAnimation(PriceEntry priceEntry, Vector3 position, Vector2Int animationDirection)
    {
        Icon icon = itemFactory.CreateResourceIcon(priceEntry.resourceType);
        Transform animation = icon.transform;
        icon.SetTextActive(true, "+" + priceEntry.amount.ToString());

        Vector3 finishPosition = position + new Vector3(animationDirection.x, animationDirection.y, 0) * grid.GetCellSize();

        animation.SetParent(transform);
        position.z = -0.001f;
        animation.position = position;
        Vector3 originalScale = animation.localScale;
        animation.localScale = originalScale * grid.GetCellSize();

        animation.DOMove(finishPosition, 0.75f).SetEase(Ease.InCubic);
        animation.GetComponent<SpriteRenderer>().material.DOFade(0, 0.75f).SetEase(Ease.InCubic).OnComplete(() => itemFactory.Destroy(icon));
    }

    // public int GetResourceAmount(Item item)
    // {
    //     if (resources.ContainsKey(item))
    //         return resources[item];
    //     return 0;
    // }
}
