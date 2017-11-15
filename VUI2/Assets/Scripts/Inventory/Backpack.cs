using UnityEngine;

public class Backpack : MonoBehaviour {

    public SteamVR_TrackedObject trackedObject;

    private ItemObject currentItemObject = null;

    private bool inventoryOpen = false;

    public SteamVR_Controller.Device Controller
    {
        get
        {
            return SteamVR_Controller.Input((int)trackedObject.index);
        }
    }


    void Update()
    {
        if (!currentItemObject) return;

        if (Controller.GetHairTriggerUp())
        {
            EventManager.Instance.Invoke(new PickUpItemEvent(currentItemObject.GetItem()));
            Destroy(currentItemObject.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        SetItemData(other);
    }

    private void OnTriggerStay(Collider other)
    {
        //SetItemData(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!currentItemObject)
        {
            return;
        }

        currentItemObject = null;
    }

    void SetItemData(Collider col)
    {
        if (currentItemObject || !col.GetComponent<Rigidbody>())
        {
            return;
        }

        ItemObject itemObject = col.GetComponent<ItemObject>();

        if (itemObject) currentItemObject = itemObject;
    }
}
