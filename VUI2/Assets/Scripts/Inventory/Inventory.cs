using UnityEngine;

public class Inventory : MonoBehaviour {

    public GameObject inventoryPanel;
    public Transform slotParent;
    public GameObject slotPrefab;
    public int amountOfSlots = 15;
    public Slot[] slots;


    private void OnEnable()
    {
        EventManager.Instance.AddListener<OpenInventoryEvent>(OnOpenInventory);
        EventManager.Instance.AddListener<CloseInventoryEvent>(OnCloseInventory);
        EventManager.Instance.AddListener<PickUpItemEvent>(OnPickUpItem);
    }

    private void OnDisable()
    {
        EventManager.Instance.RemoveListener<OpenInventoryEvent>(OnOpenInventory);
        EventManager.Instance.RemoveListener<CloseInventoryEvent>(OnCloseInventory);
        EventManager.Instance.RemoveListener<PickUpItemEvent>(OnPickUpItem);
    }

    // Initialize
    void Start()
    {
        slots = new Slot[amountOfSlots];

        // Abort initializing when fields are not set to prevent errors
        if (slotParent == null || slotPrefab == null) return;

        for (int i = 0; i < amountOfSlots; i++)
        {
            slots[i] = Instantiate(slotPrefab, slotParent).GetComponent<Slot>();
            slots[i].Init();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (inventoryPanel)
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
        inventoryPanel.SetActive(true);
    }

    void OnCloseInventory(CloseInventoryEvent e)
    {
        inventoryPanel.SetActive(false);
    }

    void OnPickUpItem(PickUpItemEvent e)
    {
        AddItem(e.Item);
    }

    bool AddItem(Item item)
    {
        Slot slot = null;

        if (item.Stackable)
        {
            slot = FindSlotWithSameType(item);

            if (slot != null)
            {
                slot.Item.Amount += item.Amount;
                slot.AmountText.text = slot.Item.Amount.ToString();
            }
        }

        if (slot == null)
        {
            slot = FindFirstAvailableSlot();

            if (slot)
            {
                slot.Item = item;
                slot.AmountText.text = slot.Item.Amount.ToString();
            }
        }

        return slot != null;
    }

    Slot FindSlotWithSameType(Item item)
    {
        foreach (Slot slot in slots)
        {
            if (slot.Item != null)
            {
                if (slot.Item.Title.Equals(item.Title))
                {
                    return slot;
                }
            }
        }
        return null;
    }

    Slot FindFirstAvailableSlot()
    {
        foreach (Slot slot in slots)
        {
            if (slot.Item == null)
                return slot;
        }
        return null;
    }
}
