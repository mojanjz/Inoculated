using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sisterInventory : MonoBehaviour
{
    GameObject highlighter;
    int inventoryIndex = 0;
    int maxIndex = 4;
    Vector3 initialPosition;
    // Start is called before the first frame update
    void Start()
    {
        highlighter = GameObject.FindGameObjectWithTag("InventoryHighLighter");
        initialPosition = highlighter.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        getInput();
    }

    private void getInput()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            Vector3 temp = new Vector3(0, 100.0f * Time.deltaTime, 0);
            highlighter.GetComponent<SpriteRenderer>().enabled = true;
            if (inventoryIndex < maxIndex + 1 && inventoryIndex > 0)
            {
                highlighter.transform.position += temp;
            }
            else
            {
                inventoryIndex = 1;
                highlighter.transform.position = initialPosition;
            }
            inventoryIndex += 1;
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            GameObject slot = findSlot(inventoryIndex-1);
            slot.transform.GetComponent<Slot>().DropItem();
            highlighter.GetComponent<SpriteRenderer>().enabled =false;
            inventoryIndex = 0;
        }

    }
    private GameObject findSlot(int i)
    {
        Transform trans = GameObject.Find("sister's inventory").transform;
        Transform childTrans = trans.Find("Slot (" + (i).ToString() + ")");
        return childTrans.gameObject;
    }
}
