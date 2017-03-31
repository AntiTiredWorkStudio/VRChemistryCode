using UnityEngine;
using System.Collections;

public class FarView : MonoBehaviour {

    public static FarView farView;
    public Camera targetFarcam;
    Transform target;
	// Use this for initialization
	void Start () {
        farView = this;
        //targetFarcam = transform.GetComponentInChildren<Camera>();

    }
	
	// Update is called once per frame
	void Update () {
        if (target != null)
        {
            targetFarcam.transform.forward = Vector3.Lerp(targetFarcam.transform.forward, (target.position - targetFarcam.transform.position).normalized, Time.deltaTime * 1.5f);
           
            targetFarcam.transform.position = Vector3.Lerp(targetFarcam.transform.position,target.position - target.forward * 0.15f,Time.deltaTime*1.5f);
        }
	}

    public void LoadFocusObject(Transform _target) {
        target = _target;
    }
}
