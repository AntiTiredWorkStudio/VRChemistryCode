using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using atom;
public class PathCircle : Path {
    public LineRenderer lr;
    public bool OnRender = false;
    const float radius = 3.0f;
    const int cutCount = 10;

    public override List<Vector3> GetPath()
    {
        List<Vector3> v3 = new List<Vector3>();
        for (int i = 0; i < cutCount; i++)
        {
            Vector3 v = new Vector3();

            v.x = transform.position.x + Mathf.Cos(((
                    360.0f / (float)cutCount) * i - transform.eulerAngles.y
                )
                * Mathf.Deg2Rad) * radius;
            v.z = transform.position.z + Mathf.Sin(((
                    360.0f / (float)cutCount) * i - transform.eulerAngles.y
                )
                * Mathf.Deg2Rad) * radius;

            v.y = transform.position.y;
            v3.Add(v);
         //   Gizmos.color = Color.green;
          //  Gizmos.DrawSphere(v, 0.6f);
        }
        v3.Add(v3[0]);
        return v3;
    }
}
