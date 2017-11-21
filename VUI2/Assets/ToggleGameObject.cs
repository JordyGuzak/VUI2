using UnityEngine;

public class ToggleGameObject : MonoBehaviour {
	
	public void ToggleActiveSelf()
    {
        transform.gameObject.SetActive(!transform.gameObject.activeSelf);
    }
}
