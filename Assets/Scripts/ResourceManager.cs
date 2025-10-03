using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

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
    private Dictionary<BaseArtifact, bool> activeArtifacts = new Dictionary<BaseArtifact, bool>();

    void Start()
    {
        for (int i = 0; i < System.Enum.GetValues(typeof(ResourceType)).Length; i++)
        {
            resources[(ResourceType)i] = 0;
        }
        foreach (BaseArtifact artifact in artifacts)
        {
            artifactByType[artifact.AffectedType] = artifact;
            activeArtifacts[artifact] = false;
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
                foreach (var artifact in activeArtifacts.Where(a => a.Value).Select(a => a.Key))
                {
                    artifact.Modify(item, ref produced, GatherResource);
                }

                foreach (var kvp in produced)
                {
                    if (!activeArtifacts[artifactByType[kvp.Key]])
                        GatherResource(kvp.Key, item.transform.position, kvp.Value, false);
                }

                ResetGatherDelay(item);
                Debug.Log($"Gathered 1 {item.ResourceType}. Total: {resources[item.ResourceType]}");
            }
        });
        ActivateArtifacts();
    }

    private void GatherResource(ResourceType resourceType, Vector3 position, float amount, bool alternativeMode)
    {
        resources[resourceType] += amount;
        CreateOnResourceChangeAnimation(resourceType, position, alternativeMode);
        OnResourceGathered?.Invoke(resourceType, amount);
    }

    private void ActivateArtifacts()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            activeArtifacts[artifactByType[ResourceType.Lumber]] = !activeArtifacts[artifactByType[ResourceType.Lumber]];
            OnArtifactToggle?.Invoke(artifactByType[ResourceType.Lumber], activeArtifacts[artifactByType[ResourceType.Lumber]]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            activeArtifacts[artifactByType[ResourceType.Wheat]] = !activeArtifacts[artifactByType[ResourceType.Wheat]];
            OnArtifactToggle?.Invoke(artifactByType[ResourceType.Wheat], activeArtifacts[artifactByType[ResourceType.Wheat]]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            activeArtifacts[artifactByType[ResourceType.Iron]] = !activeArtifacts[artifactByType[ResourceType.Iron]];
            OnArtifactToggle?.Invoke(artifactByType[ResourceType.Iron], activeArtifacts[artifactByType[ResourceType.Iron]]);
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

    private void CreateOnResourceChangeAnimation(ResourceType resourceType, Vector3 position, bool alternativeMode = false)
    {
        Transform animation = itemFactory.CreateResourceIcon(resourceType);

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
        animation.GetComponent<SpriteRenderer>().material.DOFade(0, 0.75f).SetEase(Ease.InCubic).OnComplete(() => Destroy(animation.gameObject));
    }

    // public int GetResourceAmount(Item item)
    // {
    //     if (resources.ContainsKey(item))
    //         return resources[item];
    //     return 0;
    // }
}
