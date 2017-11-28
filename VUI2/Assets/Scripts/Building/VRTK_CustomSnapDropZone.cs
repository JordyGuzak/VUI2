using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class VRTK_CustomSnapDropZone : VRTK_SnapDropZone {


    /// <summary>
    /// The ForceUnsnap method attempts to automatically remove the current snapped game object from the snap drop zone.
    /// </summary>
    public override void ForceUnsnap()
    {
        if (isSnapped && currentSnappedObject != null)
        {
            VRTK_InteractableObject ioCheck = ValidSnapObject(currentSnappedObject, false, false);
            if (ioCheck != null)
            {
                ioCheck.ToggleSnapDropZone(this, false);
            }
        }
    }

    protected override void OnTriggerStay(Collider collider)
    {
        //Do sanity check to see if there should be a snappable object
        if (!isSnapped && ValidSnapObject(collider.gameObject, true, false))
        {
            AddCurrentValidSnapObject(collider.gameObject);
        }

        //if the current colliding object is the valid snappable object then we can snap
        if (IsObjectHovering(collider.gameObject))
        {
            //If it isn't snapped then force the highlighter back on
            if (!isSnapped)
            {
                ToggleHighlight(collider, true);
            }

            //Attempt to snap the object
            SnapObject(collider);
        }
    }

    protected override void SnapObject(Collider collider)
    {
        VRTK_InteractableObject ioCheck = ValidSnapObject(collider.gameObject, false, false);
        //If the item is in a snappable position and this drop zone isn't snapped and the collider is a valid interactable object
        if (willSnap && !isSnapped && ioCheck != null)
        {
            //Only snap it to the drop zone if it's not already in a drop zone
            if (!ioCheck.IsInSnapDropZone())
            {
                if (highlightObject != null)
                {
                    //Turn off the drop zone highlighter
                    highlightObject.SetActive(false);
                }

                Vector3 newLocalScale = GetNewLocalScale(ioCheck);
                if (transitionInPlace != null)
                {
                    StopCoroutine(transitionInPlace);
                }

                isSnapped = true;
                currentSnappedObject = ioCheck.gameObject;
                if (cloneNewOnUnsnap)
                {
                    CreatePermanentClone();
                }

                transitionInPlace = StartCoroutine(UpdateTransformDimensions(ioCheck, highlightContainer, newLocalScale, snapDuration));

                ioCheck.ToggleSnapDropZone(this, true);
            }
        }

        //Force reset isSnapped if the item is grabbed but isSnapped is still true
        isSnapped = (isSnapped && ioCheck && ioCheck.IsGrabbed() ? false : isSnapped);
        wasSnapped = false;
    }


    protected override void ToggleHighlight(Collider collider, bool state)
    {
        VRTK_InteractableObject ioCheck = ValidSnapObject(collider.gameObject, true, false);
        if (highlightObject != null && ioCheck != null)
        {
            //Toggle the highlighter state
            highlightObject.SetActive(state);
            ioCheck.SetSnapDropZoneHover(this, state);

            willSnap = state;
            isHighlighted = state;

            if (state)
            {
                if (!IsObjectHovering(collider.gameObject) || wasSnapped)
                {
                    OnObjectEnteredSnapDropZone(SetSnapDropZoneEvent(collider.gameObject));
                }
                AddCurrentValidSnapObject(collider.gameObject);
            }
            else
            {
                OnObjectExitedSnapDropZone(SetSnapDropZoneEvent(collider.gameObject));
                RemoveCurrentValidSnapObject(collider.gameObject);
            }
        }
    }
}
