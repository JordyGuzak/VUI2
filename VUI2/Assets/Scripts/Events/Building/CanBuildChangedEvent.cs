using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanBuildChangedEvent : GameEvent {

    public object sender;
    public bool CanBuild { get; set; }

    public CanBuildChangedEvent(object sender, bool canBuild)
    {
        this.sender = sender;
        this.CanBuild = canBuild;
    }
}
