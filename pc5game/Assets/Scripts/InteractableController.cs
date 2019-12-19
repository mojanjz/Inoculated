using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableController : MonoBehaviour
{
    [SerializeField] private GameObject altObject;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    /* Method for changing the state of an object. */
    public void UpdateState()
    {
        gameObject.SetActive(false);
        altObject.SetActive(true);
    }
}
