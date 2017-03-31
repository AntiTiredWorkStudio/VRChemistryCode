using UnityEngine;
using System.Collections;
using atom;
public class ElectronMovement : MonoBehaviour {
    public Path targetPath;
    public float precentage = 0.0f;
    public float Speed = 0.5f;
    void Start()
    {
        precentage = Random.Range(0f, 1f);
    }
    void Update()
    {
        if(targetPath == null)
        {
            return;
        }
        precentage = (precentage + Speed * Time.deltaTime)%1.0f;
        transform.position = 
          iTween.PointOnPath(
            targetPath.PathTransforms, precentage);
    //    iTween.DrawPath(targetPath.PathTransforms);
    }
}
