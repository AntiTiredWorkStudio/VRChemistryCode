using UnityEngine;
using System.Collections;
[RequireComponent(typeof(DragCommand))]
public class AutoAdjustRotation : MonoBehaviour {
    DragCommand dc;

    public bool adjustX;
    public bool adjustY;
    public bool adjustZ;

    public Vector3 TargetEulerAngle;

    public float LerpSpeed = 1.5f;



   void Start () {
        dc = GetComponent<DragCommand>();
        dc.IdleUpdate.Add(AdjustAction);
	}

    Vector3 GetTargetEulerAngle()
    {
        Vector3 result = TargetEulerAngle;
        if (!adjustX)
        {
            result.x = dc.transform.eulerAngles.x;
        }
        if (!adjustY)
        {
            result.y = dc.transform.eulerAngles.y;
        }
        if (!adjustZ)
        {
            result.z = dc.transform.eulerAngles.z;
        }
        return result;
    }

    void AdjustAction()
    {
        dc.transform.rotation = Quaternion.Lerp(dc.transform.rotation,Quaternion.Euler(GetTargetEulerAngle()),Time.deltaTime*LerpSpeed);
    }
}
