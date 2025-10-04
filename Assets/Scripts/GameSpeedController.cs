using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GameSpeedController : MonoBehaviour
{
    private float multiplier = 1f;

    public static GameSpeedController Instance;
    public float Multiplier => multiplier;

    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetMultiplier(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetMultiplier(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SetMultiplier(3);
        }
    }

    private void SetMultiplier(float value)
    {
        multiplier = value;
        DOTween.timeScale = multiplier;
    }
}
