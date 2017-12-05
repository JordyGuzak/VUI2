using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using VRTK.Highlighters;

/// <summary>
/// Event Payload
/// </summary>
/// <param name="snappedObject">The interactable object that is dealing with the snap drop zone.</param>
public struct SnapZoneEventArgs
{
    public GameObject snappedObject;
}

/// <summary>
/// Event Payload
/// </summary>
/// <param name="sender">this object</param>
/// <param name="e"><see cref="SnapZoneEventArgs"/></param>
public delegate void SnapZoneEventHandler(object sender, SnapZoneEventArgs e);


public class SnapZone : MonoBehaviour {

    public GameObject highlightObjectPrefab;
    public bool displayDropZoneInEditor = true;
    public bool highlightAlwaysActive = false;
    public Color highlightColor;
    protected GameObject previousPrefab;
    protected GameObject highlightContainer;
    protected GameObject highlightObject;
    protected GameObject highlightEditorObject = null;
    protected VRTK_BaseHighlighter objectHighlighter;
    protected bool isHighlighted = false;

    /// <summary>
    /// Emitted when a valid interactable object enters the snap drop zone trigger collider.
    /// </summary>
    public event SnapZoneEventHandler ObjectEnteredSnapZone;
    /// <summary>
    /// Emitted when a valid interactable object exists the snap drop zone trigger collider.
    /// </summary>
    public event SnapZoneEventHandler ObjectExitedSnapZone;
    /// <summary>
    /// Emitted when an interactable object is successfully snapped into a drop zone.
    /// </summary>
    public event SnapZoneEventHandler ObjectSnappedToZone;
    /// <summary>
    /// Emitted when an interactable object is removed from a snapped drop zone.
    /// </summary>
    public event SnapZoneEventHandler ObjectUnsnappedFromZone;

    protected const string HIGHLIGHT_CONTAINER_NAME = "HighlightContainer";
    protected const string HIGHLIGHT_OBJECT_NAME = "HighlightObject";
    protected const string HIGHLIGHT_EDITOR_OBJECT_NAME = "EditorHighlightObject";

    protected virtual void Awake()
    {
        if (Application.isPlaying)
        {
            InitaliseHighlightObject();
        }
    }

    private void OnEnable()
    {
        ObjectEnteredSnapZone += new SnapZoneEventHandler(DoObjectEnteredSnapZone);
    }

    private void OnDisable()
    {
        ObjectEnteredSnapZone -= new SnapZoneEventHandler(DoObjectEnteredSnapZone);
    }

    protected virtual void Update()
    {
        CheckPrefabUpdate();
        CreateHighlightersInEditor();
        //set reference to current highlightObjectPrefab
        previousPrefab = highlightObjectPrefab;
        SetObjectHighlight();
    }

    /// <summary>
    /// The InitaliseHighlightObject method sets up the highlight object based on the given Highlight Object Prefab.
    /// </summary>
    /// <param name="removeOldObject">If this is set to true then it attempts to delete the old highlight object if it exists. Defaults to `false`</param>
    public virtual void InitaliseHighlightObject(bool removeOldObject = false)
    {
        //force delete previous created highlight object
        if (removeOldObject)
        {
            DeleteHighlightObject();
        }
        //Always remove editor highlight object at runtime
        ChooseDestroyType(transform.Find(ObjectPath(HIGHLIGHT_EDITOR_OBJECT_NAME)));
        highlightEditorObject = null;

        GenerateObjects();
    }

    protected virtual void GenerateObjects()
    {
        GenerateHighlightObject();
        if (highlightObject != null && objectHighlighter == null)
        {
            InitialiseHighlighter();
        }
    }

    protected virtual void InitialiseHighlighter()
    {
        VRTK_BaseHighlighter existingHighlighter = VRTK_BaseHighlighter.GetActiveHighlighter(gameObject);
        //If no highlighter is found on the GameObject then create the default one
        if (existingHighlighter == null)
        {
            highlightObject.AddComponent<VRTK_MaterialColorSwapHighlighter>();
        }
        else
        {
            VRTK_SharedMethods.CloneComponent(existingHighlighter, highlightObject);
        }

        //Initialise highlighter and set highlight colour
        objectHighlighter = highlightObject.GetComponent<VRTK_BaseHighlighter>();
        objectHighlighter.unhighlightOnDisable = false;
        objectHighlighter.Initialise(highlightColor);
        objectHighlighter.Highlight(highlightColor);

        //if the object highlighter is using a cloned object then disable the created highlight object's renderers
        if (objectHighlighter.UsesClonedObject())
        {
            foreach (Renderer renderer in GetComponentsInChildren<Renderer>(true))
            {
                if (!VRTK_PlayerObject.IsPlayerObject(renderer.gameObject, VRTK_PlayerObject.ObjectTypes.Highlighter))
                {
                    renderer.enabled = false;
                }
            }
        }
    }


    public virtual void OnObjectEnteredSnapZone(SnapZoneEventArgs e)
    {
        if (ObjectEnteredSnapZone != null)
        {
            ObjectEnteredSnapZone(this, e);
        }
    }

    public virtual void OnObjectExitedSnapZone(SnapZoneEventArgs e)
    {
        if (ObjectExitedSnapZone != null)
        {
            ObjectExitedSnapZone(this, e);
        }
    }

    public virtual SnapZoneEventArgs SetSnapDropZoneEvent(GameObject interactableObject)
    {
        SnapZoneEventArgs e;
        e.snappedObject = interactableObject;
        return e;
    }

    private void DoObjectEnteredSnapZone(object sender, SnapZoneEventArgs e)
    {
        if (highlightObject == null && highlightObjectPrefab != null)
            highlightObject = Instantiate(highlightObjectPrefab, transform);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Preview"))
        {
            OnObjectEnteredSnapZone(SetSnapDropZoneEvent(other.gameObject));
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

    protected virtual void CheckPrefabUpdate()
    {
        //If the highlightObjectPrefab has changed then delete the highlight object in preparation to create a new one
        if (previousPrefab != null && previousPrefab != highlightObjectPrefab)
        {
            DeleteHighlightObject();
        }
    }

    protected virtual void GenerateHighlightObject()
    {
        //If there is a given highlight prefab and no existing highlight object then create a new highlight object
        if (highlightObjectPrefab != null && highlightObject == null && transform.Find(ObjectPath(HIGHLIGHT_OBJECT_NAME)) == null)
        {
            CopyObject(highlightObjectPrefab, ref highlightObject, HIGHLIGHT_OBJECT_NAME);
        }

        //if highlight object exists but not in the variable then force grab it
        Transform checkForChild = transform.Find(ObjectPath(HIGHLIGHT_OBJECT_NAME));
        if (checkForChild != null && highlightObject == null)
        {
            highlightObject = checkForChild.gameObject;
        }

        //if no highlight object prefab is set but a highlight object is found then destroy the highlight object
        if (highlightObjectPrefab == null && highlightObject != null)
        {
            DeleteHighlightObject();
        }

        if (highlightObject != null)
        {
            highlightObject.SetActive(false);
        }
        SetContainer();
    }

    protected virtual void DeleteHighlightObject()
    {
        ChooseDestroyType(transform.Find(HIGHLIGHT_CONTAINER_NAME));
        highlightContainer = null;
        highlightObject = null;
        objectHighlighter = null;
    }

    protected virtual void CopyObject(GameObject objectBlueprint, ref GameObject clonedObject, string givenName)
    {
        GenerateContainer();
        Vector3 saveScale = transform.localScale;
        transform.localScale = Vector3.one;

        clonedObject = Instantiate(objectBlueprint, highlightContainer.transform) as GameObject;
        clonedObject.name = givenName;

        //default position of new highlight object
        clonedObject.transform.localPosition = Vector3.zero;
        clonedObject.transform.localRotation = Quaternion.identity;

        transform.localScale = saveScale;
        CleanHighlightObject(clonedObject);
    }

    protected virtual void GenerateContainer()
    {
        if (highlightContainer == null || transform.Find(HIGHLIGHT_CONTAINER_NAME) == null)
        {
            highlightContainer = new GameObject(HIGHLIGHT_CONTAINER_NAME);
            highlightContainer.transform.SetParent(transform);
            highlightContainer.transform.localPosition = Vector3.zero;
            highlightContainer.transform.localRotation = Quaternion.identity;
            highlightContainer.transform.localScale = Vector3.one;
        }
    }

    protected virtual void CreateHighlightersInEditor()
    {
        //Only run if it's in the editor
        if (VRTK_SharedMethods.IsEditTime())
        {
            //Generate the main highlight object
            GenerateHighlightObject();

            //Generate the editor highlighter object with the custom material
            GenerateEditorHighlightObject();

            //Ensure the game object references are force set based on whether they exist in the path
            ForceSetObjects();

            //Show the editor highlight object if it's set.
            if (highlightEditorObject != null)
            {
                highlightEditorObject.SetActive(displayDropZoneInEditor);
            }
        }
    }

    protected virtual void GenerateEditorHighlightObject()
    {
        if (highlightObject != null && highlightEditorObject == null && transform.Find(ObjectPath(HIGHLIGHT_EDITOR_OBJECT_NAME)) == null)
        {
            CopyObject(highlightObject, ref highlightEditorObject, HIGHLIGHT_EDITOR_OBJECT_NAME);
            foreach (Renderer renderer in highlightEditorObject.GetComponentsInChildren<Renderer>())
            {
                renderer.material = Resources.Load("SnapDropZoneEditorObject") as Material;
            }
            highlightEditorObject.SetActive(true);
        }
    }

    protected virtual void SetObjectHighlight()
    {
        if (highlightAlwaysActive && !isHighlighted)
        {
            highlightObject.SetActive(true);
        }
    }

    protected virtual string ObjectPath(string name)
    {
        return HIGHLIGHT_CONTAINER_NAME + "/" + name;
    }

    protected virtual void SetContainer()
    {
        Transform findContainer = transform.Find(HIGHLIGHT_CONTAINER_NAME);
        if (findContainer != null)
        {
            highlightContainer = findContainer.gameObject;
        }
    }

    protected virtual void ForceSetObjects()
    {
        if (highlightEditorObject == null)
        {
            Transform forceFindHighlightEditorObject = transform.Find(ObjectPath(HIGHLIGHT_EDITOR_OBJECT_NAME));
            highlightEditorObject = (forceFindHighlightEditorObject ? forceFindHighlightEditorObject.gameObject : null);
        }

        if (highlightObject == null)
        {
            Transform forceFindHighlightObject = transform.Find(ObjectPath(HIGHLIGHT_OBJECT_NAME));
            highlightObject = (forceFindHighlightObject ? forceFindHighlightObject.gameObject : null);
        }

        if (highlightContainer == null)
        {
            Transform forceFindHighlightContainer = transform.Find(HIGHLIGHT_CONTAINER_NAME);
            highlightContainer = (forceFindHighlightContainer ? forceFindHighlightContainer.gameObject : null);
        }
    }

    protected virtual void CleanHighlightObject(GameObject objectToClean)
    {
        //If the highlight object has any child snap zones, then force delete these
        var deleteSnapZones = objectToClean.GetComponentsInChildren<VRTK_SnapDropZone>(true);
        for (int i = 0; i < deleteSnapZones.Length; i++)
        {
            ChooseDestroyType(deleteSnapZones[i].gameObject);
        }

        //determine components that shouldn't be deleted from highlight object
        string[] validComponents = new string[] { "Transform", "MeshFilter", "MeshRenderer", "SkinnedMeshRenderer", "VRTK_GameObjectLinker" };

        //go through all of the components on the highlighted object and delete any components that aren't in the valid component list
        var components = objectToClean.GetComponentsInChildren<Component>(true);
        for (int i = 0; i < components.Length; i++)
        {
            Component component = components[i];
            var valid = false;

            //Loop through each valid component and check to see if this component is valid
            foreach (string validComponent in validComponents)
            {
                //if it's a valid component then break the check
                if (component.GetType().ToString().Contains("." + validComponent))
                {
                    valid = true;
                    break;
                }
            }

            //if this is a valid component then just continue to the next component
            if (valid)
            {
                continue;
            }

            //If not a valid component then delete it
            ChooseDestroyType(component);
        }
    }

    protected virtual void ChooseDestroyType(Transform deleteTransform)
    {
        if (deleteTransform != null)
        {
            ChooseDestroyType(deleteTransform.gameObject);
        }
    }

    protected virtual void ChooseDestroyType(GameObject deleteObject)
    {
        if (VRTK_SharedMethods.IsEditTime())
        {
            if (deleteObject != null)
            {
                DestroyImmediate(deleteObject);
            }
        }
        else
        {
            if (deleteObject != null)
            {
                Destroy(deleteObject);
            }
        }
    }

    protected virtual void ChooseDestroyType(Component deleteComponent)
    {
        if (VRTK_SharedMethods.IsEditTime())
        {
            if (deleteComponent != null)
            {
                DestroyImmediate(deleteComponent);
            }
        }
        else
        {
            if (deleteComponent != null)
            {
                Destroy(deleteComponent);
            }
        }
    }

    protected virtual void OnDrawGizmosSelected()
    {
        if (highlightObject != null && !displayDropZoneInEditor)
        {
            Vector3 boxSize = VRTK_SharedMethods.GetBounds(highlightObject.transform).size * 1.05f;
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(highlightObject.transform.position, boxSize);
        }
    }
}
