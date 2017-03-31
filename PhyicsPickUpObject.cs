using UnityEngine;
using System.Collections;
[RequireComponent(typeof(Rigidbody))]
public class PhyicsPickUpObject : DragCommand {
    public override void Start()
    {
        base.Start();
        physRigid = GetComponent<Rigidbody>();
    }
    public Rigidbody physRigid;

    public override void OnDragPrepared()
    {
        base.OnDragPrepared();
    }

    

    public override void OnDragStart()
    {
        base.OnDragStart();
        if (physRigid != null)
        {
            physRigid.velocity = Vector3.zero;
            physRigid.freezeRotation = true;
        }
        DragStartAction();
    }
    public override void OnDragEnd()
    {
        base.OnDragEnd();
        if (physRigid != null)
        {
            physRigid.velocity = DeltaVec * 3.0f;//Vector3.zero;
            physRigid.freezeRotation = true;
        }
    }


    public virtual void DragStartAction()
    {

    }
}
