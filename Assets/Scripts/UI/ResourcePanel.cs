using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class ResourcePanel : MonoBehaviour
{
    [SerializeField] private List<ResourceTexts> resourceTexts = new List<ResourceTexts>();
    [SerializeField] private ResourceManager resourceManager;

    ResourceTexts[] resourceTextsArray;

    private void Start()
    {
        resourceManager.OnResourceChanged += OnResourceGathered;
        Assert.AreEqual(resourceTexts.Count, System.Enum.GetValues(typeof(ResourceType)).Length);
        resourceTextsArray = new ResourceTexts[resourceTexts.Count];
        for (int i = 0; i < resourceTexts.Count; i++)
        {
            int index = (int)resourceTexts[i].type;
            resourceTextsArray[index] = resourceTexts[i];
            resourceTextsArray[index].text.text = "0";
        }
    }

    private void OnResourceGathered(ResourceType resourceType, float amount)
    {
        resourceTextsArray[(int)resourceType].text.text = amount.ToString();
    }

    [Serializable]
    private struct ResourceTexts
    {
        public ResourceType type;
        public Text text;
    }
}
