using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void PlayGame()
    {
        GameManager.Instance.FadeToScene(SceneList.SampleScene);
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    public void LoadStartMenu()
    {
        GameManager.Instance.FadeToScene(SceneList.Main);
    }
}
