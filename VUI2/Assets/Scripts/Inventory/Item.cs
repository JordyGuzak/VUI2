using UnityEngine;

public class Item {

    public Sprite Sprite { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int Amount { get; set; }
    public bool Stackable;

    public Item()
    {
    }

    public Item(Sprite sprite, string title, string description, int amount, bool stackable)
    {
        Sprite = sprite;
        Title = title;
        Description = description;
        Amount = amount;
        Stackable = stackable;
    }
}
