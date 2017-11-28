using System;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class VRTK_BuildController : MonoBehaviour {

    public VRTK_ControllerEvents events;
    public float maxBuildDistance = 50f;
    public List<BuildObject> buildObjects = new List<BuildObject>();

    private BuildObject selectedBuildObject;
    private int selectedIndex = 0;
    private GameObject currentPreview;
    private Vector3 targetPos;
    private Transform targetSnapZone = null;

    protected virtual void Awake()
    {
        Initialize();
    }

    protected virtual void Initialize()
    {
        if (events == null)
        {
            events = GetComponentInParent<VRTK_ControllerEvents>();
        }

        SelectBuildObject(selectedIndex);
    }

    protected virtual void OnEnable()
    {
        if (events == null)
        {
            VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_NOT_INJECTED, "BuildController", "VRTK_ControllerEvents", "events", "the parent"));
            return;
        }
        else
        {
            events.TriggerClicked += new ControllerInteractionEventHandler(DoTriggerClicked);
        }
    }

    protected virtual void OnDisable()
    {
        events.TriggerClicked -= new ControllerInteractionEventHandler(DoTriggerClicked);
    }

    private void DoTriggerClicked(object sender, ControllerInteractionEventArgs e)
    {
        Vector3 buildPos = targetSnapZone != null ? targetSnapZone.position : targetPos;
        Build(selectedBuildObject.prefab, buildPos, currentPreview.transform.rotation);
    }

    private void Update()
    {
        UpdatePreview();
    }

    public void SelectNextBuildObject()
    {
        selectedIndex++;

        if (selectedIndex > buildObjects.Count - 1)
            selectedIndex = 0;

        SelectBuildObject(selectedIndex);
    }

    public void SelectPreviousBuildObject()
    {
        selectedIndex--;

        if (selectedIndex < 0)
            selectedIndex = buildObjects.Count - 1;

        SelectBuildObject(selectedIndex);
    }

    void SelectBuildObject(int index)
    {
        if (index < 0 || index > (buildObjects.Count - 1)) return; //index out of range

        selectedBuildObject = buildObjects[index];
        SpawnPreview();
    }

    void SpawnPreview()
    {
        if (currentPreview)
            Destroy(currentPreview);

        currentPreview = Instantiate(selectedBuildObject.preview, targetPos, Quaternion.identity);
        //SetLayerRecursively(currentPreview, LayerMask.NameToLayer("Preview"));
        currentPreview.layer = LayerMask.NameToLayer("Preview");
    }

    void UpdatePreview()
    {
        if (currentPreview == null) return;

        UpdateTargetPosition();
        currentPreview.transform.position = targetPos;
    }

    void UpdateTargetPosition()
    {
        RaycastHit hit;
        if (Physics.Raycast(events.transform.position, events.transform.forward, out hit, maxBuildDistance, ~(1 << currentPreview.layer)))
        {
            targetPos = hit.point;
            targetSnapZone = null;

            if (hit.transform.tag.Equals("SnapZone"))
            {
                targetSnapZone = hit.transform;
            }
        }
    }

    void RotatePreview()
    {
        currentPreview.transform.Rotate(new Vector3(0, 90, 0));
    }

    void Build(GameObject obj, Vector3 pos, Quaternion rot, Transform parent = null)
    {
        GameObject newStructure = Instantiate(obj, pos, rot, parent);
        newStructure.layer = LayerMask.NameToLayer("Building");
    }

    void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }

}
