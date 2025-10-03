using UnityEngine;

[CreateAssetMenu(fileName = "CommonParameters", menuName = "ScriptableObjects/CommonParameters", order = 1)]
public class CommonParameters : ScriptableObject
{
    [SerializeField] private int cellSize;
    [SerializeField] private float resourceGatherRate;

    public int CellSize => cellSize;
    public float ResourceGatherRate => resourceGatherRate;
}
