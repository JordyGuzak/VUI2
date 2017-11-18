using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildController : MonoBehaviour {

    public float maxSnapDistance = 0.5f;
    public List<BuildObject> buildObjects = new List<BuildObject>();
    private BuildObject selectedBuildObject;
    private GameObject currentPreview;
    private Vector3 targetPos;
    private Vector3 offset = Vector3.zero;
    private SnapPointManager currentSpm;
	
	// Update is called once per frame
	void Update () {
        if (buildObjects.Count < 1) return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SelectBuildObject(0);
            SpawnPreview();
        }

        UpdatePreview();
	}

    void SelectBuildObject(int index)
    {
        if (index < 0 || index > (buildObjects.Count - 1)) return; //index out of range

        selectedBuildObject = buildObjects[index];
    }

    void SpawnPreview()
    {
        if (currentPreview)
            Destroy(currentPreview);

        currentPreview = Instantiate(selectedBuildObject.preview, targetPos, Quaternion.identity);
        SetLayerRecursively(currentPreview, LayerMask.NameToLayer("Preview"));
        currentSpm = currentPreview.GetComponent<SnapPointManager>();

        if (currentSpm)
        {
            currentSpm.Init();
        }
    }

    void UpdatePreview()
    {
        if (!currentPreview) return;

        UpdateTargetPosition();
        currentPreview.transform.position = targetPos;

        if (Input.GetMouseButtonDown(0))
        {
            Build(selectedBuildObject.prefab, targetPos, currentPreview.transform.rotation);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            currentPreview.transform.Rotate(new Vector3(0, 90, 0));
        }
    }

    void UpdateTargetPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 50f, ~(1 << currentPreview.layer)))
        {
            targetPos = hit.point;
            offset = Vector3.zero;
            Transform nearestSnapPoint = null;

            Collider[] hitColliders = Physics.OverlapSphere(targetPos, maxSnapDistance);
            for (int i = 0; i < hitColliders.Length; i++)
            {
                SnapPointManager spm = hitColliders[i].GetComponent<SnapPointManager>();
                if (spm && spm != currentSpm)
                {
                    Transform snapPoint = spm.GetNearestSnapPoint(targetPos);
                    float distance = Vector3.Distance(targetPos, snapPoint.position);

                    if (distance <= maxSnapDistance)
                    {
                        if (nearestSnapPoint == null || Vector3.Distance(targetPos, nearestSnapPoint.position) > distance)
                        {
                            nearestSnapPoint = snapPoint;
                        }
                    }
                }
            }

            if (nearestSnapPoint)
            {
                offset = currentPreview.GetComponent<Collider>().bounds.size / 2;
                offset = new Vector3(offset.x * nearestSnapPoint.forward.x, offset.y * nearestSnapPoint.forward.y, offset.z * nearestSnapPoint.forward.z);
                targetPos = nearestSnapPoint.position + offset;
            }
        }
    }

    void Build(GameObject obj, Vector3 pos, Quaternion rot, Transform parent = null)
    {
        SetLayerRecursively(Instantiate(obj, pos, rot, parent), LayerMask.NameToLayer("Building"));
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
