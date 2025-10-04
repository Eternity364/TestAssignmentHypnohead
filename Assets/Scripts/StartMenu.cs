using UnityEngine;

public class StartMenu : MonoBehaviour
{

    [SerializeField] ShapesManager shapesManager;
    [SerializeField] GameObject mainGameUI;
    [SerializeField] GameObject mainGame;
    [SerializeField] GameObject button;

    public void OnPressStartButton()
    {
        button.SetActive(false);
        shapesManager.Save();
        shapesManager.gameObject.SetActive(false);
        mainGameUI.SetActive(true);
        mainGame.SetActive(true);
    }
}
