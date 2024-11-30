using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneUtils
{
    public const string GAME_SCENE = "Main Game";
    public const string MENU_SCENE = "Menu";
    public const string CREDIT_SCENE = "Credits";
    public const string INSTRUCTION_SCENE = "Instructions";

    public static void LoadGameScene()
    {
        SceneManager.LoadScene(GAME_SCENE);
    }

    public static void LoadMenuScene()
    {
        SceneManager.LoadScene(MENU_SCENE);
    }

    public static void LoadCreditScene()
    {
        SceneManager.LoadScene(CREDIT_SCENE);
    }

    public static void LoadInstructionScene()
    {

        SceneManager.LoadScene(INSTRUCTION_SCENE);
    }

    public static void QuitApplication()
    {
        Application.Quit();
    }
}
