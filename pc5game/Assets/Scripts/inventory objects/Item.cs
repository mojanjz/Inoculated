using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class Item
//{
//    public Item (int id=0, string name=null, GameObject model=null)
//    {
//        ID = id;
//        Name = name;
//        Model = model;
//    }

//    public virtual void useItem()
//    {
//        Debug.Log("should override useItem for each object");
//    }

//    public int ID { get; set; }
//    public string Name { get; set; }
//    public GameObject Model { get; set; }
//}

interface Item
{
    void useItem(); //error
}


