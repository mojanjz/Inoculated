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
            Pickup pickup = groundObject.GetComponent<Pickup>();
            groundObject.transform.position = pickup.inventory.transform.position; // Set object on ground where the player is standing
            pickup.WasDropped = true;
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
        Pickup pickup = groundObject.GetComponent<Pickup>();
        GameObject player = pickup.inventory.gameObject;
        Debug.Log(pickup.inventory.gameObject);
        CharacterStats charStat = player.GetComponent<CharacterStats>();

        string itemType = groundObject.tag;
        switch (itemType){
            case "healthPotion":
                Debug.Log("health potion case statement");
                charStat.updateHealth(1);
                break;
        }
        Debug.Log("resetting the slot");
        foreach (Transform child in transform)
        {
            resetSlot(transform.gameObject.name); //resets slot so it's reusable
            pickup.WasDropped = true;
            if (child != null)
            {
                Destroy(child.gameObject);
            }

            groundObject = null;

        }

    }
    public void resetSlot(string slotName)
    {
        int indexValue = int.Parse(Regex.Match(slotName, @"\d+").Value);
        Pickup pickup = groundObject.GetComponent<Pickup>();
        pickup.inventory.isFull[indexValue] = false;
    }

}
