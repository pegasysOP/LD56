using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{

    public const string GAME_SCENE = "Lewis Test";
    public const string MENU_SCENE = "Menu";
    public const string CREDIT_SCENE = "Credits";
    public const string INSTRUCTION_SCENE = "Instructions";

    private BoardManager BM;

    // Start is called before the first frame update
    void Start()
    {
        BM = FindObjectOfType<BoardManager>(); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadGameScene()
    {
        SceneManager.LoadScene(GAME_SCENE);
    }

    public void LoadMenuScene()
    {
        SceneManager.LoadScene(MENU_SCENE);
    }

    public void LoadCreditScene()
    {
        SceneManager.LoadScene(CREDIT_SCENE);
    }

    public void LoadInstructionScene()
    {
        SceneManager.LoadScene(INSTRUCTION_SCENE);
    }

    public void QuitButtonPressed()
    {
        Application.Quit();
    }
}
