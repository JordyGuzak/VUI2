using UnityEngine;

public class InteractionController : MonoBehaviour {

    public float maxDistance = 3f;
    Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update () {
		
        if (InputManager.GetKeyDown(Actions.INTERACT))
        {
            RaycastHit hit;
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, maxDistance, (1 << LayerMask.NameToLayer("Interactable"))))
            {
                Interactable interactable = hit.transform.GetComponent<Interactable>();
                if (interactable)
                {
                    interactable.Interact();
                }
            }
        }
    }
}
