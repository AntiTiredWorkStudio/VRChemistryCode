using UnityEngine;
using System.Collections;

public class TestUnit : MonoBehaviour {
#if UNITY_EDITOR
    void Update () {
        if (UnityEditor.Selection.activeObject == gameObject)
        {
            transform.Translate(Input.GetAxis("Vertical") * Vector3.forward * 0.0001f + Input.GetAxis("Horizontal") * Vector3.left * 0.0001f);
        }
      //  transform.Translate(Vector3.left * Time.deltaTime*0.005f);
	}
#endif
}
