using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class Slot : MonoBehaviour
{
    public GameObject inventoryObject;
    public void DropItem()
    {
        foreach (Transform child in transform)
        {
            Debug.Log("destoying the child object " + child.gameObject.name);
            resetSlot(transform.gameObject.name); //resets slot so it's reusable
            inventoryObject.SetActive(true);
            Destroy(child.gameObject);


        }
    }
    public void resetSlot(string slotName)
    {
        int indexValue = int.Parse(Regex.Match(slotName, @"\d+").Value);
        inventoryObject.transform.GetComponent<Pickup>().inventory.isFull[indexValue-1] = false;
    }

}
