using UnityEngine;
using System.Collections;

public class PointerEventHandle : MonoBehaviour {
    public enum HandleState
    {
        DRAG,
        HOVER,
        OUT
    }

    public HandleState handleStatus = HandleState.OUT;
    bool activeResult = false;
    public void MainButtonResult(bool result)
    {
        if (activeResult != result)
        {
            if (result)
            {
                Active();
            }
            else
            {
                Unactive();
            }
            activeResult = result;
        }
    }
    public virtual void Active()
    {

    }
    public virtual void Unactive()
    {

    }
    public virtual void Press(Transform handle)
    {

    }

    public virtual void Enter(Transform handle)
    {

    }

    public virtual void Leave(Transform handle)
    {

    }


    public virtual void Up(Transform handle)
    {

    }

    public virtual void Out(Transform handle)
    {
        if(handleStatus != HandleState.OUT)
        {
            Leave(handle);
            handleStatus = HandleState.OUT;
        }
    }

    public virtual void Drag(Transform handle)
    {
        if (handleStatus != HandleState.DRAG)
        {
            Press(handle);
            handleStatus = HandleState.DRAG;
        }
    }

    public virtual void Hover(Transform handle)
    {
        if (handleStatus != HandleState.HOVER)
        {
            switch (handleStatus)
            {
                case HandleState.OUT:
                    Enter(handle);
                    break;
                case HandleState.DRAG:
                    Up(handle);
                    break;
                default:
                    break;
            }
            handleStatus = HandleState.HOVER;
        }
    }
}
