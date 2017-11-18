using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildGridBased : MonoBehaviour {

    public float gridSize = 1f;
    public float offset = 1f;
    public List<BuildObject> buildObjects = new List<BuildObject>();
    private BuildObject selectedBuildObject;
    private GameObject currentPreview;
    private Vector3 targetPos;

	
	// Update is called once per frame
	void Update () {
        if (buildObjects.Count < 1) return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SelectBuildObject(0);
            SpawnPreview();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SelectBuildObject(1);
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
    }

    void UpdateTargetPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 10f, ~(1 << currentPreview.layer)))
        {
            targetPos = hit.point;
            targetPos -= Vector3.one * offset;
            targetPos /= gridSize;
            targetPos = new Vector3(Mathf.Round(targetPos.x), Mathf.Round(targetPos.y), Mathf.Round(targetPos.z));
            targetPos *= gridSize;
            targetPos += Vector3.one * offset;
        }
    }

    void Build(GameObject obj, Vector3 pos, Quaternion rot, Transform parent = null)
    {
        Instantiate(obj, pos, rot, parent);
    }
}
