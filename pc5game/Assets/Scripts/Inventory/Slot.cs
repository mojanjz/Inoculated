using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class Slot : MonoBehaviour
{
    private GameObject groundObject;
    public void DropItem()
    {
        foreach (Transform child in transform)
        {
            Pickup pickup = groundObject.GetComponent<Pickup>();
            groundObject.transform.position = pickup.inventory.Player.transform.position; // Set object on ground where the player is standing
            pickup.WasDropped = true;
            groundObject.SetActive(true);

            resetSlot();
        }
    }
    public void useItem()
    {
        if(groundObject == null)
        {
            return;
        }
        Pickup pickup = groundObject.GetComponent<Pickup>();
        GameObject player = pickup.inventory.Player;
        Debug.Log(pickup.inventory.gameObject);
        CharacterStats charStat = player.GetComponent<CharacterStats>();

        bool isUsed = false;
        string itemType = groundObject.tag;
        switch (itemType){
            case "healthPotion":
                Debug.Log("health potion case statement");
                charStat.updateHealth(10);
                isUsed = true;
                break;
            case "key":
                isUsed = player.GetComponent<Unlocker>().Unlock(groundObject);
                break;
        }

        // Only result if used
        if (isUsed)
        {
            resetSlot();
        }
    }

    // resets slot so it's reusable
    public void resetSlot()
    {
        foreach (Transform child in transform)
        {
            string slotName = transform.gameObject.name; 
            int indexValue = int.Parse(Regex.Match(slotName, @"\d+").Value);
            Pickup pickup = groundObject.GetComponent<Pickup>();
            pickup.inventory.isFull[indexValue] = false;

            if (child != null)
            {
                Destroy(child.gameObject);
            }

            groundObject = null;
        }
    }

    public void SaveGroundItem(GameObject item)
    {
        groundObject = item;
    }
}
