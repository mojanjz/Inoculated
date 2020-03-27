using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{

    [SerializeField]private GameObject player1;
    [SerializeField]private GameObject player2;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (player1 == null || player2 == null) {
            transitionScene();
        }
    }

    public void transitionScene()
    {
        SceneManager.LoadScene("GameOver");
    }
}
