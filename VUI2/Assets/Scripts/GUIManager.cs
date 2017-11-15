using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIManager : MonoBehaviour {


    public GameObject inventory;

    void OnEnable()
    {
        EventManager.Instance.AddListener<OpenInventoryEvent>(OnOpenInventory);
        EventManager.Instance.AddListener<CloseInventoryEvent>(OnCloseInventory);
    }

    void OnDisable()
    {
        EventManager.Instance.RemoveListener<OpenInventoryEvent>(OnOpenInventory);
        EventManager.Instance.RemoveListener<CloseInventoryEvent>(OnCloseInventory);
    }
	
	// Update is called once per frame
	void Update () {
		
        if (inventory)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                EventManager.Instance.Invoke(new OpenInventoryEvent());
            }

            if (Input.GetKeyUp(KeyCode.Tab))
            {
                EventManager.Instance.Invoke(new CloseInventoryEvent());
            }
        }
	}

    void OnOpenInventory(OpenInventoryEvent e)
    {
        inventory.SetActive(true);
    }

    void OnCloseInventory(CloseInventoryEvent e)
    {
        inventory.SetActive(false);
    }
}
