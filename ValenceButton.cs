using UnityEngine;
using System.Collections;
using atom;
public class ValenceButton : MonoBehaviour {
    public Path circleButton;
    public float targetPos = 0.0f;
    public float currentPos = 0.0f;
    public bool isInit = false;
    public bool isUnload = false;
    public UILabel valenceLabel;
    public int valence;
    public void InitButton(Path _circleButton,float _targetPos,int _valence)
    {
        if (!isInit)
        {
            isInit = true;
        }
        circleButton = _circleButton;
        targetPos = _targetPos;
        valence = _valence;
        valenceLabel.text = valence.ToString();
    }

  /*  public void UnloadButton()
    {
        if (!isInit && !isUnload)
        {
            isInit = true;
            isUnload = true;
            targetPos = 0.0f;
        }
    }*/


    void Update () {
        if (isInit)
        {
            currentPos = Mathf.Lerp(currentPos, targetPos, Time.deltaTime*3.5f);
            Vector3 targetPosVec = iTween.PointOnPath(circleButton.PathTransforms, currentPos);
            Vector3 deltaPosVec = iTween.PointOnPath(circleButton.PathTransforms, currentPos);
            transform.position = deltaPosVec;
            if (Mathf.Abs(currentPos - targetPos) <= 0.006f)
            {
              /*  if (isUnload)
                {
                    gameObject.GetComponentInParent<ValenceSelectionFunction>().OnValenceButtonRemoved(this);
                    Destroy(gameObject);
                }*/
                isInit = false;
            }
        }
	}
}
