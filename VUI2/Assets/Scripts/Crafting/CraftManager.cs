using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftManager : Singleton<CraftManager> {

    protected CraftManager() { } // guarantee this will be always a singleton only - can't use the constructor!

    public ViveController controllerRight;
    public ViveController controllerLeft;

    public GameObject hammerPrefab;

    private void Update()
    {
        if (controllerRight.objectInHand && controllerLeft.objectInHand)
        {
            Craftable craftableA = controllerRight.objectInHand.GetComponent<Craftable>();

            if (craftableA)
            {
                if (!craftableA.collidingObject) return;

                Craftable craftableB = craftableA.collidingObject.GetComponent<Craftable>();

                if (craftableB)
                {
                    GameObject gobj = Craft(craftableA.itemData.GetItem(), craftableB.itemData.GetItem());

                    if (gobj)
                    {
                        Destroy(craftableA.gameObject);
                        Destroy(craftableB.gameObject);
                        Instantiate(hammerPrefab, controllerRight.transform.position, Quaternion.identity);
                    }
                }
            }
        }
    }

    public GameObject Craft(Item itemA, Item itemB)
    {
        if (itemA.Title.Equals("Wood") && itemB.Title.Equals("Stone"))
        {
            if (hammerPrefab) return hammerPrefab;
        }
        return null;
    }




}
