using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class Cell : MonoBehaviour
{
    [SerializeField] GameObject animationObj;
    [SerializeField] SpriteRenderer back;
    [SerializeField] float pingPongDuration = 1f;

    public UnityAction OnMouseClick;

    private Tween pingPongTween;
    private Vector3 initialAnimationPosition;

    private void Start()
    {
        initialAnimationPosition = animationObj.transform.localPosition;
    }

    public void Init(Color color, Vector3 position, float cellSize)
    {
        float spritePixels = back.sprite.rect.width;
        float spriteUnits = spritePixels / back.sprite.pixelsPerUnit;
        float scale = cellSize / spriteUnits;
        transform.localPosition = position;
        transform.localScale = new Vector3(scale, scale, 1f);
        back.color = color;
    }

    public void SetVisible(bool visible)
    {
        back.enabled = visible;
    }

    public void SetAnimationActive(bool active, float speedMultiplier)
    {
        animationObj.SetActive(active);
        speedMultiplier *= 2f;
        if (active)
        {
            pingPongTween?.Kill();

            var sr = animationObj.GetComponent<SpriteRenderer>();
            if (sr == null) return;

            animationObj.transform.localPosition = initialAnimationPosition;
            float height = sr.bounds.size.y / animationObj.transform.localScale.y;
            float targetY = animationObj.transform.position.y + height;
            pingPongTween = animationObj.transform.DOMoveY(targetY, pingPongDuration)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine)
                .SetLink(gameObject);
            pingPongTween.timeScale = speedMultiplier;
        }
        else
            pingPongTween?.Pause();
    }

    private void OnDisable()
    {
        pingPongTween?.Kill();
        pingPongTween = null;
    }

    private void OnMouseDown()
    {
        OnMouseClick?.Invoke();
    }
}
