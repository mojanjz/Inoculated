using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unlockable : MonoBehaviour
{
    [SerializeField] private GameObject altObject;
    public GameObject key;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    // Method for "unlocking" the object by updating the state of the object.
    // Returns true if unlocked.
    public bool OnUnlock(GameObject key, bool overrideKey = false)
    {
        if ( (key == this.key && key != null) || overrideKey)
        {
            gameObject.SetActive(false);

            if (altObject != null)
            {
                altObject.SetActive(true);
            }

            return true;
        }

        return false;
    }
}
