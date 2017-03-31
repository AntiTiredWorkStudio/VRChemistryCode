using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//[RequireComponent(typeof(PointerBase))]
public class PointerAdaper : MonoBehaviour {
    protected PointerBase pointerBase = null;
    public PointerBase PB
    {
        get
        {
            if(pointerBase == null)
            {
                pointerBase = GetComponent<PointerBase>();
            }
            return pointerBase;
        }
    }
    private Dictionary<PointerEvents, ControllerClickedEventHandler> PointerHandles = null;
    protected Dictionary<PointerEvents, ControllerClickedEventHandler> PointerHandlesHash
    {
        get {
            if (PointerHandles == null)
            {
                PointerHandles = PB.EventsHandles;
            }
            return PointerHandles;
        }
    }
    protected ControllerClickedEventHandler GetPointerHandles(PointerEvents pe)
    {
        if (PointerHandlesHash.ContainsKey(pe))
        {
            return PointerHandlesHash[pe];
        }
        return null;
    }
    void Awake()
    {
        InitAdapter(PointerHandlesHash);
    }

    public virtual void InitAdapter(Dictionary<PointerEvents, ControllerClickedEventHandler> pHandler)
    {

    }
}
