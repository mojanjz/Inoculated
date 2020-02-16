using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
    //public GameObject inventory_object;
    public GameObject inventoryObject;
    public void DropItem()
    {
        Debug.Log(transform.gameObject);
        foreach (Transform child in transform)
        {
            Debug.Log("destoying the child object " + child.gameObject.name);
            inventoryObject.SetActive(true);
            Destroy(child.gameObject);

        }
    }
}
