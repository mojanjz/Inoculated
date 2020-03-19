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
            resetSlot(transform.gameObject.name); //resets slot so it's reusable
            inventoryObject.SetActive(true);
            if(child != null)
            {
                Destroy(child.gameObject);
            }



        }
    }
    public void useItem()
    {
        Debug.Log("inventory object is " + inventoryObject.name);
        if(inventoryObject.name.Equals("health potion"))
        {
            Debug.Log("add to health");
        } else if(inventoryObject.name.Equals("Stick pick-up"))
        {
            Debug.Log("do something with the log");
        }
        //inventoryObject.GetComponent<Item>().useItem();
    }
    public void resetSlot(string slotName)
    {
        int indexValue = int.Parse(Regex.Match(slotName, @"\d+").Value);
        inventoryObject.transform.GetComponent<Pickup>().inventory.isFull[indexValue-1] = false;
    }

}
