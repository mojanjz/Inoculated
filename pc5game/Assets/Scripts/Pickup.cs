using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// script to be added to items that the player can pick up
public class Pickup : MonoBehaviour
{
    private Inventory inventory;
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
        Debug.Log("I collided with the object!");
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

                    //Instantiate(itemButton,inventory.slots[i].transform,false); //instantiates it at the inventory slot
                    Vector3 vec = new Vector3();
                    vec = inventory.slots[i].transform.position;
                    var obj = Instantiate(itemButton, canvas.transform);
                    Debug.Log("I instantiated the itemButton");
                    obj.transform.SetParent(inventory.slots[i].transform);
                    //var obj = Instantiate(itemButton, inventory.slots[i].transform,false);
                    obj.transform.position = vec;
                    findSlot(i);
                    gameObject.SetActive(false);
                    //Destroy(gameObject);
                    break;
                }
            }

        } 
    }

    void findSlot(int i)
    {
        GameObject currentSlot = GameObject.Find("Slot(1)");
        Debug.Log("current slot is " + currentSlot);
    }

}
