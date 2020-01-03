using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unlockable : MonoBehaviour
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


    /* Method for "unlocking" the object by updating the state of the object. */
    public void OnUnlock()
    {
        gameObject.SetActive(false);
        altObject.SetActive(true);
    }
}
