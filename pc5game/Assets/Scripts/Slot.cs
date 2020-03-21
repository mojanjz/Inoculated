using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class Slot : MonoBehaviour
{
    public GameObject groundObject;
    public void DropItem()
    {
        foreach (Transform child in transform)
        {
            resetSlot(transform.gameObject.name); //resets slot so it's reusable
            groundObject.SetActive(true);
            if(child != null)
            {
                Destroy(child.gameObject);
            }

            groundObject = null;

        }
    }
    public void useItem()
    {
        if(groundObject == null)
        {
            return;
        }

        Debug.Log("inventory object is " + groundObject.name);
        if(groundObject.name.Equals("health potion"))
        {
            Debug.Log("add to health");
        } else if(groundObject.name.Equals("Stick pick-up"))
        {
            Debug.Log("do something with the log");
        }
        //inventoryObject.GetComponent<Item>().useItem();
    }
    public void resetSlot(string slotName)
    {
        int indexValue = int.Parse(Regex.Match(slotName, @"\d+").Value);
        Pickup pickup = groundObject.GetComponent<Pickup>();
        pickup.inventory.isFull[indexValue] = false;
        groundObject.transform.position = pickup.inventory.transform.position; // Set object on ground where the player is standing
        pickup.WasDropped = true;
    }

}
