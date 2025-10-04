using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactsPanel : MonoBehaviour
{
    [SerializeField] private List<ArtifactIcon> artifactIcons = new List<ArtifactIcon>();
    [SerializeField] private ResourceManager resourceManager;

    public Vector3 GetIconPosition(BaseArtifact artifact)
    {
        foreach (var artifactIcon in artifactIcons)
        {
            if (artifactIcon.artifact == artifact)
            {
                float depth = Vector3.Distance(Camera.main.transform.position, Vector3.zero);
                Vector3 screenPos = artifactIcon.icon.transform.position;
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(
                    new Vector3(screenPos.x, screenPos.y, Camera.main.nearClipPlane)
                );
                return worldPos;
            }
        }
        return Vector3.zero;
    }

    private void Start()
    {
        resourceManager.OnArtifactToggle += OnArtifactToggle;
    }

    private void OnArtifactToggle(BaseArtifact artifact, bool isActive)
    {
        int index = -1;
        for (int i = 0; i < artifactIcons.Count; i++)
        {
            if (artifactIcons[i].artifact == artifact)
            {
                index = i;
                break;
            }
        }
        if (index != -1 && index < artifactIcons.Count)
        {
            artifactIcons[index].icon.SetActive(isActive);
        }
    }

    [Serializable]
    private struct ArtifactIcon
    {
        public BaseArtifact artifact;
        public GameObject icon;
    }
}
