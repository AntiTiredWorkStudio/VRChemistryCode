using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 操作扳手的事件大全
/// </summary>
public enum PointerEvents
{
    CLICK = 0,
    UNCLICK = 1,
    TCLICK = 2,
    TUNCLICK = 3,
    TTOUCH = 4,
    TUNTOUCH = 5,
    TAXISMOVE = 6
}

/// <summary>
/// 操作扳手种类
/// </summary>
public enum PointerType
{
    LEFT = 0,
    RIGHT =1,
    BOTH = 3
}

public interface PointerEventsInterface
{
    void OnTouched(object sender, ControllerClickedEventArgs e);

    void OnUnTouched(object sender, ControllerClickedEventArgs e);

    void OnAxisMove(object sender, ControllerClickedEventArgs e);

    void OnTriggerClicked(object sender, ControllerClickedEventArgs e);

    void OnTriggerUnclicked(object sender, ControllerClickedEventArgs e);

     void OnTouchpadClicked(object sender, ControllerClickedEventArgs e);

     void OnTouchpadUnclicked(object sender, ControllerClickedEventArgs e);
}

[System.Serializable]
public class PointerEventSender
{
    public PointerEventsInterface pointerEventTarget;
    public PointerType pointerType;
    public PointerEvents eventType;

    public delegate void PointerMethordHandle(object sender, ControllerClickedEventArgs e);

    public PointerMethordHandle TargetFunction;

    public PointerEventSender(PointerEventsInterface pet,PointerEvents et,PointerType pt)
    {
        pointerEventTarget = pet;
        eventType = et;
        pointerType = pt;
        switch (et)
        {
            case PointerEvents.CLICK:// 扣动扳机
                TargetFunction = pointerEventTarget.OnTriggerClicked;
                return;
            case PointerEvents.TAXISMOVE://拇指移动
                TargetFunction = pointerEventTarget.OnAxisMove;
                return;
            case PointerEvents.TCLICK://触控板点击
                TargetFunction = pointerEventTarget.OnTouchpadClicked;
                return;
            case PointerEvents.TTOUCH://触控板触摸
                TargetFunction = pointerEventTarget.OnTouched;
                return;
            case PointerEvents.TUNCLICK://触控板抬起（点击结束）
                TargetFunction = pointerEventTarget.OnTouchpadUnclicked;
                return;
            case PointerEvents.TUNTOUCH://触控板停止触摸（结束触摸未点击）
                TargetFunction = pointerEventTarget.OnUnTouched;
                return;
            case PointerEvents.UNCLICK://抬起扳机
                TargetFunction = pointerEventTarget.OnTriggerUnclicked;
                return;
        }
    }
}


[RequireComponent(typeof(SteamVR_TrackedObject))]
[RequireComponent(typeof(SteamVR_ControllerEvents))]
public class PointerBase : MonoBehaviour , PointerEventsInterface
{
    
    //static Dictionary<PointerType, PointerBase> PointerHash = null;
   /* static void AddPointerBase(PointerType pt , PointerBase pb)
    {
        if(TotalEventHash!= null && !TotalEventHash.ContainsKey(pt))
        {
            TotalEventHash.Add(pt, new Dictionary<PointerEvents, List<PointerEventSender>>());
        }
        if(PointerHash == null)
        {
            PointerHash = new Dictionary<PointerType, PointerBase>();
        }
        PointerHash.Add(pt, pb);
    }*/
   // public static PointerBase Instance(PointerType pt)
   // {
    /*   if (!PointerHash.ContainsKey(pt)) {
            PointerBase[] _bases = GameObject.FindObjectsOfType<PointerBase>();
            foreach (PointerBase pb in _bases)
            {
                if(pb.PointerID == pt)
                {
                    return pb;
                }
            }
        }*/

    //    return PointerHash[pt];
  //  }
    /// <summary>
    /// 该方法应在初始化(Start)前后调用
    /// </summary>
    /// <param name="pt"></param>
    public static void AddEventSender(PointerType pointerType,PointerEventsInterface pointerEventTarget,params PointerEvents[] eventTypes)
    {
        if (PointerEventSenders == null)
        {
            PointerEventSenders = new List<PointerEventSender>();
        }
        foreach (PointerEvents pe in eventTypes)
        {
            if (pointerType == PointerType.BOTH)
            {
                PointerEventSenders.Add(new PointerEventSender(pointerEventTarget, pe, PointerType.LEFT));
                PointerEventSenders.Add(new PointerEventSender(pointerEventTarget, pe, PointerType.RIGHT));
            }
            else
            {
                PointerEventSenders.Add(new PointerEventSender(pointerEventTarget, pe, pointerType));
            }
        }
    }

    public static Dictionary<PointerType, Dictionary<PointerEvents, List<PointerEventSender>>> TotalEventHash = null;

    public static List<PointerEventSender> PointerEventSenders;
    //Dictionary<PointerEvents, List<PointerEventSender>> pointereventsenderhash = null;



    /// <summary>
    /// 初始化的时间一般为调用事件时
    /// </summary>
    public static Dictionary<PointerType, Dictionary<PointerEvents, List<PointerEventSender>>> PointerEventSenderHash
    {
        get
        {
            if (TotalEventHash == null)
            {
                TotalEventHash = new Dictionary<PointerType, Dictionary<PointerEvents, List<PointerEventSender>>>();
                for (int t = 0; t < 2; t++)
                {
                    for (int i = 0; i < 7; i++)
                    {
                        if (!TotalEventHash.ContainsKey((PointerType)i))
                        {
                            TotalEventHash.Add(
                                (PointerType)i
                                ,new Dictionary<PointerEvents, List<PointerEventSender>>()
                                );
                        }
                        TotalEventHash[(PointerType)t].Add(
                            (PointerEvents)i,
                        GetHandleOfEventTypeFromList((PointerEvents)i,(PointerType)t));
                    }
                }
            }
            return TotalEventHash;
        }
    }

    public PointerType PointerID;

    /// <summary>
    /// 调用所有注册方法
    /// </summary>
    /// <param name="pe"></param>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public static void CallEventHandles(PointerEvents pe,PointerType pt, object sender, ControllerClickedEventArgs e)
    {
      //  return;
        foreach(PointerEventSender pes in
        GetHandleOfEventTypeFromHash(pe,pt))
        {
            pes.TargetFunction(sender, e);
        }
    }

    /// <summary>
    /// 通过哈希表获取方法
    /// </summary>
    /// <param name="pe"></param>
    /// <returns></returns>
    private static List<PointerEventSender> GetHandleOfEventTypeFromHash(PointerEvents pe, PointerType pt)
    {
        if (PointerEventSenderHash[pt].ContainsKey(pe))
        {
            return PointerEventSenderHash[pt][pe];
        }
        else
        {
            return null;
        }
    }
    /// <summary>
    /// 不通过哈希表取,哈希表还未完成遍历初始化时，使用此方法，目前仅用于初始化。
    /// </summary>
    /// <param name="pe"></param>
    /// <returns></returns>
    private static List<PointerEventSender> GetHandleOfEventTypeFromList(PointerEvents pe,PointerType pt)
    {
        List<PointerEventSender> list = new List<PointerEventSender>();
        foreach (PointerEventSender p in PointerEventSenders)
        {
            if(pt== p.pointerType && pe == p.eventType)
            {
                list.Add(p);
            }
        }
        return list;
    }

    SteamVR_TrackedObject SVTO = null;
    SteamVR_ControllerEvents SVRC = null;
    EventDelegate ed;
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

    

    /// <summary>
    /// 提供事件源，传给适配器和StramVR事件系统进行连接
    /// </summary>
    public Dictionary<PointerEvents, ControllerClickedEventHandler> EventsHandles
    {
        get
        {
            Dictionary<PointerEvents, ControllerClickedEventHandler> eventHandle = new Dictionary<PointerEvents, ControllerClickedEventHandler>();
            eventHandle.Add(PointerEvents.CLICK, new ControllerClickedEventHandler( OnTriggerClicked));
            eventHandle.Add(PointerEvents.UNCLICK, new ControllerClickedEventHandler(OnTriggerUnclicked));
            eventHandle.Add(PointerEvents.TCLICK, new ControllerClickedEventHandler(OnTouchpadClicked));
            eventHandle.Add(PointerEvents.TUNCLICK, new ControllerClickedEventHandler(OnTouchpadUnclicked));
            eventHandle.Add(PointerEvents.TTOUCH, new ControllerClickedEventHandler(OnTouched));
            eventHandle.Add(PointerEvents.TUNTOUCH, new ControllerClickedEventHandler(OnUnTouched));
            eventHandle.Add(PointerEvents.TAXISMOVE, new ControllerClickedEventHandler(OnAxisMove));
            return eventHandle;
        }
    }

    public virtual void OnTouched(object sender, ControllerClickedEventArgs e)
    {
        CallEventHandles(PointerEvents.TTOUCH,PointerID, sender,e);
    //    Debug.LogWarning("OnTouched");
    }

    public virtual void OnUnTouched(object sender, ControllerClickedEventArgs e)
    {
         CallEventHandles(PointerEvents.TUNTOUCH, PointerID, sender, e);
   //     Debug.LogWarning("OnUnTouched");
    }

    public virtual void OnAxisMove(object sender, ControllerClickedEventArgs e)
    {
        CallEventHandles(PointerEvents.TAXISMOVE, PointerID, sender, e);
     //   Debug.LogWarning("OnTouchAxisMove");
    }

    public virtual void OnTriggerClicked(object sender, ControllerClickedEventArgs e)
    {
        CallEventHandles(PointerEvents.CLICK, PointerID, sender, e);
    //    Debug.LogWarning("OnTriggerClicked");
    }
    public virtual void OnTriggerUnclicked(object sender, ControllerClickedEventArgs e)
    {
         CallEventHandles(PointerEvents.UNCLICK, PointerID, sender, e);
   //     Debug.LogWarning("OnTriggerUnclicked");
    }

    public virtual void OnTouchpadClicked(object sender, ControllerClickedEventArgs e)
    {
          CallEventHandles(PointerEvents.TCLICK, PointerID, sender, e);
    //    Debug.LogWarning("OnTouchpadClicked");
    }

    public virtual void OnTouchpadUnclicked(object sender, ControllerClickedEventArgs e)
    {
          CallEventHandles(PointerEvents.TUNCLICK, PointerID, sender, e);
    //    Debug.LogWarning("OnTouchpadUnclicked");
    }


}
