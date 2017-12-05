using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapZoneExitEvent : GameEvent {

    public object sender;
    public GameObject ObjectInZone { get; set; }

    public SnapZoneExitEvent(object sender, GameObject obj)
    {
        this.sender = sender;
        this.ObjectInZone = obj;
    }
}
