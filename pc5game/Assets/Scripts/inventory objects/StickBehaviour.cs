using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickBehaviour : MonoBehaviour
{
	public StickItem stick;

    // Start is called before the first frame update
    void Start()
    {
        GameObject stickModel = GameObject.Find("Stick pick-up");
        stick = new StickItem(1, "Stick pick-up", stickModel);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
