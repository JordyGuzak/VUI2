using UnityEngine;

public class ItemObject : MonoBehaviour {

    [SerializeField]
    private Item item = new Item();

    public Item GetItem()
    {
        return item.Copy();
    }
}
