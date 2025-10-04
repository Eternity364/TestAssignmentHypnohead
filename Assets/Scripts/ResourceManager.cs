using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
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

    public UnityAction<BaseArtifact, bool> OnArtifactToggle;
    public UnityAction<ResourceType, float> OnResourceGathered;

    private Dictionary<ResourceType, float> resources = new Dictionary<ResourceType, float>();
    private Dictionary<Item, float> gatherCurrentDelays = new Dictionary<Item, float>();
    private Dictionary<ResourceType, BaseArtifact> artifactByType = new Dictionary<ResourceType, BaseArtifact>();
    private HashSet<BaseArtifact> activeArtifacts = new HashSet<BaseArtifact>();

    void Start()
    {
        for (int i = 0; i < System.Enum.GetValues(typeof(ResourceType)).Length; i++)
        {
            resources[(ResourceType)i] = 0;
        }
        foreach (BaseArtifact artifact in artifacts)
        {
            artifactByType[artifact.AffectedType] = artifact;
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
        grid.IterateThroughResourceCells((item, cell) =>
        {
            gatherCurrentDelays[item] -= Time.deltaTime;
            if (gatherCurrentDelays[item] <= 0)
            {
                var produced = new Dictionary<ResourceType, float>
                {
                    [item.ResourceType] = 1
                };
                foreach (BaseArtifact artifact in activeArtifacts)
                {
                    artifact.Modify(new ArtifactArgs
                    {
                        item = item,
                        iconPosition = grid.GetResourceCellPosition(item),
                        producedResources = produced,
                        OnModify = GatherResource
                    });
                }

                foreach (var kvp in produced)
                {
                    if (!activeArtifacts.Contains(artifactByType[kvp.Key]))
                        GatherResource(kvp.Key, grid.GetResourceCellPosition(item), (int)kvp.Value, false);
                }

                ResetGatherDelay(item);
                Debug.Log($"Gathered 1 {item.ResourceType}. Total: {resources[item.ResourceType]}");
            }
        });
        ActivateArtifacts();
    }

    private void GatherResource(ResourceType resourceType, Vector3 position, int amount, bool alternativeMode)
    {
        resources[resourceType] += amount;
        CreateOnResourceChangeAnimation(resourceType, position, amount, alternativeMode);
        OnResourceGathered?.Invoke(resourceType, amount);
    }

    private void ActivateArtifacts()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ToggleArtifact(ResourceType.Lumber);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ToggleArtifact(ResourceType.Wheat);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ToggleArtifact(ResourceType.Iron);
        }
    }

    private void ToggleArtifact(ResourceType resourceType)
    {
        BaseArtifact artifact = artifactByType[resourceType];
        bool active = activeArtifacts.Contains(artifact);
        if (active)
            activeArtifacts.Remove(artifact);
        else
            activeArtifacts.Add(artifact);
        OnArtifactToggle?.Invoke(artifact, !active);
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

    private void CreateOnResourceChangeAnimation(ResourceType resourceType, Vector3 position, int amount, bool alternativeMode = false)
    {
        Icon icon = itemFactory.CreateResourceIcon(resourceType);
        Transform animation = icon.transform;
        icon.SetTextActive(true, "+" + amount.ToString());

        Vector3 finishPosition = position + Vector3.up * grid.GetCellSize();
        if (alternativeMode)
        {
            if (resourceType == ResourceType.Wheat)
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
