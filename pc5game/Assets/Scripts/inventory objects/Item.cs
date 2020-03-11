using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public Item (int id=null, string name=null, GameObject model=null)
    {
        ID = id;
        Name = name;
        Model = model;
    }

    public virtual void useItem(GameObject player)
    {
        Debug.Log("should overrid useItem for each object");
    }

    public int ID { get; set; }
    public string Name { get; set; }
    public GameObject Model { get; set; }
}



