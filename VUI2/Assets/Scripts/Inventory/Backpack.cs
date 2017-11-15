using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Backpack : MonoBehaviour {


    public SteamVR_TrackedObject trackedObject;

    private ItemData currentItemData = null;

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
        if (!currentItemData) return;

        if (Controller.GetHairTriggerUp())
        {
            EventManager.Instance.Invoke(
                new PickUpItemEvent(
                new Item(
                    currentItemData.sprite,
                    currentItemData.title,
                    currentItemData.description,
                    currentItemData.amount,
                    currentItemData.stackable
                )));

            Destroy(currentItemData.gameObject);
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
        if (!currentItemData)
        {
            return;
        }

        currentItemData = null;
    }

    void SetItemData(Collider col)
    {
        if (currentItemData || !col.GetComponent<Rigidbody>())
        {
            return;
        }

        ItemData itemData = col.GetComponent<ItemData>();

        if (itemData) currentItemData = itemData;
    }
}
