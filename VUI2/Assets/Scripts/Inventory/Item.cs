using UnityEngine;

[System.Serializable]
public class Item {

    public Sprite Sprite;
    public string Title;
    public string Description;
    public int Amount;
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

    public Item Copy()
    {
        return new Item(Sprite, Title, Description, Amount, Stackable);
    }
}
