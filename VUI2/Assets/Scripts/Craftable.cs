using UnityEngine;

[RequireComponent(typeof(ItemObject))]
public class Craftable : MonoBehaviour {

    [HideInInspector] public GameObject collidingObject;
    [HideInInspector] public ItemObject itemObject;

    void Start()
    {
        itemObject = GetComponent<ItemObject>();
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
