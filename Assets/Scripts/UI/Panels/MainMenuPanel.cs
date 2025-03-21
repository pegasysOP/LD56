using UnityEngine;
using UnityEngine.UI;

public class MainMenuPanel : MonoBehaviour
{
    public Button startButton;
    public Button instructionsButton;
    public Button quitButton;

    private void OnEnable()
    {
        startButton.onClick.AddListener(OnStartButtonClick);
        instructionsButton.onClick.AddListener(OnInstructionButtonClick);
        quitButton.onClick.AddListener(OnQuitButtonClick);

#if UNITY_WEBGL
        quitButton.gameObject.SetActive(false);
#endif
    }

    private void OnDisable()
    {
        startButton.onClick.RemoveListener(OnStartButtonClick);
        instructionsButton.onClick.RemoveListener(OnInstructionButtonClick);
        quitButton.onClick.RemoveListener(OnQuitButtonClick);
    }

    private void OnStartButtonClick()
    {
        AudioManager.Instance.PlayRegularButtonClip();
        SceneUtils.LoadGameScene();
    }
    
    private void OnInstructionButtonClick()
    {
        AudioManager.Instance.PlayMenuClip();
        AudioManager.Instance.PlayRegularButtonClip();
        SceneUtils.LoadInstructionScene();
    }
    
    private void OnQuitButtonClick()
    {
        AudioManager.Instance.PlayRegularButtonClip();
        SceneUtils.QuitApplication();
    }
}
