using UnityEngine;

public class PickUpItemEvent : GameEvent {

    public Item Item { get; private set; }

	public PickUpItemEvent(Item item)
    {
        this.Item = item;
    }
}
