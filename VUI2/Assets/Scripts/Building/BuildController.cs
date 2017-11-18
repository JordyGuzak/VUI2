using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildController : MonoBehaviour {

    public float maxSnapDistance = 0.5f;
    public GameObject[] buildPrefabs;

    private GameObject objectInHand;
    private SnapPointManager spmInHand;
	
	// Update is called once per frame
	void Update () {
        if (buildPrefabs.Length < 1) return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SpawnObject(0);
        }

        if (Input.GetMouseButtonDown(0))
        {
            Release();
        }

        if (objectInHand)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 50f, ~(1 << objectInHand.layer)))
            {
                Vector3 targetPos = hit.point;
                Vector3 offset = spmInHand.SnapPoints[0].localPosition;
                Transform nearestSnapPoint = null;

                Collider[] hitColliders = Physics.OverlapSphere(targetPos, maxSnapDistance);
                for (int i = 0; i < hitColliders.Length; i++)
                {
                    SnapPointManager spm = hitColliders[i].GetComponent<SnapPointManager>();
                    if (spm && spm != spmInHand)
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
                    targetPos = nearestSnapPoint.position;
                    offset = spmInHand.GetNearestSnapPoint(targetPos).localPosition;
                }        

                objectInHand.transform.position = targetPos - offset;
            }
        }
	}

    void SpawnObject(int index)
    {
        if (objectInHand)
            Destroy(objectInHand);

        objectInHand = Instantiate(buildPrefabs[index]);
        SetLayerRecursively(objectInHand, LayerMask.NameToLayer("Building"));

        spmInHand = objectInHand.GetComponent<SnapPointManager>();
        if (spmInHand)
        {
            spmInHand.Init();
        }
    }

    void Release()
    {
        if (!objectInHand) return;

        SetLayerRecursively(objectInHand, 0);// Default layer
        objectInHand = null;
        spmInHand = null;
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
