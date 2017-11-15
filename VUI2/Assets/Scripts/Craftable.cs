using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ItemData))]
public class Craftable : MonoBehaviour {

    public GameObject collidingObject;
    public ItemData itemData;

    void Start()
    {
        itemData = GetComponent<ItemData>();
    }

    public virtual void Craft()
    {
        Debug.Log("Craft() called from the base class");
    }

    void SetCollidingObject(Collider col)
    {
        if (collidingObject || !col.GetComponent<Rigidbody>())
        {
            return;
        }

        collidingObject = col.gameObject;
    }

    public void OnTriggerEnter(Collider other)
    {
        SetCollidingObject(other);
    }

    public void OnTriggerStay(Collider other)
    {
        SetCollidingObject(other);
    }

    public void OnTriggerExit(Collider other)
    {
        if (!collidingObject)
        {
            return;
        }

        collidingObject = null;
    }
}
