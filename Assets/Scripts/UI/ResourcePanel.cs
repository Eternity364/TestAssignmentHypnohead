using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourcePanel : MonoBehaviour
{
    [SerializeField] private List<Text> resourceTexts = new List<Text>();
    [SerializeField] private ResourceManager resourceManager;

    private void Start()
    {
        resourceManager.OnResourceGathered += OnResourceGathered;
        for (int i = 0; i < resourceTexts.Count; i++)
        {
            resourceTexts[i].text = "0";
        }
    }

    private void OnResourceGathered(ResourceType resourceType, float amount)
    {
        int index = -1;
        switch (resourceType)
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
        if (index != -1 && index < resourceTexts.Count)
        {
            float currentAmount = float.Parse(resourceTexts[index].text);
            resourceTexts[index].text = (currentAmount + amount).ToString();
        }
    }
}
