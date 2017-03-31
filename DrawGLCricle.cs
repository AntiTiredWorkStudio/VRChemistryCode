using UnityEngine;
using System.Collections;

public class DrawGLCricle : MonoBehaviour {

    public Material mat;


    // Use this for initialization
    void Start()
    {
    }


    void OnPostRender()
    {
        GL.PushMatrix();
        mat.SetPass(0);
        //绘制2D线段，注释掉GL.LoadOrtho();则绘制3D图形
        GL.LoadOrtho();
        //开始绘制直线类型，需要两个顶点
        GL.Begin(GL.LINES);
        //绘制起点，绘制的点需在Begin和End之间
        GL.Vertex3(0, 0, 0);
        GL.Vertex3(1, 1, 0);
        GL.End();
        GL.Flush();
        GL.PopMatrix();
    }
}
