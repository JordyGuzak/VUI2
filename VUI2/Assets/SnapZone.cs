using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapZone : MonoBehaviour {

    public GameObject highlightPrefab;
    private GameObject highlightObject;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Preview"))
        {
            if (highlightObject == null && highlightPrefab != null)
                highlightObject = Instantiate(highlightPrefab, transform);
        }
    }

    //private void OnTriggerStay(Collider other)
    //{
    //    if (other.gameObject.layer == LayerMask.NameToLayer("Preview"))
    //    {
    //        if 
    //    }
    //}

    private void OnTriggerExit(Collider other)
    {
        if (highlightObject != null)
            Destroy(highlightObject);
    }
}
