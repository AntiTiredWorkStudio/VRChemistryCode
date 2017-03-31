using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(UIButton))]
public class OnSelection : PointerEventHandle
{
    public List<EventDelegate> OnPointerPressed;
    public List<EventDelegate> OnPointerSelection;
    public List<EventDelegate> OnPointerDeSelection;
    public bool isSelection = false;
   protected UIButton buttonTarget;
    void Start()
    {
        buttonTarget = GetComponent<UIButton>();
    }

    public void FadeState(bool selectionState)
    {
        isSelection = selectionState;
        buttonTarget.SetState(UIButtonColor.State.Pressed, false);
        //Destroy(buttonTarget.gameObject);
        Debug.LogWarning("buttonTarget.SetState(UIButtonColor.State.Pressed, isSelection)");
      /*  if (isSelection)
        {
            buttonTarget.SetState(UIButtonColor.State.Pressed, false);
        }
        else
        {
            buttonTarget.SetState(UIButtonColor.State.Pressed, true);
        }*/
    }

    public override void Press(Transform handle)
    {
        base.Press(handle);
        foreach (EventDelegate ed in OnPointerPressed)
        {
            ed.target.SendMessage(ed.methodName, ed.parameters);
        }
        Debug.LogWarning("Press :"+isSelection);
        if (!isSelection)
        {
            isSelection = true;
            buttonTarget.SetState(UIButtonColor.State.Pressed, true);
            foreach (EventDelegate ed in OnPointerSelection)
            {
                ed.target.SendMessage(ed.methodName, ed.parameters);
            }
        }
        else
        {
            isSelection = false;
            buttonTarget.SetState(UIButtonColor.State.Normal, false);

            Debug.LogWarning("Is True?"+isSelection);
            foreach (EventDelegate ed in OnPointerDeSelection)
            {
                ed.target.SendMessage(ed.methodName, ed.parameters);
            }
        }
    }
}
