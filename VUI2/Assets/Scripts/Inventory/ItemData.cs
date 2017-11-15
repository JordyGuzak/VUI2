using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemData : MonoBehaviour {

    public Sprite sprite;
    public string title;
    public string description;
    public int amount = 1;
    public bool stackable;

    public Item GetItem()
    {
        return new Item(sprite, title, description, amount, stackable);
    }
}
