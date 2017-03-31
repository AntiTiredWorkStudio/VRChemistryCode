using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class CardSpawner : FunctionBoard {
    // public List<string, Dictionary<string, object>> ElementDataForm;
    protected static CardSpawner cardSpawner = null;
    public static CardSpawner Instance
    {
        get
        {
            if (cardSpawner == null)
            {
                cardSpawner = GameObject.FindObjectOfType<CardSpawner>();
            }
            return cardSpawner;
        }
    }

    public CEDisplayWindow CrerateCEDisplayWindow(string symbol)
    {
        foreach (Dictionary<string, string>  ed in ElementData) {
            if (ed["Symbol"] == symbol)
            {
                return new CEDisplayWindow(ed);
            }
        }
        return null;
    }

   public List<Dictionary<string, string>> ElementData
    {
        get
        {
            if(ElementDataForm == null)
            {
                DbAccess L =
              DbAccess.L;
                if (!L.HasTable("AElement"))
                {
                    Debug.LogWarning("数据库中没有找到AElement表，无法创建元素周期表");
                }
                else
                {
                    ElementDataForm = L.ReadFullTableReturnList("AElement");
                }
            }
            return ElementDataForm;
        }
    }
   public List<Dictionary<string, string>> ElementDataForm;
    public Object cardPrefab;
    public const float CardWidth = 27f;
    public const float CardHeight = -40f;
    public Vector3 offset = Vector3.zero;
    // Use this for initialization
    void Awake ()
    {
        if (cardPrefab == null)
        {
            return;
        }
        (cardPrefab as GameObject).SetActive(false);
        InitFromDatabase();
    }
	void InitFromDatabase()
    {
        (cardPrefab as GameObject).SetActive(true);
        DbAccess L =
            DbAccess.L;
        if (!L.HasTable("AElement"))
        {
            Debug.LogWarning("数据库中没有找到AElement表，无法创建元素周期表");
            return;
        }
        ElementDataForm = L.ReadFullTableReturnList("AElement");
        Vector3 startPosition = (cardPrefab as GameObject).transform.position;
        CEDisplayWindow.CEDisplayWindowBook = new Dictionary<string, CEDisplayWindow>();
        foreach (Dictionary<string, string> d in ElementDataForm)
        {
            GameObject card = Instantiate(cardPrefab) as GameObject;
            card.transform.parent = transform;
            try
            {
                card.transform.localPosition = new Vector3(
                         startPosition.x + CardWidth * float.Parse(d["Family"] as string)+offset.x, startPosition.y + CardHeight*float.Parse(d["Cycle"] as string) + offset.y, startPosition.z + offset.z
                   );
            }
            catch//(System.Exception e)
            {
            //    Debug.LogWarning("Exception:" + e.Message);
                continue;
            }
            card.transform.rotation = (cardPrefab as GameObject).transform.rotation;
            card.transform.localScale = Vector3.one * 0.2f;
           CECardInfo cei = new CECardInfo(d);
            card.GetComponent<CECardController>().LoadCECardInfo(cei);
           // Debug.Log(d["Symbol"]);
            if (!CEDisplayWindow.CEDisplayWindowBook.ContainsKey(d["Symbol"]))
            {
        //        Debug.Log(d["Symbol"]);
                CEDisplayWindow.AddCEDisplayWindow(d["Symbol"], new CEDisplayWindow(d));
                //CEDisplayWindow.CEDisplayWindowBook.Add(d["Symbol"], new CEDisplayWindow(d));
            }
        }
        (cardPrefab as GameObject).SetActive(false);
    }
	// Update is called once per frame
	void Update () {
	
	}
}
