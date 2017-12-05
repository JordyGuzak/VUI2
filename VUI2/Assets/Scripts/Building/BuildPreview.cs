using System.Collections.Generic;
using UnityEngine;

public class BuildPreview : MonoBehaviour {

    public Material green;
    public Material red;

    private List<Collider> cols = new List<Collider>();
    private bool canBuild = true;

    public bool CanBuild
    {
        get { return canBuild; }
        set
        {
            canBuild = value;

            if (canBuild)
            {
                SetMaterial(green);
            }
            else
            {
                SetMaterial(red);
            }

            EventManager.Instance.Invoke(new CanBuildChangedEvent(this, canBuild));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Building"))
        {
            cols.Add(other);

            if (CanBuild) CanBuild = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (cols.Contains(other))
        {
            cols.Remove(other);

            if (cols.Count == 0 && CanBuild == false)
            {
                CanBuild = true;
            }
        }
    }

    void SetMaterial(Material mat)
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material = mat;
        }
    }
}
