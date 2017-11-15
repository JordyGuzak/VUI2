using System.Collections.Generic;
using UnityEngine;

public class InputManager {

    static InputManager instance;
    public static InputManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new InputManager();
                instance.Init();
            }
            return instance;
        }
    }

    public Dictionary<Actions, ActionKeys> actions = new Dictionary<Actions, ActionKeys>();

    public void Init()
    {
        actions.Clear();
        actions.Add(Actions.FORWARD, new ActionKeys(KeyCode.W, KeyCode.UpArrow));
        actions.Add(Actions.BACK, new ActionKeys(KeyCode.S, KeyCode.DownArrow));
        actions.Add(Actions.LEFT, new ActionKeys(KeyCode.A, KeyCode.LeftArrow));
        actions.Add(Actions.RIGHT, new ActionKeys(KeyCode.D, KeyCode.RightArrow));

        actions.Add(Actions.JUMP, new ActionKeys(KeyCode.Space, KeyCode.None));
        actions.Add(Actions.SPRINT, new ActionKeys(KeyCode.LeftShift, KeyCode.RightShift));
        actions.Add(Actions.INTERACT, new ActionKeys(KeyCode.E, KeyCode.None));
    }

    public static bool GetKey(Actions action)
    {
        ActionKeys ak;
        if (Instance.actions.TryGetValue(action, out ak))
        {
            if (Input.GetKey(ak.PrimaryKeyCode) || Input.GetKey(ak.AltKeyCode))
            {
                return true;
            }
        }
        return false;
    }

    public static bool GetKeyDown(Actions action)
    {
        ActionKeys ak;
        if (Instance.actions.TryGetValue(action, out ak))
        {
            if (Input.GetKeyDown(ak.PrimaryKeyCode) || Input.GetKeyDown(ak.AltKeyCode))
            {
                return true;
            }
        }
        return false;
    }

    public static bool GetKeyUp(Actions action)
    {
        ActionKeys ak;
        if (Instance.actions.TryGetValue(action, out ak))
        {
            if (Input.GetKeyUp(ak.PrimaryKeyCode) || Input.GetKeyUp(ak.AltKeyCode))
            {
                return true;
            }
        }
        return false;
    }

    public void ChangePrimaryKeyCode(Actions action, KeyCode newKeyCode)
    {
        ActionKeys ak;
        if (actions.TryGetValue(action, out ak))
        {
            ak.PrimaryKeyCode = newKeyCode;
        }
    }

    public void ChangeAltKeyCode(Actions action, KeyCode newKeyCode)
    {
        ActionKeys ak;
        if (actions.TryGetValue(action, out ak))
        {
            ak.AltKeyCode = newKeyCode;
        }
    }
}

public enum Actions
{
    FORWARD,
    BACK,
    LEFT,
    RIGHT,
    JUMP,
    SPRINT,
    INTERACT
}
