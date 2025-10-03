using UnityEngine;
using UnityEngine.Events;

public class Cell : MonoBehaviour
{
    public UnityAction OnMouseClick;

    private void OnMouseDown()
    {
        OnMouseClick?.Invoke();
    }
}
