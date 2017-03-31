using UnityEngine;
using System.Collections;
//[RequireComponent(typeof(UISprite))]
//[RequireComponent(typeof(DragCommand))]
public class SpriteColorSwitch : MonoBehaviour {
    DragCommand dc;
    UISprite uis;
    Color prepareStatusColor;
    Color normalStatusColor;
    Color dragStatusColor;
    void Start()
    {
        dc = GetComponentInParent<DragCommand>();
        uis = GetComponentInParent<UISprite>();
        prepareStatusColor = Color.white;
        normalStatusColor = new Color(0.16f, 0.964f, 1.0f);
        dragStatusColor = new Color(1.0f,0.905f,0.384f);
        uis.color = normalStatusColor;
        dc.PrepareDrag.Add(PrepareState);
        dc.StartDrag.Add(DragState);
        dc.FinishedDrag.Add(NormalState);
        
    }
    void PrepareState()
    {
        uis.color = prepareStatusColor;
    }
    void NormalState()
    {
        uis.color = normalStatusColor;
    }

    void DragState()
    {
        uis.color = dragStatusColor;
    }
}
