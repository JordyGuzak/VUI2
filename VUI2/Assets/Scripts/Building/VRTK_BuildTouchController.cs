using System.Collections;
using UnityEngine;
using VRTK;

[RequireComponent(typeof(VRTK_BuildController))]
public class VRTK_BuildTouchController : MonoBehaviour
{

    public VRTK_ControllerEvents events;
    public VRTK_BuildController buildController;

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

        buildController = GetComponentInParent<VRTK_BuildController>();
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
    {
    }

    protected virtual void DoShowMenu(float initialAngle, object sender = null)
    {
        DoChangeAngle(initialAngle); // Needed to register initial touch position before the touchpad axis actually changes
    }

    protected virtual void DoChangeAngle(float angle, object sender = null)
    {
        currentAngle = angle;
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
        InteractButton(CalculateAngle(e), ButtonEvent.hoverOn);
        //DoShowMenu(CalculateAngle(e));
    }

    protected virtual void DoTouchpadUntouched(object sender, ControllerInteractionEventArgs e)
    {
        touchpadTouched = false;
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
        float buttonAngle = 360f / 4; //Each button is an arc with this angle
        angle = VRTK_SharedMethods.Mod(angle, 360); //Offset the touch coordinate with our offset

        int buttonID = (int)VRTK_SharedMethods.Mod(((angle + (buttonAngle / 2f)) / buttonAngle), 4); //Convert angle into ButtonID (This is the magic)

        if (evt == ButtonEvent.hoverOn)
        {
            switch (buttonID)
            {
                case 0:
                    buildController.SelectNextBuildObject();
                    break;
                case 1:
                    buildController.RotatePreviewCounterClockWise();
                    break;
                case 2:
                    buildController.SelectPreviousBuildObject();
                    break;
                case 3:
                    buildController.RotatePreviewClockWise();
                    break;
                default:
                    break;
            }
        }
    }

    protected virtual float CalculateAngle(ControllerInteractionEventArgs e)
    {
        return 360 - e.touchpadAngle;
    }
}
