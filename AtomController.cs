using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class AtomController : MonoBehaviour
{
    public static float SizeToRadius(float size)
    {
        return 3.0f;// size / 0.015625f / 0.3f;// / 0.09f;
    }
    public static float SizeTo_Size(float size)
    {
        return size / 0.015625f / 0.1f / 0.09f;
    }
    public static float _SizeToSize(float _size)
    {
        return _size * 0.015625f * 0.1f * 0.09f;
    }
    public void SetPos(Vector3 pos)
    {
        StartCoroutine(setPOS(pos));
    }

    IEnumerator setPOS(Vector3 pos)
    {
        yield return new WaitForSeconds(0.1f);
        for (int i = 0; i < 10; i++)
        {
            transform.position = pos;
            yield return new WaitForSeconds(0.1f);
        }
        GetComponent<DragCommand>().enabled = true;
        GetComponent<Intermolecular>().enabled = true;
        enabled = true;
    }
    public static List<int> ValenceStringToList(string valence)
    {
        string targetString = valence+",";
        //  Debug.LogWarning(targetString);
        int startSeek = 0;
        int seek = 0;
        List<int> valences = new List<int>();
        for (int i = 0; i < targetString.Length; i++)
        {
            if (targetString[i] == ',')
            {
                string cut = targetString.Substring(startSeek, seek - startSeek);
                // targetString = targetString.Remove(startSeek, seek - startSeek);
                //  Debug.LogWarning(cut);
               Debug.LogWarning(int.Parse(cut));

                valences.Add(int.Parse(cut));
               // colorQueue.Enqueue((byte)int.Parse(cut));
                startSeek = seek + 1;
                /*  if (colorQueue.Count >= 4)
                  {
                      Debug.LogWarning("break");
                      break;
                  }*/
            }
            //Debug.Log(seek);
            seek++;
        }
        //    Debug.LogWarning((byte)int.Parse(targetString.Substring(startSeek, seek - startSeek)));
        return valences;
    }
    public static Color AColorToColor(string aColor)
    {
        string targetString = aColor.Replace("#", "");
      //  Debug.LogWarning(targetString);
        int startSeek = 0;
        int seek = 0;
        Queue<byte> colorQueue = new Queue<byte>();
        for (int i = 0;i< targetString.Length;i++)
        {
            if(targetString[i] == ',')
            {
                string cut = targetString.Substring(startSeek, seek - startSeek);
               // targetString = targetString.Remove(startSeek, seek - startSeek);
              //  Debug.LogWarning(cut);
           //     Debug.LogWarning((byte)int.Parse(cut));
                colorQueue.Enqueue((byte)int.Parse(cut));
                startSeek = seek+1;
              /*  if (colorQueue.Count >= 4)
                {
                    Debug.LogWarning("break");
                    break;
                }*/
            }
            //Debug.Log(seek);
            seek++;
        }
    //    Debug.LogWarning((byte)int.Parse(targetString.Substring(startSeek, seek - startSeek)));
        colorQueue.Enqueue((byte)int.Parse(targetString.Substring(startSeek, seek - startSeek)));
        byte r = colorQueue.Dequeue();
        byte g = colorQueue.Dequeue();
        byte b = colorQueue.Dequeue();
        return new Color32(r, g, b, 255);
    }
    public ElectronController ELECCTRONS;
    public string ValenceString;
    public float size;
    
    public string AtomID
    {
        get
        {
            return name +"_" + GetInstanceID();
        }
    }



    public Intermolecular Intermo
    {
        get
        {
            return GetComponent<Intermolecular>();
        }
    }

    /// <summary>
    /// 拖拽网
    /// </summary>
    public Compound CompoundPivot
    {
        get
        {
            return compoundPivot;
        }
        set
        {
            compoundPivot = value;
        }
    }

    public Compound compoundPivot = null;

    public TextMesh text3DName {
        get
        {
           return transform.FindChild("3DText/Name").GetComponent<TextMesh>();
        }
    }
    public TextMesh text3DValence
    {
        get
        {
            return transform.FindChild("3DText/Name/Valence").GetComponent<TextMesh>();
        }
    }

    public void ResetAtom()
    {
        //Debug.LogWarning("Reset Atom");
        if(ELECCTRONS == null)
        {
            ELECCTRONS = transform.FindChild("ELECCTRONS").gameObject.GetComponent<ElectronController>();
        }
        ELECCTRONS.ResetElectron();
    }
    public void LoadAtom(string name,Color color, string electronLayerString, float _size,string valance)
    {
        string Header = "";
        Dictionary<string, ElectronLayer> electronLayer = ElectronLayer.ElectronLayersMaker(electronLayerString, out Header, true);
        if (ELECCTRONS == null)
        {
            ELECCTRONS = transform.FindChild("ELECCTRONS").gameObject.GetComponent<ElectronController>();
        }
        ELECCTRONS.ElectronConfig(electronLayer, _size);
        ValenceString = valance;
        /*
         
        
                通过电子式的解析结果创建电子轨道并判断哪些轨道在一个电子层中。 

         
         */
        text3DName.text = name;
        //  float brightness = 0.299f * color.r + 0.587f * color.g + 0.114f * color.b;
        //      Debug.LogWarning(brightness);
        /*   if (brightness > 0.5f)
           {
               text3D.color = Color.black;
           } else {
               text3D.color = Color.white;
           }*/
//        text3DName.
        text3DName.font.material.color = Color.white;
        GetComponent<Renderer>().material.SetColor("_EmissionColor",color*0.6f);
        //   transform.localScale = Vector3.one * size;
        size = _SizeToSize(_size);// * 0.015625f* 0.1f*0.09f;
        //Debug.LogWarning("Valence:" + valance);
        //ValenceStringToList(valance);
    }

    public void HighQualityView()
    {

        Color color = GetComponent<Renderer>().material.GetColor("_EmissionColor");
        Material mat = new Material(Shader.Find("Shader Forge/Force field"));
        mat.shader = Shader.Find("Shader Forge/Force field");
        mat.SetColor("_Color", color);
        mat.SetFloat("_noies_power", 10.0f);
        mat.SetFloat("_UV", 0.6f);
        mat.SetColor("_fresnel_color", new Color32((byte)111, (byte)111, (byte)111, (byte)255));
        mat.SetFloat("_fresnel_value", 1.5f);

        //  float unbrightness = 1.0f-0.299f * color.r + 0.587f * color.g + 0.114f * color.b;
        mat.SetFloat("_strong", 1.0f);
        mat.SetTexture("_Texture", Resources.Load("img/Smoke01") as Texture);
        mat.SetTexture("_2tex", Resources.Load("img/Smoke01") as Texture);
        GetComponent<Renderer>().material = mat;
        if (GetComponent<AtomController>())
        {
            //在这里隐藏电子层，隐藏灯光并设置原子大小。
            GetComponent<AtomController>().OnLeaveBoard();
            GetComponent<AtomController>().text3DName.color = Color.white;
            Dictionary<int, ElectronLayer> dictionary = GetComponent<AtomController>().ELECCTRONS.ElectronLayerList;

            int index = 0;

            foreach (int key in dictionary.Keys)
            {
                index++;
                GameObject layer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                layer.GetComponent<Renderer>().material = mat;
                layer.transform.parent = transform;
                layer.transform.localPosition = Vector3.zero;
                layer.transform.rotation = transform.rotation;
                layer.transform.localScale = Vector3.one - Vector3.one * ((float)index / (float)dictionary.Keys.Count);
            }
        }
    }

    // Use this for initialization
    void Start () {
        ELECCTRONS = transform.FindChild("ELECCTRONS").gameObject.GetComponent<ElectronController>();
      //  text3D = transform.FindChild("Name").GetComponent<TextMesh>();
    }

    public void OnLeaveBoard()
    {
        ELECCTRONS.gameObject.SetActive(false);
        if (GetComponentInChildren<Light>())
        GetComponentInChildren<Light>().enabled = false;
        transform.localScale = Vector3.one * size;
    }

    void OnCollisionEnter()
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }
    void OnCollisionStay()
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }
    void OnCollisionExit()
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

}
