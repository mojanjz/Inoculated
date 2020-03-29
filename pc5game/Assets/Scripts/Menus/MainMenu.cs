using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void playGame()
    {
        GameManager.Instance.FadeToScene(SceneList.SampleScene);
    }
    public void quitGame()
    {
        Application.Quit();
    }
}
