using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserPointer : MonoBehaviour {

    private SteamVR_TrackedObject trackedObj;

    public GameObject laserPrefab;
    private GameObject laser;
    private Transform laserTransform;
    private Vector3 hitPoint;

    public Transform cameraRigTransform; //transform of [CameraRig].
    public GameObject teleportReticlePrefab; //Stores a reference to the teleport reticle prefab.
    private GameObject reticle; //A reference to an instance of the reticle.
    private Transform teleportReticleTransform; //Stores a reference to the teleport reticle transform for ease of use
    public Transform headTransform; //Stores a reference to the player’s head (the camera).
    public Vector3 teleportReticleOffset; //Is the reticle offset from the floor, so there’s no “Z-fighting” with other surfaces.
    public LayerMask teleportMask; //Is a layer mask to filter the areas on which teleports are allowed
    private bool shouldTeleport; //Is set to true when a valid teleport location is found.

    BlinkEffect blinkEffect;

    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        blinkEffect = GetComponent<BlinkEffect>();
    }

    void Start()
    {
        laser = Instantiate(laserPrefab);
        laserTransform = laser.transform;
        reticle = Instantiate(teleportReticlePrefab);
        teleportReticleTransform = reticle.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (Controller.GetPress(SteamVR_Controller.ButtonMask.Touchpad))
        {
            RaycastHit hit;

            if (Physics.Raycast(trackedObj.transform.position, transform.forward, out hit, 100, teleportMask))
            {
                hitPoint = hit.point;
                ShowLaser(hit);

                // Show the teleport reticle
                reticle.SetActive(true);
                // Move the reticle to where the raycast hit with the addition of an offset to avoid Z-fighting.
                teleportReticleTransform.position = hitPoint + teleportReticleOffset;
                // Set shouldTeleport to true to indicate the script found a valid position for teleporting.
                shouldTeleport = true;
            }
        }
        else
        {
            laser.SetActive(false);
            reticle.SetActive(false);
        }

        if (Controller.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad) && shouldTeleport)
        {
            StartCoroutine(Teleport());
        }
    }

    private void ShowLaser(RaycastHit hit)
    {
        // Show the laser.
        laser.SetActive(true);

        // Position the laser between the controller and the point where the raycast hits. 
        // You use Lerp because you can give it two positions and the percent it should travel. 
        // If you pass it 0.5f, which is 50%, it returns the precise middle point.
        laserTransform.position = Vector3.Lerp(trackedObj.transform.position, hitPoint, .5f);

        // Point the laser at the position where the raycast hit.
        laserTransform.LookAt(hitPoint);

        // Scale the laser so it fits perfectly between the two positions.
        laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y,
            hit.distance);
    }

    IEnumerator Teleport()
    {
        // Set the shouldTeleport flag to false when teleportation is in progress.
        shouldTeleport = false;
        // Hide the reticle
        reticle.SetActive(false);

        StartCoroutine(blinkEffect.Blink());

        while (!blinkEffect.EyesClosed)
        {
            yield return null;
        }

        // Calculate the difference between the positions of the camera rig’s center and the player’s head.
        Vector3 difference = cameraRigTransform.position - headTransform.position;
        // Reset the y-position for the above difference to 0, because the calculation doesn’t consider the vertical position of the player’s head.
        difference.y = 0;
        // Move the camera rig to the position of the hit point and add the calculated difference. Without the difference, the player would teleport to an incorrect location. See the example below:
        cameraRigTransform.position = hitPoint + difference;
    }
}
