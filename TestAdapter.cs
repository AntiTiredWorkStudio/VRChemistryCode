using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestAdapter : PointerAdaper {
    public override void InitAdapter(Dictionary<PointerEvents, ControllerClickedEventHandler> pHandler)
    {
        base.InitAdapter(pHandler);
        
    }

    public uint index = 0;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            ControllerClickedEventArgs e = new ControllerClickedEventArgs();
            e.controllerIndex = index;
            GetPointerHandles(PointerEvents.CLICK)(this,e);
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            ControllerClickedEventArgs e = new ControllerClickedEventArgs();
            e.controllerIndex = index;
            GetPointerHandles(PointerEvents.UNCLICK)(this, e);
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            ControllerClickedEventArgs e = new ControllerClickedEventArgs();
            e.controllerIndex = index;
            GetPointerHandles(PointerEvents.TCLICK)(this, e);
        }

        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            ControllerClickedEventArgs e = new ControllerClickedEventArgs();
            e.controllerIndex = index;
            GetPointerHandles(PointerEvents.TUNCLICK)(this, e);
        }
    }
}
