using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapZoneEnterEvent : GameEvent {

    public object sender;
    public GameObject ObjectInZone { get; set; }

	public SnapZoneEnterEvent(object sender, GameObject obj)
    {
        this.sender = sender; 
        this.ObjectInZone = obj;
    }
}
