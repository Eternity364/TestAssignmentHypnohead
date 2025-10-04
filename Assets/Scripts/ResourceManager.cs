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
    [SerializeField] private List<BaseArtifact> artifacts = new List<BaseArtifact>();

    public HashSet<BaseArtifact> Artifacts => new HashSet<BaseArtifact>(artifactsSet);
    public UnityAction<BaseArtifact, bool> OnArtifactToggle;
    public UnityAction<PriceEntry> OnResourceChanged;


    private HashSet<BaseArtifact> artifactsSet = new HashSet<BaseArtifact>();
    private Dictionary<ResourceType, float> resources = new Dictionary<ResourceType, float>();
    private Dictionary<Item, float> gatherCurrentDelays = new Dictionary<Item, float>();
    private Dictionary<ResourceType, BaseArtifact> artifactByType = new Dictionary<ResourceType, BaseArtifact>();
    private HashSet<BaseArtifact> activeArtifacts = new HashSet<BaseArtifact>();
    private List<PriceEntry> priceEntries = new();

    public void ToggleArtifact(ResourceType resourceType)
    {
        BaseArtifact artifact = artifactByType[resourceType];
        bool active = activeArtifacts.Contains(artifact);
        if (active)
            activeArtifacts.Remove(artifact);
        else
            activeArtifacts.Add(artifact);
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
        IronArtifact ironArtifact = (IronArtifact)artifactByType[ResourceType.Iron];
        ironArtifact.grid = grid;
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
        //ActivateArtifacts();
    }

    private void ProcessResourceItem(Item item)
    {
        gatherCurrentDelays[item] -= Time.deltaTime * GameSpeedController.Instance.Multiplier;
        if (gatherCurrentDelays[item] <= 0)
        {
            PriceEntry priceEntry = new PriceEntry { resourceType = item.ResourceType, amount = 1 };

            foreach (BaseArtifact artifact in activeArtifacts)
            {
                artifact.Modify(new ArtifactArgs
                {
                    item = item,
                    iconPosition = grid.GetResourceCellPosition(item),
                    priceEntry = priceEntry,
                    OnModify = GatherResource
                });
            }

            GatherResource(priceEntry, grid.GetResourceCellPosition(item), false);
            ResetGatherDelay(item);
        }
    }

    private void GatherResource(PriceEntry priceEntry, Vector3 position, bool alternativeMode)
    {
        priceEntries.Clear();
        priceEntries.Add(priceEntry);
        ChangeResourceAmountBy(priceEntries);
        CreateOnResourceChangeAnimation(priceEntry, position, alternativeMode);
        OnResourceChanged?.Invoke(
                new PriceEntry()
                {
                    resourceType = priceEntry.resourceType,
                    amount = (int)resources[priceEntry.resourceType]
                });
    }

    private void ActivateArtifacts()
    {
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ToggleArtifact(ResourceType.Lumber);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            ToggleArtifact(ResourceType.Wheat);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            ToggleArtifact(ResourceType.Iron);
        }
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

    private void CreateOnResourceChangeAnimation(PriceEntry priceEntry, Vector3 position, bool alternativeMode = false)
    {
        Icon icon = itemFactory.CreateResourceIcon(priceEntry.resourceType);
        Transform animation = icon.transform;
        icon.SetTextActive(true, "+" + priceEntry.amount.ToString());

        Vector3 finishPosition = position + Vector3.up * grid.GetCellSize();
        if (alternativeMode)
        {
            if (priceEntry.resourceType == ResourceType.Wheat)
                finishPosition += Vector3.left * grid.GetCellSize();
        }

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
