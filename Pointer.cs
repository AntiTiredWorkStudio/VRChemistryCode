using UnityEngine;
using System.Collections;
[RequireComponent(typeof(SteamVR_TrackedObject))]
[RequireComponent(typeof(SteamVR_ControllerEvents))]
public class Pointer : PointerBase
{
  
    // Use this for initialization
    public virtual void Start () {
        //ControllerEvents.TouchpadClicked
      //  ControllerEvents.TriggerClicked += new ControllerClickedEventHandler(EnablePointerBeam);
      //  ControllerEvents.TriggerUnclicked += new ControllerClickedEventHandler(DisablePointerBeam);
        // ControllerEvents.AliasPointerOn += new ControllerClickedEventHandler(EnablePointerBeam);
        // ControllerEvents.AliasPointerOff += new ControllerClickedEventHandler(DisablePointerBeam);

        if (Laser != null)
        {
            GameObject laser = Instantiate(Laser) as GameObject;
            laser.name = "Laser_"+name;
            laserLine = laser.GetComponent<LineRenderer>();
            //laserLine.enabled = false;
        }
        else
        {
            throw new System.Exception("没有激光样式");
        }

    }

    public uint controllerIndex = 0;
    /// <summary>
    /// 控制器点下
    /// </summary>
    public bool isActive = false;

    public override void OnTriggerClicked(object sender, ControllerClickedEventArgs e)
    {
        if (!isActive)
        {
            controllerIndex = e.controllerIndex;
            isActive = true;
        }
        base.OnTriggerClicked(sender, e);
    }

    public override void OnTriggerUnclicked(object sender, ControllerClickedEventArgs e)
    {
        if (isActive)
        {
            controllerIndex = e.controllerIndex;
            isActive = false;
        }
        base.OnTriggerUnclicked(sender, e);
    }
    
    Ray ray;
    protected RaycastHit hitInfo;
    LineRenderer laserLine;
    public GameObject Laser;

    public PointerEventHandle LastOnPointerHandle = null;
    public bool UseRaycaster = true;
    void SetLaserLineColor(Color col)
    {
        laserLine.material.SetColor("_Color", col);
        laserLine.material.SetColor("_EmissionColor", col);
    }
    public virtual void OnHitOthers(RaycastHit hitInfo,bool triggers)
    {

    }
    public void TogglePointer(bool result)
    {
        if (UseRaycaster)
        {
            laserLine.enabled = TrackedObject.isValid;
            ray = new Ray(transform.position, transform.forward * 2000.0f);
        //      Debug.DrawRay(transform.position, transform.forward * 2000.0f);
            laserLine.SetPosition(0, transform.position);

            if (Physics.Raycast(ray, out hitInfo))//, LayerMask.NameToLayer("Land")))
            {
             /*   if (hitInfo.collider.gameObject.layer != 
                {
                    return;
                }*/
                // hitInformation = hitInfo;
                laserLine.SetPosition(1, hitInfo.point);

                if (hitInfo.collider.GetComponent<PointerEventHandle>())
                {
                    hitInfo.collider.GetComponent<PointerEventHandle>().MainButtonResult(result);
                    if (LastOnPointerHandle == null || !LastOnPointerHandle.Equals(hitInfo.collider.GetComponent<PointerEventHandle>()))
                    {
                        if (LastOnPointerHandle != null)
                        {
                            LastOnPointerHandle.Out(transform);
                        }
                        LastOnPointerHandle = hitInfo.collider.GetComponent<PointerEventHandle>();
                        if (result)
                        {
                            SetLaserLineColor(new Color(0.9f, 0.1f, 0.9f));
                            LastOnPointerHandle.Drag(transform);
                        }
                        else
                        {
                            SetLaserLineColor(Color.green);
                            LastOnPointerHandle.Hover(transform);
                        }
                    }
                    else
                    {
                        if (result)
                        {
                            SetLaserLineColor(new Color(0.9f, 0.1f, 0.9f));
                            LastOnPointerHandle.Drag(transform);
                        }
                        else
                        {
                            SetLaserLineColor(Color.green);
                            LastOnPointerHandle.Hover(transform);
                        }
                    }
                }
                else
                {
                    SetLaserLineColor(Color.yellow);
                    OnHitOthers(hitInfo, result);
                }
            }
            else
            {
                laserLine.SetPosition(1, transform.forward * 2000.0f);

                SetLaserLineColor(Color.red);

                if (LastOnPointerHandle != null)
                {
                    LastOnPointerHandle.Out(transform);
                    LastOnPointerHandle = null;
                }

            }
        }
        else
        {
            laserLine.enabled = false;
        }
    }

    protected virtual void Update ()
    {
        TogglePointer(isActive);
    }
}
