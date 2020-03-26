using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// script to be added to items that the player can pick up
public class Pickup : MonoBehaviour
{
    public PlayerInventory inventory;
    public GameObject itemButton;
    [SerializeField] private Canvas canvas;
    public bool WasDropped = false;

    // Start is called before the first frame update
    void Start()
    {
        //itemButton.AddComponent<Button>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (WasDropped)
        {
            WasDropped = false;
            return;
        }

        //Debug.Log("I collided with the object!");
       if (other.CompareTag("Player"))
        {
            // Get the inventory of the specific player
            inventory = other.GetComponent<CharacterStats>().Inventory;
            
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
                    GameObject currentSlot = findSlot(i, inventory);
                    currentSlot.GetComponent<Slot>().SaveGroundItem(gameObject);
                    gameObject.SetActive(false);
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
    GameObject findSlot(int i, PlayerInventory inventory)
    {
        Transform trans = inventory.gameObject.transform;
        Transform childTrans = trans.Find("Slot (" + (i).ToString() + ")");
        return childTrans.gameObject;
    }

}
