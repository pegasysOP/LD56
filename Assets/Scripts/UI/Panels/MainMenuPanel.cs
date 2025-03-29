using UnityEngine;
using UnityEngine.UI;

public class MainMenuPanel : MonoBehaviour
{
    public Button startButton;
    public Button quitButton;

    private void OnEnable()
    {
        startButton.onClick.AddListener(OnStartButtonClick);
        quitButton.onClick.AddListener(OnQuitButtonClick);

#if UNITY_WEBGL
        quitButton.gameObject.SetActive(false);
#endif
    }

    private void OnDisable()
    {
        startButton.onClick.RemoveListener(OnStartButtonClick);
        quitButton.onClick.RemoveListener(OnQuitButtonClick);
    }

    private void OnStartButtonClick()
    {
        AudioManager.Instance.PlayRegularButtonClip();
        SceneUtils.LoadGameScene();
    }
    
    private void OnQuitButtonClick()
    {
        AudioManager.Instance.PlayRegularButtonClip();
        SceneUtils.QuitApplication();
    }
}
