using UnityEngine;

public class ActionKeys {

    private KeyCode primary;
    private KeyCode alt;


    public KeyCode PrimaryKeyCode
    {
        get { return primary; }
        set { primary = value; }
    }

    public KeyCode AltKeyCode
    {
        get { return alt; }
        set { alt = value; }
    }
    
    public ActionKeys(KeyCode primary, KeyCode alt)
    {
        this.primary = primary;
        this.alt = alt;
    }
}
