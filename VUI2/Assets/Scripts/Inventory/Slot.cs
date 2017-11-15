using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    private Item item;
    private Image image;

    public Text AmountText { get; set; }
    public Item Item
    {
        get { return item; }
        set
        {
            item = value;

            if (item != null)
            {
                image.sprite = item.Sprite;
                image.enabled = true;
            }
            else
            {
                image.enabled = false;
            }
        }
    }

    public void Init()
    {
        image = GetComponentsInChildren<Image>()[1];
        AmountText = GetComponentInChildren<Text>();
    }
}
