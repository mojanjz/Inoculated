using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public GameObject Player;

    public bool[] isFull;
    public GameObject[] slots;

    [SerializeField] GameObject highlighter;
    [SerializeField] GameObject inventory;
    [SerializeField] KeyMap keyMap;
    int inventoryIndex = 0;
    int maxIndex = 3;
    Vector3 initialPosition;
    float slotDist;
    private int startIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        initialPosition = highlighter.transform.position;
        slotDist = findSlot(startIndex + 1).transform.position.y - findSlot(startIndex).transform.position.y; // Y distance between first two slots
    }

    // Update is called once per frame
    void Update()
    {
        getInput();
    }

    private void getInput()
    {
        if (Input.GetKeyDown(keyMap.Cycle))
        {
            Vector3 temp = new Vector3(0, slotDist, 0);
            //highlighter.GetComponent<SpriteRenderer>().enabled = true;
            if (inventoryIndex < maxIndex && inventoryIndex >= 0)
            {
                highlighter.transform.position += temp;
                inventoryIndex++;
            }
            else
            {
                inventoryIndex = startIndex;
                highlighter.transform.position = initialPosition;
            }
        }
        if (Input.GetKeyDown(keyMap.Drop))
        {
            GameObject slot = findSlot(inventoryIndex);
            slot.transform.GetComponent<Slot>().DropItem();
            //highlighter.GetComponent<SpriteRenderer>().enabled =false;
            //inventoryIndex = startIndex;
        }
        if (Input.GetKeyDown(keyMap.Use))
        {
            GameObject slot = findSlot(inventoryIndex);
            slot.transform.GetComponent<Slot>().useItem();
        }
    }
    private GameObject findSlot(int i)
    {
        Transform trans = inventory.transform;
        Transform childTrans = trans.Find("Slot (" + (i).ToString() + ")");
        return childTrans.gameObject;
    }
}
