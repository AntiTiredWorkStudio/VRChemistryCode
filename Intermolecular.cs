using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Intermolecular : MonoBehaviour
{
    public static void GetConnections(Intermolecular intermo, ref Dictionary<string, AtomController> Atoms)
    {
        List<Bond> Connections = intermo.Connections;
        if (Connections == null || Connections.Count == 0)
        {
            return;
        }
        foreach (Bond bond in Connections)
        {
            if (bond.NoticeDestroy || (bond.paramaterAtoms.Count ==0) || (bond.paramaterBonds.Count == 0))
            {
                Debug.LogWarning("GC:" + bond.name + "_" + bond.NoticeDestroy +" or empty");
                continue;
            }
            AtomController atom = bond.GetAnotherAtom(intermo.Atom);
            if(atom == null)
            {
                continue;
            }
            /*AtomController atom = bond.RootAtom;*/
            if (!Atoms.ContainsKey(atom.AtomID))
            {
                Atoms.Add(atom.AtomID, atom);
                Debug.LogWarning("GC:" + atom.AtomID);
                GetConnections(atom.Intermo, ref Atoms);
            }
            else
            {
                continue;
            }
        }
    }
    public static Intermolecular CreateAtomIntermolecular(AtomController atom)
    {
        Intermolecular instance = atom.gameObject.AddComponent<Intermolecular>();
        instance.atom = atom;
        return instance;
    }

    public static void DeleteAtomIntermolecular(AtomController atom)
    {
        if (atom.gameObject.AddComponent<Intermolecular>())
        {
            Destroy(atom.gameObject.AddComponent<Intermolecular>());
        }
    }

    public const string pivotName = "IntermolecularPivot";

    public List<int> valences;
    public AtomController Atom {
        get
        {
            if (atom == null)
            {
                atom = GetComponent<AtomController>();
            }
            return atom;
        }
    }

    AtomController atom = null;

    public Transform pivot
    {
        get
        {
            if (piv == null)
            {
                if (transform.FindChild(pivotName) == null) {
                    piv = (new GameObject(pivotName)).transform;
                    piv.transform.parent = transform;
                    piv.transform.localPosition = Vector3.zero;
                    piv.transform.localRotation = new Quaternion();
                    piv.transform.localScale = Vector3.one;
                }
                else
                {
                    piv = transform.FindChild(pivotName);
                    TriggerCollider.ToString();
                }
                // SphereCollider spc = piv.gameObject.AddComponent<SphereCollider>();
            }
            return piv;
        }
    }


    SphereCollider trigger = null;
    public SphereCollider TriggerCollider
    {
        get
        {
            if (trigger == null)
            {
                if (pivot.GetComponent<SphereCollider>())
                {
                    trigger = pivot.GetComponent<SphereCollider>();
                }
                else
                {
                    trigger = pivot.gameObject.AddComponent<SphereCollider>();
                }
                trigger.radius = AtomController.SizeToRadius(Atom.size);
                trigger.isTrigger = true;
            }
            return trigger;
        }
    }

    Transform piv = null;
    public List<Bond> FreeBonds;
    public List<Bond> ConnectedBonds;

    public List<Bond> ChildsBonds;
    void Start() {
        if (!Checking)
        {
            //    InitIntermolecularPivot();
            CreateValences();
        }

        TriggerCollider.radius = AtomController.SizeToRadius(Atom.size);
        CollectFreeBonds();
    }

    bool Checking
    {
        get
        {
            return transform.FindChild(pivotName) != null && ChildsBonds.Count > 0;
        }
    }

    public int mainValence = 0;

    public bool HasBreakBond
    {
        get
        {
            return FreeBonds != null && FreeBonds.Count > 0;
        }
    }

    /// <summary>
    /// 拿到最近的一条化学键
    /// </summary>
    /// <param name="atom"></param>
    /// <returns></returns>
    public Bond GetNearestBreakBond(AtomController atom)
    {
        Vector3 targetPos = atom.transform.position;
        float distance = Mathf.Infinity;
        Bond targetBond = null ;
        foreach(Bond b in FreeBonds)
        {
            if (BondDirTargets.ContainsValue(b.transform))
            {
                continue;
            }
            float temp = Vector3.Distance(b.HeadNode.transform.position, targetPos);
            if(temp < distance)
            {
                distance = temp;
                targetBond = b;
            }
        }
        return targetBond;
    }

  /*  void InitIntermolecularPivot() {
        transform.parent = null;
        transform.localScale = Vector3.one * Atom.size;
        SphereCollider spc = pivot.gameObject.AddComponent<SphereCollider>();
        spc.radius = 1.5f;
        spc.isTrigger = true;
    }*/
    void InitValences()
    {
        valences = AtomController.ValenceStringToList(Atom.ValenceString);
    }

    void InitBondList()
    {
        ChildsBonds = new List<Bond>();
        foreach (Bond bd in GetComponentsInChildren<Bond>())
        {
            if (!bd.isConnectBond && bd.transform.parent == transform)
            {
                ChildsBonds.Add(bd);
                mainValence += bd.RealValence;
            }
        }
    }

    public int defaultValence = 1;

    [ContextMenu("CreateByEditor")]
    void CreateValences()
    {
        pivot.ToString();
        foreach (Bond bd in GetComponentsInChildren<Bond>())
        {
            DestroyImmediate(bd.gameObject);
        }
        ChildsBonds.Clear();
    //    InitIntermolecularPivot();
        InitValences();
        InitBondList();
        Debug.LogWarning("Create Valences");
        mainValence = 0;
        SetValence(defaultValence);
    }

    public bool ConnectedAtom(AtomController atom)
    {
        foreach (Bond bd in ConnectedBonds)
        {
            if(bd.RootAtom == atom)
            {
                return true;
            }
        }
        return false;
    }

    void CollectFreeBonds()
    {
        FreeBonds = new List<Bond>();
        ConnectedBonds = new List<Bond>();
        foreach (Bond bd in ChildsBonds)
        {
            if (!bd.IsChild)
            {
                FreeBonds.Add(bd);
            }
            else
            {
                ConnectedBonds.Add(bd.ParentBond.GetAnotherBond(bd));
            }
        }
    }

    public void SetValence(int value)
    {
        //添加预置的化学键
        if (valences.Contains(value))
        {
            Atom.text3DValence.text = ((value > 0) ?( "+" + value ): ""+value);
            /* if (pivot.childCount >= 0)
              {
                  pivot.DestroyChildren();
              }*/
            int totalCreated = Mathf.Clamp(Mathf.Abs(value) - mainValence, 0, 1000);
            for(int i = 0; i < totalCreated; i++)
            {
                Bond bond = 
                    Bond.CreateBond(Atom, ((value > 0) ? (Bond.PlusMinus.POSITIVE) : Bond.PlusMinus.NAGATIVE));
                ChildsBonds.Add(bond);
                mainValence += bond.RealValence;
                /*   GameObject bond = new GameObject("molecular_"+i);
                   bond.transform.parent = pivot;
                   bond.transform.localPosition = Vector3.zero;
                   bond.transform.localRotation = Random.rotation;
                   bond.transform.localScale = Vector3.one;
                   GameObject molecularObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                   molecularObject.transform.localScale = new Vector3(atom.size*0.01f, atom.size * 0.01f, atom.size);
                   molecularObject.GetComponent<MeshRenderer>().material.shader = Shader.Find("Shader Forge/Force field");
                   molecularObject.transform.parent = bond.transform;
                   molecularObject.AddComponent<Rigidbody>().isKinematic = true;
                   molecularObject.transform.position = bond.transform.position+ 0.5f*bond.transform.forward* atom.size ;
                   molecularObject.transform.forward = bond.transform.forward;*/
                //(bond.transform.position- molecularObject.transform.position).normalized;// new Vector3(0.0f, atom.size, 0.0f);
            }
        }
        else
        {
            Debug.LogWarning("该原子并不包含该化合价:" + defaultValence);
        }
    }

    void OnDestroy()
    {
     /*   if (transform.FindChild("IntermolecularPivot"))
        {
            Destroy(transform.FindChild("IntermolecularPivot").gameObject);
        }*/
    }

    public string DirTargetID
    {
        get
        {
            return name + "_"+GetInstanceID();
        }
    }

    public Bond GetDirectionBond(string key)
    {
        if (HasKey(key))
        {
            return BondDirTargets[key].GetComponent<Bond>() ;
        }
        else
        {
            return null;
        }
    }

    public void LoadDirectionBond(string key,Transform trans)
    {
        if (HasKey(key))
        {
            UnLoadDirectionBond(key);
        }
       // BondDirTargets.Clear();
        BondDirTargets.Add(key, trans);
    }
    public bool HasKey(string key)
    {
        return BondDirTargets.ContainsKey(key);
    }
    public void UnLoadDirectionBond(string key)
    {
        BondDirTargets.Remove(key);
    }
    Dictionary<string, Transform> bondDirTargets;
    public Dictionary<string, Transform> BondDirTargets
    {
        get
        {
            if (bondDirTargets == null)
            {
                bondDirTargets = new Dictionary<string, Transform>();
            }
            return bondDirTargets;
        }
    }


    [ContextMenu("Connections")]
    List<Bond> TestConections()
    {
        /*   foreach(Bond bond in Connections)
           {
               AtomController atom = bond.GetAnotherAtom(this.Atom);
               Debug.LogWarning(atom.AtomID);
   //            atom.Intermo;
           }*/
        Dictionary<string, AtomController> atoms = new Dictionary<string, AtomController>();
        GetConnections(this,ref atoms);
        return Connections;
    }
    public List<Bond> Connections
    {
        get
        {
            List<Bond> connections = new List<Bond>();
            foreach(Bond bond in GetComponentsInChildren<Bond>())
            {
                if (bond.NoticeDestroy)
                {
                    continue;
                }
                if (!bond.isConnectBond || ((int)bond.bondStatus)<4)
                {
                    if(bond.parentBond != null)
                    {
                        if (!connections.Contains(bond.parentBond))
                        {
                            connections.Add(bond.parentBond);
                        }
                    }
                    continue;
                }
               // Debug.Log(bond.name+"_"+bond.GetInstanceID());
                connections.Add(bond);
            }
            return connections;
        }
    }
}