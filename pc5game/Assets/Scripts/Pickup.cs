using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// script to be added to items that the player can pick up
public class Pickup : MonoBehaviour
{
    private Inventory inventory;
    public GameObject itemButton;
    // Start is called before the first frame update
    void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();

        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("I collided with the object!");
       if (other.CompareTag("Player"))
        {   //check to see if inventory is full or not
            for (int i = 0; i < inventory.slots.Length; i++)
            {
                if(inventory.isFull[i] == false)
                { // let's fill it
                    inventory.isFull[i] = true;
                    Instantiate(itemButton,inventory.slots[i].transform,false); //instantiates it at the inventory slot
                    Destroy(gameObject);
                    break;
                }
            }

        } 
    }


}
