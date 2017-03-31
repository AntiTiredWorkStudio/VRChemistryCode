using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class DragCommand : PointerEventHandle {
    /// <summary>
    /// 创建拖拽拼盘
    /// </summary>
    /// <param name="DragPannelPos"></param>
    /// <param name=""></param>
    /// <returns></returns>
    public static DragCommand CreateDragPannel(Vector3 DragPannelPos,Transform target)
    {
        GameObject spoint = 
        Instantiate(Resources.Load("Prefabs/SplashPoint")) as GameObject;
        spoint.transform.position = DragPannelPos;
        spoint.transform.localScale = Vector3.one * 0.009f;
        DragCommand dcommand = spoint.GetComponent<DragCommand>();
       // dcommand.FollowedObjectTransform = target;
        //dcommand.InitDragRoot(target);
        return dcommand;
    }
    
    public delegate void OnEventHappen();

    public List<OnEventHappen> PrepareDrag = new List<OnEventHappen>();

    public List<OnEventHappen> StartDrag = new List<OnEventHappen>();

    public List<OnEventHappen> FinishedDrag = new List<OnEventHappen>();

    public List<OnEventHappen> IdleUpdate = new List<OnEventHappen>();


    public Transform FollowedObjectTransform;



    //Vector3 FollowedOffset;
    public virtual void Start()
    {
        if (FollowedObjectTransform == null)
        {
            FollowedObjectTransform = transform;
        }
        InitDragRoot(FollowedObjectTransform);
        lastVec = transform.position;//设定最后移动位置为当前位置
      //  StartDeltaPosProgress();
    }

    /// <summary>
    /// 位置变化量
    /// </summary>
    public Vector3 deltaResult;
    /// <summary>
    /// 可拖拽物体位置的变化量
    /// </summary>
    public Vector3 DeltaVec
    {
        get
        {
            return deltaResult;
        }
    }

    public Vector3 lastVec;
    Timer t = null;

    void StartDeltaPosProgress()
    {
        Debug.LogWarning("StartDeltaPosProgress");
        t =  Timer.addTimer(gameObject, 0.2f, null,0,OnDeltaPosTracked);
        t.startTimer();
    }
    void EndDeltaPosProgress()
    {
        t.endTimer();
    }

    void OnDeltaPosTracked(int times)
    {
        deltaResult = transform.position - lastVec;
        lastVec = transform.position;
    }


    void CallDelegateMethordList(List<OnEventHappen> mlist)
    {
        if(mlist!= null)
        {
            foreach (OnEventHappen oeh in mlist)
            {
                //Debug.LogWarning("Call Methord");
                oeh();
            }
        }
    }

    bool OnDrag = false;
    //Vector3 offset;
    Transform targetHandle = null;

    Transform handleRoot = null;


    Transform dragObjectRoot = null;


    void InitHandleRoot(Transform handle)
    {
       if (handleRoot != null)
        {
            Destroy(handleRoot.gameObject);
            handleRoot = null;
        }
      // if(transform.FindChild("HandleRoot"))
        handleRoot = new GameObject("HandleRoot").transform;
        handleRoot.parent = handle;
        handleRoot.transform.position = transform.position;
        handleRoot.transform.rotation = transform.rotation;
    }

    public void InitDragPannelPositionAndRoot(Vector3 targetPos)
    {
        //transform.position = Vector3.Lerp(transform.position, targetPos,Time.deltaTime*2.0f);
        transform.position = targetPos;
        dragObjectRoot.transform.position = FollowedObjectTransform.position;
        /*   if (FollowedObjectTransform != null)
           {
               InitDragRoot(FollowedObjectTransform);
           }*/
    }

    void InitDragRoot(Transform dragPivot)
    {
        if (dragObjectRoot != null)
        {
            Destroy(dragObjectRoot.gameObject);
            dragObjectRoot = null;
        }
        if (transform.FindChild("DragRoot"))
        {
            dragObjectRoot = transform.FindChild("DragRoot").transform;
        }
        else
        {
            dragObjectRoot = new GameObject("DragRoot").transform;
            dragObjectRoot.parent = transform;
            dragObjectRoot.transform.position = dragPivot.position;
            dragObjectRoot.transform.rotation = dragPivot.rotation;
        }
    }


    public override void Drag(Transform handle)
    {
        base.Drag(handle);
        if (!OnDrag)
        {
            if (Vector3.Distance(handle.position, transform.position) < MinTriggerdDistance)
            {
                OnDrag = true;
                targetHandle = handle;
                InitHandleRoot(targetHandle);
               // offset = handle.position - transform.position;
                targetHandle.GetComponent<Pointer>().UseRaycaster = false;
                CallDelegateMethordList(StartDrag);
                OnDragStart();
            }
        }
    }

    public virtual void OnDragPrepared()
    {

    }
    public virtual void OnDragStart()
    {
        if (t == null)
        {
            StartDeltaPosProgress();//Condition01
        }
    }
    public virtual void OnDragEnd()
    {
        if (t != null)
        {
            EndDeltaPosProgress();//Condition02
        }
    }

    public float MinTriggerdDistance = 0.006f;
    public  override  void Hover(Transform handle)
    {
        base.Hover(handle);
        if (Vector3.Distance(handle.position, transform.position) < MinTriggerdDistance)
        {
            CallDelegateMethordList(PrepareDrag);
            OnDragPrepared();
        }
    }

    public override void Out(Transform handle)
    {
        base.Out(handle);
        CallDelegateMethordList(FinishedDrag);
        OnDragEnd();
    }

    protected virtual void DragFollow()
    {
        if (OnDrag)
        {
            if (targetHandle != null)
            {
                transform.position = handleRoot.position;
                transform.rotation = handleRoot.rotation;
                if (!targetHandle.GetComponent<Pointer>().isActive)
                {
                    OnDrag = false;
                    targetHandle.GetComponent<Pointer>().UseRaycaster = true;
                    CallDelegateMethordList(FinishedDrag);
                    OnDragEnd();
                }
            }
        }
    }
    protected virtual void Follow()
    {
        if (FollowedObjectTransform != null)
        {
            FollowedObjectTransform.position = dragObjectRoot.position;
            FollowedObjectTransform.rotation = dragObjectRoot.rotation;
        }
    }
    protected virtual void Update()
    {
        DragFollow();
        Follow();
        if (!OnDrag)
        {
            CallDelegateMethordList(IdleUpdate);
        }

    }

}
