using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[RequireComponent(typeof(SteamVR_TrackedObject))]
[RequireComponent(typeof(SteamVR_ControllerEvents))]
public class SteamVRTrackerAdaper : PointerAdaper
{

    SteamVR_TrackedObject SVTO = null;
    SteamVR_ControllerEvents SVRC = null;

    public SteamVR_TrackedObject TrackedObject
    {
        get
        {
            if (SVTO == null)
            {
                SVTO = GetComponent<SteamVR_TrackedObject>();
            }
            return SVTO;
        }
    }

    public SteamVR_ControllerEvents ControllerEvents
    {
        get
        {
            if (SVRC == null)
            {
                SVRC = GetComponent<SteamVR_ControllerEvents>();
            }
            return SVRC;
        }
    }

    public override void InitAdapter(Dictionary<PointerEvents, ControllerClickedEventHandler> pHandler)
    {
        base.InitAdapter(pHandler);
        ControllerEvents.TriggerClicked += new ControllerClickedEventHandler(GetPointerHandles(PointerEvents.CLICK));
        ControllerEvents.TriggerUnclicked += new ControllerClickedEventHandler(GetPointerHandles(PointerEvents.UNCLICK));
        ControllerEvents.TouchpadClicked += new ControllerClickedEventHandler(GetPointerHandles(PointerEvents.TCLICK));
        ControllerEvents.TouchpadUnclicked += new ControllerClickedEventHandler(GetPointerHandles(PointerEvents.TUNCLICK));
        ControllerEvents.TouchpadTouched += new ControllerClickedEventHandler(GetPointerHandles(PointerEvents.TTOUCH));
        ControllerEvents.TouchpadUntouched += new ControllerClickedEventHandler(GetPointerHandles(PointerEvents.TUNTOUCH));
        ControllerEvents.TouchpadAxisChanged += new ControllerClickedEventHandler(GetPointerHandles(PointerEvents.TAXISMOVE));
        //        GetPointerHandles()
    }
}
