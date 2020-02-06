using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
   public GameObject inventoryObject;
   public void DropItem()
    {
        foreach(Transform child in transform)
        {
            Debug.Log("destoying the child object " + child.gameObject.name);
            inventoryObject.SetActive(true);
            Destroy(child.gameObject);

        }
    }
}
