using UnityEngine;
using UnityEngine.UI;

public class InstructionsMenuPanel : MonoBehaviour
{
    public Button menuButton;

    private void OnEnable()
    {
        menuButton.onClick.AddListener(OnMenuButtonClick);
    }

    private void OnDisable()
    {
        menuButton.onClick.RemoveListener(OnMenuButtonClick);
    }

    private void OnMenuButtonClick()
    {
        AudioManager.Instance.PlayMenuClip();
        AudioManager.Instance.PlayRegularButtonClip();
        SceneUtils.LoadMenuScene();
    }
}
