using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ElectronLayer
{
    public const string ElectronHe = "[2,0.1]";
    public const string ElectronNe = "[2,0.1][2,0.08][6,0.08]";
    public const string ElectronAr = "[2,0.1][2,0.08][6,0.08][2,0.07][6,0.07]";

    public enum ElectronLayerName
    {
        _1s = 0,
        _2s = 1,
        _2p, _3s, _3p, _4s, _3d, _4p, _5s, _4d, _5p, _6s, _4f, _5d, _6p, _7s, _5f, _6d
    }
    public int ElectronCount;
    public float Speed;
    public ElectronLayerName LayerName;
    public ElectronLayer(int electronCount,float speed,ElectronLayerName layerName)
    {
        ElectronCount = electronCount;
        Speed = speed;
        LayerName = layerName;
    }

    public static Dictionary<string, ElectronLayer> ElectronLayersMaker(string keyLine, out string header, bool descriptFlag = true)
    {
        Dictionary<string, ElectronLayer> _ElectronLayers = new Dictionary<string, ElectronLayer>();
        string targetString = keyLine;
        bool endSerching = false;
        int startSeek = 0;
        int seek = 0;
        header = "";
        Queue<string> subLayer = new Queue<string>();
        int LayerIndex = 0;
        if (targetString.Contains("<Ar>"))
        {
            if (descriptFlag)
            {
                 targetString = targetString.Replace("<Ar>", "<Ar>" + ElectronAr);
            }
            else
            {
                LayerIndex += 5;
            }
        }
        if (targetString.Contains("<Ne>"))
        {
            if (descriptFlag)
            {
                targetString = targetString.Replace("<Ne>", "<Ne>" + ElectronNe);
            }
            else
            {
                LayerIndex += 3;
            }
        }
        if (targetString.Contains("<He>"))
        {
            if (descriptFlag)
            {
                targetString = targetString.Replace("<He>", "<He>" + ElectronHe);
            }
            else
            {
                LayerIndex += 1;
            }
        }
        while (!endSerching)
        {
            switch (targetString[seek])
            {
                case '[':
                    startSeek = seek;
                    break;
                case ']':
                    string key = targetString.Substring(startSeek, (seek + 1) - startSeek);
                    subLayer.Enqueue(key);
                    break;
                case '<':
                    startSeek = seek;
                    break;
                case '>':
                     header = targetString.Substring(startSeek, (seek + 1) - startSeek);
                    header = header.Replace("<", "");
                    header = header.Replace(">", "");
                    break;
                default:
                    break;
            }

            if (seek >= targetString.Length - 1)
            {
                endSerching = true;
            }
            else
                seek++;
        }
        foreach (string st in subLayer)
        {
            string targetLayer =
                st.Replace("[", "").Replace("]", "");
            seek = 0;
            startSeek = 0;
            bool HasCut = false;
            while (!HasCut && (seek < targetLayer.Length - 1))
            {
                switch (targetLayer[seek])
                {
                    case ',':
                        startSeek = seek;
                        HasCut = true;
                        break;
                    default:
                        break;
                }
                seek++;
            }

            _ElectronLayers.Add(((
                ElectronLayer.ElectronLayerName)LayerIndex).ToString(),
                new ElectronLayer(
                    int.Parse(targetLayer.Substring(0, startSeek)),
                    float.Parse(targetLayer.Substring(startSeek + 1, targetLayer.Length - 1 - startSeek))
                , (ElectronLayer.ElectronLayerName)LayerIndex));

            LayerIndex++;
        }
        return _ElectronLayers;
    }
}

public class CECardInfo
{
    public string ElectronLayerDescription = "";
    public string ValencesString
    {
        get
        {
            return valences.Replace("/", "'\n").Replace("'", "");
        }
    }
    public CECardInfo(Dictionary<string, string> infos)
    {
        symbol = infos["Symbol"] as string;
        name = infos["CHName"] as string;
        index = int.Parse(infos["EIndex"]);
        valences = infos["Valence"] as string;
        atomMass = float.Parse(infos["AWeight"]);
        x = int.Parse(infos["Family"] );
        y = int.Parse(infos["Cycle"]);
        ElectronLayerLine = infos["Electronics"] as string;
    }
    public string symbol;
    public string name;
    public int index;
    public string valences;
    public float atomMass;
    public int x;
    public int y;
    public string ElectronLayerLine;
}
public class CECardController : MonoBehaviour {
    public UILabel SymbolLabel;
    public UILabel NameLabel;
    public UILabel IndexLabel;
    public UILabel ValenceLabel;
    public UILabel AtomMassLabel;

    public static string  GetValencesString(string commaString)
    {
        if(commaString == "0")
        {
            return "0";
        }
        if(commaString.Length == 1 && !(commaString.StartsWith("+") || commaString.StartsWith("-")))
        {
            return "+" + commaString;
        }
        return commaString.Replace(",", "\n");
    }

    public void LoadCECardInfo(CECardInfo CEInfo)
    {
        SymbolLabel.text = CEInfo.symbol;
        NameLabel.text = CEInfo.name;
        IndexLabel.text = CEInfo.index.ToString();
        ValenceLabel.text = GetValencesString(CEInfo.valences);
        AtomMassLabel.text = CEInfo.atomMass.ToString("F4");
        gameObject.name = SymbolLabel.text+"_Card";
    }
    // Use this for initialization
    void Start () {
        GetComponent<OnPointer>().OnPointerPressed.Add(new EventDelegate(OnPressed));
      //  string header = "";
      //  Dictionary<string,ElectronLayer> election = ElectronLayer.ElectronLayersMaker("<Ar>[2,0.06][9,0.06]",out header);
       /* Debug.LogWarning("Header:"+header);
        foreach (string key in election.Keys)
        {
            Debug.LogWarning(election[key].LayerName.ToString() +" count:"+ election[key].ElectronCount +" speed:"+ election[key].Speed);
        }*/
    }

    public void OnPressed()
    {
        if (DisplayWindow.OnSelectedDisplayWindow == null)
        {
            Debug.LogWarning("NoOnSelectionDisoplayWindow");
            return;
        }
        else
        {
            DisplayWindow.OnSelectedDisplayWindow.LoadCEDisplayWindow(CEDisplayWindow.CEDisplayWindowBook[SymbolLabel.text]);
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
