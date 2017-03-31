using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class CEDisplayWindow
{
    public static void AddCEDisplayWindow(string key, CEDisplayWindow cedisplay)
    {
        if (cedisplayWindowBook == null)
        {
            cedisplayWindowBook = new Dictionary<string, CEDisplayWindow>();
        }
        cedisplayWindowBook.Add(key, cedisplay);
    }
    public static Dictionary<string, CEDisplayWindow> CEDisplayWindowBook
    {
        get
        {
            if (cedisplayWindowBook == null)
            {
                cedisplayWindowBook = new Dictionary<string, CEDisplayWindow>();
            }
            return cedisplayWindowBook;
        }

        set
        {
            if (cedisplayWindowBook == null)
            {
                cedisplayWindowBook = new Dictionary<string, CEDisplayWindow>();
            }
            cedisplayWindowBook = value;
        }
    }
    static Dictionary<string, CEDisplayWindow> cedisplayWindowBook;
    public string Symbol;
    public string Valence;
    public string Electron;
    public float Electric;
    public float AtomicWeight;
    public string Structure;
    public int FamilyValue;
    public int WeekValue;
    public string ElementCH;
    public string ElementEN;
    public float ARadius;
    public string AColor;
    public CEDisplayWindow(Dictionary<string, string> infos)
    {
        Symbol = infos["Symbol"];
        Valence = infos["Valence"];
        int tryInt = 0;
        if(int.TryParse(Valence,out tryInt))
        {
            if (tryInt > 0)
            {
                Valence = "+" + Valence;
            }
        }
       // Debug.LogWarning("Valence:" + Valence);
        Electron = infos["Electronics"];
        Electric = float.Parse(infos["Electro"]);
        AtomicWeight = float.Parse(infos["AWeight"]);
        Structure = infos["CStruct"];
        FamilyValue = int.Parse(infos["Family"]);
        WeekValue = int.Parse(infos["Cycle"]);
        ElementCH = infos["CHName"];
        ElementEN = infos["ENName"];
        ARadius = float.Parse(infos["ARadius"]);
        AColor = infos["AColor"];
    }
}
[System.Serializable]
public class LayerFormula
{
    public UILabel Head_Label;
    public UILabel Electro_Head_Label;
    public UILabel Corner_Head_Label;
    public LayerFormula(GameObject electroObject)
    {
        Electro_Head_Label = electroObject.GetComponent<UILabel>();
        Corner_Head_Label = electroObject.transform.FindChild("Corner_Label").GetComponent<UILabel>();
    }
}
public class DisplayWindow : FunctionBoard {
    public static DisplayWindow OnSelectedDisplayWindow
    {
        get
        {
           if (onSelecteddisplayWindow == null)
            {
                onSelecteddisplayWindow = GameObject.FindObjectOfType<DisplayWindow>();
                onSelecteddisplayWindow.OnSelection();
            }
            Debug.LogWarning("Get OnSelect DisplayWindow"+ onSelecteddisplayWindow);
            return onSelecteddisplayWindow;
        }

        set
        {
            if (onSelecteddisplayWindow != null)
            {
                onSelecteddisplayWindow.OnDeSelection();
            }

            onSelecteddisplayWindow = value;
            if(onSelecteddisplayWindow == null)
            {
                Debug.LogWarning("没有OnSelectedWindow");
            }else{
                onSelecteddisplayWindow.OnSelection();
            }
        }
    }
    static DisplayWindow onSelecteddisplayWindow = null;
    public UILabel Valence;
    public LayerFormula layerFormula;
    public List<GameObject> LayerFormulaList;
    // public UILabel Electron;
    public UILabel Electric;
    public UILabel AtomicWeight;
    public UILabel Structure;
    public UILabel FamilyValue;
    public UILabel WeekValue;
    public UILabel ElementCH;
    public UILabel ElementEN;
    public AtomController atom;
    CEDisplayWindow currentCE;

    public void ResetWindow()
    {
        if (LayerFormulaList != null && LayerFormulaList.Count >0)
        {
            foreach (GameObject lf in LayerFormulaList)
            {
                Destroy(lf);
            }
            LayerFormulaList = new List<GameObject>();
        }
        if (layerFormula.Head_Label == null)
        {
            layerFormula.Head_Label = transform.FindChild("Electron_Window/Head_Label").GetComponent<UILabel>();
        }
    }

    public void LoadCEDisplayWindow(CEDisplayWindow cedisplay)
    {
        ResetWindow();
        atom.ResetAtom();
        string header = "";
        Dictionary<string,ElectronLayer> electronLayer = ElectronLayer.ElectronLayersMaker(cedisplay.Electron, out header,false);
     //   Debug.Log(header);
        layerFormula.Head_Label.text = (header == "")?"":"["+header+"]";
        int index = 0;
        LayerFormula lastLF = null;
        foreach (string key in electronLayer.Keys)
        {
            GameObject newLayer = Instantiate(layerFormula.Electro_Head_Label.gameObject) as GameObject;
            newLayer.name = newLayer.name.Replace("(Clone)", "");
            LayerFormulaList.Add(newLayer);
            newLayer.transform.parent = layerFormula.Electro_Head_Label.transform.parent;
            newLayer.transform.localPosition = layerFormula.Electro_Head_Label.transform.localPosition;
            newLayer.transform.localRotation = layerFormula.Electro_Head_Label.transform.localRotation;
            newLayer.transform.localScale = layerFormula.Electro_Head_Label.transform.localScale;
            foreach (UILabel ul in newLayer.GetComponentsInChildren<UILabel>(true))
            {
                ul.enabled = true;
            }
            Vector3 v3 =
                newLayer.transform.localPosition;
            LayerFormula lf = new LayerFormula(newLayer);
            if (index == 0)
            {
                layerFormula.Corner_Head_Label.enabled = false;
                layerFormula.Electro_Head_Label.enabled = false;
            }
            v3.x = (lf.Electro_Head_Label.width + lf.Corner_Head_Label.width)  * index - 8;
            lf.Electro_Head_Label.text = electronLayer[key].LayerName.ToString().Replace("_","");
            lf.Corner_Head_Label.text = electronLayer[key].ElectronCount.ToString();
            newLayer.transform.localPosition = v3;
            layerFormula = lf;
            //Debug.Log(electronLayer[key].LayerName + " " + electronLayer[key].ElectronCount + " " + electronLayer[key].Speed + " ");
            index++;
            if (index == electronLayer.Keys.Count)
            {
                try
                {
                    Vector3 v = layerFormula.Electro_Head_Label.gameObject.transform.localPosition;
                    v.x = lastLF.Electro_Head_Label.transform.localPosition.x + (lastLF.Electro_Head_Label.width + lastLF.Corner_Head_Label.width);
                    layerFormula.Electro_Head_Label.gameObject.transform.localPosition = v;
                    Debug.LogWarning("等于！！！:" + layerFormula.Electro_Head_Label.gameObject.name);
                }
                catch { }
            }
            lastLF = layerFormula;
        }
        currentCE = cedisplay;
        atom = GetComponentInChildren<AtomController>();
        Valence.text = cedisplay.Valence;
        //Electron.text = cedisplay.Electron;
        Electric.text = cedisplay.Electric.ToString();
        AtomicWeight.text = cedisplay.AtomicWeight.ToString();
        Structure.text = cedisplay.Structure;
        FamilyValue.text = cedisplay.FamilyValue.ToString();
        WeekValue.text = cedisplay.WeekValue.ToString();
        ElementCH.text = cedisplay.ElementCH;
        ElementEN.text = cedisplay.ElementEN;
        atom.LoadAtom(currentCE.Symbol, AtomController.AColorToColor(cedisplay.AColor), cedisplay.Electron, currentCE.ARadius, cedisplay.Valence);
    }
    public string initalElementSysmbol = "K";
    public override void InitWnd()
    {
        base.InitWnd();
     //   DisplayWindow.OnSelectedDisplayWindow = this;
        GetComponent<OnSelection>().OnPointerSelection.Add(new EventDelegate(OnSelection));
        GetComponent<OnSelection>().OnPointerDeSelection.Add(new EventDelegate(OnDeSelection));
        LoadCEDisplayWindow(CEDisplayWindow.CEDisplayWindowBook[initalElementSysmbol]);
    }

    public void OnSelection()
    {
        //Debug.LogWarning()
        if (DisplayWindow.onSelecteddisplayWindow != null && DisplayWindow.onSelecteddisplayWindow == this)
        {
            Debug.LogWarning("DisplayWindow.onSelecteddisplayWindow != null && DisplayWindow.onSelecteddisplayWindow == this");
            DisplayWindow.onSelecteddisplayWindow.gameObject.GetComponent<OnSelection>().FadeState(true);
            return;
        }
        else
        {
            Debug.LogWarning("DisplayWindow.onSelecteddisplayWindow != this");
            if (DisplayWindow.onSelecteddisplayWindow != null)
            {
                Debug.LogWarning("Deselect:"+DisplayWindow.onSelecteddisplayWindow.name);
//                DisplayWindow.onSelecteddisplayWindow.gameObject.GetComponent<OnSelection>().FadeState(false);
                 DisplayWindow.onSelecteddisplayWindow.gameObject.GetComponent<OnSelection>().Press(transform);
            }
            Debug.LogWarning("Select: " + name);
            DisplayWindow.onSelecteddisplayWindow = this;
            if (!DisplayWindow.onSelecteddisplayWindow.gameObject.GetComponent<OnSelection>().isSelection)
            {
                DisplayWindow.onSelecteddisplayWindow.gameObject.GetComponent<OnSelection>().FadeState(true);
                //   DisplayWindow.onSelecteddisplayWindow.gameObject.GetComponent<OnSelection>().Press(transform);
            }
            // DisplayWindow.OnSelectedDisplayWindow.GetComponent<OnSelection>().Press(transform);
            //
            //   }

        }
    }

    public void OnDeSelection()
    {
        //Debug.LogWarning("OnDeSelection!!!!!!!!!!!!");
        if (DisplayWindow.onSelecteddisplayWindow = this)
        {
//            DisplayWindow.onSelecteddisplayWindow.gameObject.GetComponent<OnSelection>().FadeState(false);
            DisplayWindow.onSelecteddisplayWindow = null;
        }
    }
    void OnGUI()
    {
        return;
#if UNITY_EDITOR
        if(Application.platform == RuntimePlatform.WindowsEditor)
        {
            if (GUILayout.Button("Select centerAtom"))
            {
                GameObject.FindObjectOfType<CECardController>().OnPressed();
                /* if(DisplayWindow.OnSelectedDisplayWindow != null)
                 {
                     if(DisplayWindow.OnSelectedDisplayWindow.atom != null)
                     {
                         UnityEditor.Selection.activeObject = DisplayWindow.OnSelectedDisplayWindow.atom.gameObject;
                     }
                     else
                     {
                         Debug.LogWarning("No Selected Displaywindow Atom");
                     }
                 }
                 else
                 {
                     Debug.LogWarning("No Selected Displaywindow");
                 }*/
            }
            if (GUILayout.Button("Show Card Board"))
            {
                GameObject.FindObjectOfType<CardSpawner>().gameObject.GetComponent<FunctionBoard>().SwitchBoard();
            }


            if (GUILayout.Button("Select DisplayWnd"))
            {
                GameObject dw = 
                  GameObject.FindObjectOfType<DisplayWindow>().gameObject;
                dw.GetComponent<OnSelection>().Press(dw.transform);
            }
            }
#endif
    }
    public override void UpdateWnd()
    {
        base.UpdateWnd();
    }
}