using System.Collections;
using UnityEngine;
using VRTK;

public class VRTK_BuildTouchController : MonoBehaviour {

    public VRTK_ControllerEvents events;

    //public List<RadialMenuButton> buttons;

    protected float currentAngle; //Keep track of angle for when we click
    protected bool touchpadTouched;

    public enum ButtonEvent
    {
        hoverOn,
        hoverOff,
        click,
        unclick
    }

    protected virtual void Awake()
    {
        Initialize();
    }

    protected virtual void Initialize()
    {
        if (events == null)
        {
            events = GetComponentInParent<VRTK_ControllerEvents>();
        }
    }

    protected virtual void OnEnable()
    {
        if (events == null)
        {
            VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_NOT_INJECTED, "RadialMenuController", "VRTK_ControllerEvents", "events", "the parent"));
            return;
        }
        else
        {
            events.TouchpadPressed += new ControllerInteractionEventHandler(DoTouchpadClicked);
            events.TouchpadReleased += new ControllerInteractionEventHandler(DoTouchpadUnclicked);
            events.TouchpadTouchStart += new ControllerInteractionEventHandler(DoTouchpadTouched);
            events.TouchpadTouchEnd += new ControllerInteractionEventHandler(DoTouchpadUntouched);
            events.TouchpadAxisChanged += new ControllerInteractionEventHandler(DoTouchpadAxisChanged);
        }
    }

    protected virtual void OnDisable()
    {
        events.TouchpadPressed -= new ControllerInteractionEventHandler(DoTouchpadClicked);
        events.TouchpadReleased -= new ControllerInteractionEventHandler(DoTouchpadUnclicked);
        events.TouchpadTouchStart -= new ControllerInteractionEventHandler(DoTouchpadTouched);
        events.TouchpadTouchEnd -= new ControllerInteractionEventHandler(DoTouchpadUntouched);
        events.TouchpadAxisChanged -= new ControllerInteractionEventHandler(DoTouchpadAxisChanged);
    }

    protected virtual void DoClickButton(object sender = null) // The optional argument reduces the need for middleman functions in subclasses whose events likely pass object sender
    {
        //menu.ClickButton(currentAngle);
    }

    protected virtual void DoUnClickButton(object sender = null)
    {;
    }

    protected virtual void DoShowMenu(float initialAngle, object sender = null)
    {
        DoChangeAngle(initialAngle); // Needed to register initial touch position before the touchpad axis actually changes
    }

    protected virtual void DoHideMenu(bool force, object sender = null)
    {
//menu.StopTouching();
    // menu.HideMenu(force);
    }

    protected virtual void DoChangeAngle(float angle, object sender = null)
    {
        currentAngle = angle;

     //   menu.HoverButton(currentAngle);
    }

    protected virtual void AttemptHapticPulse(float strength)
    {
        if (events)
        {
            VRTK_ControllerHaptics.TriggerHapticPulse(VRTK_ControllerReference.GetControllerReference(events.gameObject), strength);
        }
    }

    protected virtual void DoTouchpadClicked(object sender, ControllerInteractionEventArgs e)
    {
        DoClickButton();
    }

    protected virtual void DoTouchpadUnclicked(object sender, ControllerInteractionEventArgs e)
    {
        DoUnClickButton();
    }

    protected virtual void DoTouchpadTouched(object sender, ControllerInteractionEventArgs e)
    {
        touchpadTouched = true;
        DoShowMenu(CalculateAngle(e));
    }

    protected virtual void DoTouchpadUntouched(object sender, ControllerInteractionEventArgs e)
    {
        touchpadTouched = false;
        DoHideMenu(false);
    }

    //Touchpad finger moved position
    protected virtual void DoTouchpadAxisChanged(object sender, ControllerInteractionEventArgs e)
    {
        if (touchpadTouched)
        {
            DoChangeAngle(CalculateAngle(e));
        }
    }

    //Turns and Angle and Event type into a button action
    protected virtual void InteractButton(float angle, ButtonEvent evt) //Can't pass ExecuteEvents as parameter? Unity gives error
    {
        //Get button ID from angle
      //  float buttonAngle = 360f / buttons.Count; //Each button is an arc with this angle
      //  angle = VRTK_SharedMethods.Mod((angle + -offsetRotation), 360); //Offset the touch coordinate with our offset

      //  int buttonID = (int)VRTK_SharedMethods.Mod(((angle + (buttonAngle / 2f)) / buttonAngle), buttons.Count); //Convert angle into ButtonID (This is the magic)
    }

    protected virtual float CalculateAngle(ControllerInteractionEventArgs e)
    {
        return 360 - e.touchpadAngle;
    }
}
