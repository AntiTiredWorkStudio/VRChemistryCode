using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[RequireComponent(typeof(UIButton))]
public class OnPointer : PointerEventHandle
{
    public List<EventDelegate> OnPointerPressed;
    UIButton buttonTarget;
    public bool FarviewFocus = false; 
    void Start()
    {
        buttonTarget = GetComponent<UIButton>();
    }

    public override void Press(Transform handle)
    {
        base.Press(handle);
        buttonTarget.SetState(UIButtonColor.State.Pressed, false);
        if (FarviewFocus)
        {
            FarView.farView.LoadFocusObject(transform);
        }
        if (OnPointerPressed != null)
        {
            foreach (EventDelegate ed in OnPointerPressed)
            {
                ed.target.SendMessage(ed.methodName, ed.parameters);
            }
        }
    }

    public override void Enter(Transform handle)
    {
        base.Enter(handle);
        buttonTarget.SetState(UIButtonColor.State.Hover, false);
    }
    public override void Up(Transform handle)
    {
        base.Up(handle);
        buttonTarget.SetState(UIButtonColor.State.Hover, false);
    }
    public override void Leave(Transform handle)
    {
        base.Leave(handle);
        buttonTarget.SetState(UIButtonColor.State.Normal, false);
    }
}
