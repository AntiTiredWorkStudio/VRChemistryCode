using UnityEngine;
using System.Collections;

public class BillBoardMove : MonoBehaviour {
    public enum BillAxis
    {
        forward,back,
        left,right,
        top,bottom
    }
    public BillAxis billAxis;
     Vector3 axis
     {
         get
         {
             switch (billAxis)
             {
                 case BillAxis.forward:
                     return transform.forward;
                 case BillAxis.back:
                     return -transform.forward;
                 case BillAxis.top:
                     return transform.up;
                 case BillAxis.bottom:
                     return -transform.up;
                 case BillAxis.left:
                     return -transform.right;
                 case BillAxis.right:
                     return transform.right;
                 default:
                     return transform.forward;
             }
         }
     }
    public bool lockX;
    public bool lockY;
    public bool lockZ;
    protected Vector3 InitalAxis;
    Camera targetCam;
    void Start()
    {
        InitalAxis = axis;
        if (Camera.main == null)
        {
            targetCam = GameObject.FindObjectOfType<Camera>();
        }
        else
        {
            targetCam = Camera.main;
        }
    }
	void Update () {
        
        if (Camera.main.transform)
        {
            Vector3 dir = (transform.position - Camera.main.transform.position).normalized;
            if (lockX)
            {
                dir.x = InitalAxis.x;
            }
            if (lockY)
            {
                dir.y = InitalAxis.y;
            }
            if (lockZ)
            {
                dir.z = InitalAxis.z;
            }
            switch (billAxis)
            {
                case BillAxis.forward:
                     transform.forward = dir;
                    break;
                case BillAxis.back:
                     transform.forward = -dir;
                    break;
                case BillAxis.top:
                     transform.up = dir;
                    break;
                case BillAxis.bottom:
                     transform.up = -dir;
                    break;
                case BillAxis.left:
                     transform.right = -dir;
                    break;
                case BillAxis.right:
                     transform.right = dir;
                    break;
                default:
                     transform.forward = dir;
                    break;
            }


           // transform.forward = (transform.position - Camera.main.transform.position).normalized;
          //  transform.LookAt(Camera.main.transform);
        }
	}
}
