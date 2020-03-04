using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// script to be added to items that the player can pick up
public class Pickup : MonoBehaviour
{
    public Inventory inventory;
    public GameObject itemButton;
    [SerializeField] private Canvas canvas;

    // Start is called before the first frame update
    void Start()
    {
        itemButton.AddComponent<Button>();
        // inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("I collided with the object!");
       if (other.CompareTag("Player"))
        {
            // Get the inventory of the specific player
            inventory = other.GetComponent<Inventory>();
            
            //check to see if inventory is full or not
            for (int i = 0; i < inventory.slots.Length; i++)
            {
                if(inventory.isFull[i] == false)
                { // let's fill it
                    inventory.isFull[i] = true;
                    Vector3 vec = new Vector3();
                    vec = inventory.slots[i].transform.position;
                    var obj = Instantiate(itemButton, canvas.transform);
                    obj.transform.SetParent(inventory.slots[i].transform);
                    obj.transform.position = vec;
                    GameObject currentSlot = findSlot(i,other.gameObject);
                    currentSlot.GetComponent<Slot>().inventoryObject = gameObject;
                    gameObject.SetActive(false);
                    //Destroy(gameObject);
                    break;
                }
            }

        } 
    }
    /*
     * Input 1: index of the slot of the inventory
     * Input 2: player associated with the inventory
     * Function: finds the appropriate slot number
     */
    GameObject findSlot(int i, GameObject player)
    {
        string inventoryName;
        if (string.Equals(player.name, "PlayerBro"))
        {
           inventoryName = "brother's inventory";
        } else
        {
            inventoryName = "sister's inventory";
        }
        Transform trans = GameObject.Find(inventoryName).transform;
        Transform childTrans = trans.Find("Slot (" + (i+1).ToString() + ")");
        return childTrans.gameObject;
    }

}
