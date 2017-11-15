using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothFollow : MonoBehaviour {

    public Transform target;
    public Vector3 offset;
    public Vector3 rotation;
    public float distance = 5f;
    public float smoothing = 5f;
	
	// Update is called once per frame
	void LateUpdate () {
		if (target)
        {
            Vector3 targetPos = target.position + offset;
            Vector3 newPos = Vector3.Lerp(transform.position, targetPos, smoothing * Time.deltaTime);
            transform.position = newPos;
            transform.localEulerAngles = rotation;
        }
	}
}
