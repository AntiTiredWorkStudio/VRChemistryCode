using UnityEngine;
using System.Collections;

public class MovementPointer : Pointer {
    public Transform ControllerRoot;
    public Transform MoveRegion;
    bool MoveAction = false;
    public override void Start()
    {
        base.Start();
        MoveRegion.gameObject.SetActive(false);
    }

    public override void OnTouchpadClicked(object sender, ControllerClickedEventArgs e)
    {
        MoveAction = true;
        base.OnTouchpadClicked(sender, e);
    }

    public override void OnTouchpadUnclicked(object sender, ControllerClickedEventArgs e)
    {
        MoveAction = false;
        base.OnTriggerUnclicked(sender, e);
    }

    protected override void Update()
    {
        base.Update();
       
        if (MoveRegion.gameObject.activeSelf)
        {
            base.OnHitOthers(hitInfo, MoveAction);
        }
        if (MoveAction)
        {
            if ((hitInfo.collider != null) && hitInfo.collider.gameObject.layer != LayerMask.NameToLayer("Land"))
            {
            //    Debug.LogWarning("Move Layer is not Land");
                return;
            }
            if (!MoveRegion.gameObject.activeSelf)
            {
                MoveRegion.gameObject.SetActive(true);

            }
            MoveRegion.transform.position = hitInfo.point;
        }
        else
        {
            if (MoveRegion.gameObject.activeSelf)
            {
                ControllerRoot.transform.position = MoveRegion.transform.position;
                MoveRegion.gameObject.SetActive(false);
            }

        }
    }
}
