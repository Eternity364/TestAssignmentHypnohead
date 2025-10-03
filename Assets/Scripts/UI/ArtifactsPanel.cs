using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactsPanel : MonoBehaviour
{
    [SerializeField] private List<GameObject> artifactIcons = new List<GameObject>();
    [SerializeField] private ResourceManager resourceManager;

    private void Start()
    {
        resourceManager.OnArtifactToggle += OnArtifactToggle;
    }

    private void OnArtifactToggle(BaseArtifact artifact, bool isActive)
    {
        int index = -1;
        switch (artifact.AffectedType)
        {
            case ResourceType.Lumber:
                index = 0;
                break;
            case ResourceType.Wheat:
                index = 1;
                break;
            case ResourceType.Iron:
                index = 2;
                break;
        }
        if (index != -1 && index < artifactIcons.Count)
        {
            artifactIcons[index].SetActive(isActive);
        }
    }
}
