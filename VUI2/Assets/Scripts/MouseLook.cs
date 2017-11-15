using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour {

    public float sensitivity = 1f;

    private Camera cam;
    // Use this for initialization
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update () {
        float mouseX = Input.GetAxisRaw("Mouse X") * sensitivity;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sensitivity;

        transform.rotation *= Quaternion.Euler(new Vector3(0, mouseX, 0));
        cam.transform.rotation *= Quaternion.Euler(new Vector3(-mouseY, 0, 0));
	}
}
