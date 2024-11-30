using UnityEngine;
using UnityEngine.UI;

public class CreditsMenuPanel : MonoBehaviour
{
    public Button menuButton;
    public Button quitButton;

    private void OnEnable()
    {
        menuButton.onClick.AddListener(OnMenuButtonClick);
        quitButton.onClick.AddListener(OnQuitButtonClick);

#if UNITY_WEBGL
        quitButton.gameObject.SetActive(false);
#endif
    }

    private void OnDisable()
    {
        menuButton.onClick.RemoveListener(OnMenuButtonClick);
        quitButton.onClick.RemoveListener(OnQuitButtonClick);
    }

    private void OnMenuButtonClick()
    {
        AudioManager.Instance.PlayRegularButtonClip();
        SceneUtils.LoadMenuScene();
    }

    private void OnQuitButtonClick()
    {
        AudioManager.Instance.PlayRegularButtonClip();
        SceneUtils.QuitApplication();
    }
}
