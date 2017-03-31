using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ValenceCreateStruct
{
    public int index;
    public ValenceCreateStruct(Vector3 pos,float pre,int _index)
    {
        point = pos;
        posPrecentage = pre;
        index = _index;
    }
    public float posPrecentage;
    public Vector3 point;
}
public class ValenceSelectionFunction : FunctionBoard ,PointerEventsInterface{
    public PathCircle UIcreatedCircle;
    public GameObject SelectionButton;
    public int countValence = 1;
    public override void InitWnd()
    {
        base.InitWnd();
        // CreateCircle();
        //增加控制器事件响应
        PointerBase.AddEventSender(PointerType.BOTH, this as PointerEventsInterface, PointerEvents.UNCLICK,
            PointerEvents.TCLICK,
            PointerEvents.TUNCLICK,
            PointerEvents.TTOUCH,
            PointerEvents.TUNTOUCH,
            PointerEvents.TAXISMOVE,
            PointerEvents.CLICK);
        /*PointerBase.Instance(PointerType.LEFT).AddEventSender(this as PointerEventsInterface,
            PointerEvents.TCLICK,
            PointerEvents.TUNCLICK,
            PointerEvents.TTOUCH,
            PointerEvents.TUNTOUCH,
            PointerEvents.TAXISMOVE);*/
        //StartCoroutine(Open());
    }

    /*  IEnumerator Open()
       {
           yield return new WaitForSeconds(3.0f);
           OpenBoardWithAtom(GameObject.FindObjectOfType<AtomController>());
       }*/

    public AtomController targetAtom;

    void OnGUI()
    {
        if (GUILayout.Button("OnTouchpadClicked"))
        {
            //OnTouchpadClicked(GameObject.FindObjectOfType<Pointer>(),new ControllerClickedEventArgs());
        }
    }

    public void OpenBoardWithAtom(AtomController atom)
    {
       /* if (ButtonCircles != null)
        {
            foreach (ValenceButton bc in ButtonCircles)
            {
                Destroy(bc);
            }
            ButtonCircles.Clear();
        }*/
        Debug.LogWarning(atom.text3DName.text+":"+ atom.Intermo.valences.Count);
        targetAtom = atom;
        OpenBoard();
        List<int> vs = AtomController.ValenceStringToList(atom.ValenceString);
        if (!vs.Contains(0)) {
            vs.Add(0);
        }
        CreateCircle(vs);
    }

    public void CloseBoardAndRemoveAtom()
    {
     /*   foreach (ValenceButton vb in ButtonCircles)
        {
            Destroy(vb.gameObject);
        }
        ButtonCircles.Clear();*/
        CloseBoard();
        /* foreach (ValenceButton bc in ButtonCircles)
         {
             bc.UnloadButton();
             //Destroy(bc);
         }*/
    }

    /*public void OnValenceButtonRemoved(ValenceButton vb)
    {
        ButtonCircles.Remove(vb);
        if(ButtonCircles.Count == 0)
        {
            ButtonCircles.Clear();
            targetAtom = null;
            CloseBoard();
        }
    }*/

    public override void UpdateWnd()
    {
        base.UpdateWnd();
        if(targetAtom != null)
        {
            transform.position = targetAtom.transform.position;
        }
    }
    /*public void OpenBoardWithCountRound(int countRound)
    {
        OpenBoard();
        CreateCircle(countRound);
    }*/

    ValenceCreateStruct[] GetButtonPoints(int countRound)
    {
        List<ValenceCreateStruct> vcs = new List<ValenceCreateStruct>();
//        ValenceCreateStruct[] vcs = new ValenceCreateStruct[countRound];
        Transform[] points = UIcreatedCircle.PathTransforms;
     //   List<Vector3> TargetPoints = new List<Vector3>();
        float count = 0.0f;
        float seek = 1.0f/countRound;
        for (int i = 0; i < countRound; i++)
        {
            vcs.Add(new ValenceCreateStruct(iTween.PointOnPath(points, count),
              count,i));
         //   TargetPoints.Add(vcs[i].point);
            count = (count+ seek)%1.0f;
        }
        return vcs.ToArray();
    }

    List<ValenceButton> ButtonCircles = null;

    void CreateCircle(List<int> valences)
    {
        if (ButtonCircles != null && ButtonCircles.Count > 0) {
            foreach (ValenceButton vb in ButtonCircles)
            {
                Destroy(vb.gameObject);
            }
            ButtonCircles.Clear();
        }
        (SelectionButton as GameObject).SetActive(true);
        ButtonCircles = new List<ValenceButton>();
        foreach (ValenceCreateStruct v in GetButtonPoints(valences.Count))
        {
            GameObject button = Instantiate(SelectionButton, v.point, new Quaternion()) as GameObject;
            button.transform.parent = transform;
            button.transform.localScale = Vector3.one;
            button.transform.position = v.point;
            ValenceButton vb = button.GetComponent<ValenceButton>();
            vb.InitButton(UIcreatedCircle, v.posPrecentage,valences[v.index]);
            ButtonCircles.Add(vb);
        }
        (SelectionButton as GameObject).SetActive(false);
    }

    public UILabel slabel;

    public void OnTouched(object sender, ControllerClickedEventArgs e)
    {

    }

    public void OnUnTouched(object sender, ControllerClickedEventArgs e)
    {
        if (ControlPointer == null)
        {
            return;
        }
        MonoBehaviour mono = sender as MonoBehaviour;
        if (mono.gameObject.GetComponent<Pointer>() != ControlPointer) { return; }
        //slabel.text = "UnTouched";
        ControlPointer = null;
        CloseBoardAndRemoveAtom();
    }

    public void OnAxisMove(object sender, ControllerClickedEventArgs e)
    {
        if (!WndCotnroller.WndStatus)
        {
            return;
        }
       // Debug.LogWarning("AxisMove::" + e.touchpadAxis.ToString());
        slabel.text = e.touchpadAxis.ToString();
        //e.controllerIndex
    }

    public void OnTriggerClicked(object sender, ControllerClickedEventArgs e)
    {
//        if(targetAtom)
        //throw new NotImplementedException();
    }

    public void OnTriggerUnclicked(object sender, ControllerClickedEventArgs e)
    {
        if(ControlPointer == null)
        {
            return;
        }
        MonoBehaviour mono = sender as MonoBehaviour;
        if (mono.gameObject.GetComponent<Pointer>() != ControlPointer) { return; }
        ControlPointer = null;
        CloseBoardAndRemoveAtom();
        //throw new NotImplementedException();
    }
    Pointer ControlPointer;
    public void OnTouchpadClicked(object sender, ControllerClickedEventArgs e)
    {
        MonoBehaviour mono = sender as MonoBehaviour;
        ControlPointer = mono.gameObject.GetComponent<Pointer>();
        /*foreach (ValenceButton vb in ButtonCircles) {
            Destroy(vb.gameObject
                );
        }
        ButtonCircles.Clear();*/
        if (ControlPointer.LastOnPointerHandle!=null && ControlPointer.isActive)
        {
            transform.position =
                mono.gameObject.transform.position;
            OpenBoardWithAtom(ControlPointer.LastOnPointerHandle.gameObject.GetComponent<AtomController>());
        }
    }

    public void OnTouchpadUnclicked(object sender, ControllerClickedEventArgs e)
    {
        if (ControlPointer == null)
        {
            return;
        }
        MonoBehaviour mono = sender as MonoBehaviour;
        Debug.LogWarning("mono:" + mono.name);
        if (mono.gameObject.GetComponent<Pointer>() != ControlPointer) { return; }
        ControlPointer = null;
        CloseBoardAndRemoveAtom();
    }
}
