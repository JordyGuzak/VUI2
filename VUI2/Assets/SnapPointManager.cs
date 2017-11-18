using UnityEngine;

public class SnapPointManager : MonoBehaviour {

    [HideInInspector]
    public Transform[] SnapPoints;

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        SnapPoints = new Transform[transform.childCount];

        for (int i = 0; i < SnapPoints.Length; i++)
        {
            SnapPoints[i] = transform.GetChild(i);
        }
    }

    public Transform GetNearestSnapPoint(Vector3 pos)
    {
        float nearestDistance = Vector3.Distance(pos, SnapPoints[0].position);
        int nearestIndex = 0;

        for (int i = 1; i < SnapPoints.Length; i++)
        {
            var currDistance = Vector3.Distance(pos, SnapPoints[i].position);

            if (currDistance < nearestDistance)
            {
                nearestDistance = currDistance;
                nearestIndex = i;
            }
        }

        return SnapPoints[nearestIndex];
    }
}
