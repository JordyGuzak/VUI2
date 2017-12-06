using UnityEngine;

public class SnapZone : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Preview"))
        {
            EventManager.Instance.Invoke(new SnapZoneEnterEvent(this, other.gameObject));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Preview"))
        {
            EventManager.Instance.Invoke(new SnapZoneExitEvent(this, other.gameObject));
        }
    }
}
