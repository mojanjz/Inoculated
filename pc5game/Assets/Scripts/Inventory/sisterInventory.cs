using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sisterInventory : MonoBehaviour
{
    [SerializeField] private InventoryKeyMap inventoryKeyMap;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        getInput();
    }

    private void getInput()
    {

        if (Input.GetKey(inventoryKeyMap.RightCell))
        {

        }
        if (Input.GetKey(inventoryKeyMap.LeftCell))
        {

        }
        if (Input.GetKey(inventoryKeyMap.UseObject))
        {
            Debug.Log("use the object");
        }

    }
}
