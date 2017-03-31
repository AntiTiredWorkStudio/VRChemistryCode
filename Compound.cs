using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Compound : MonoBehaviour {
    public static Compound CreateCompound(Transform pivot)
    {
        GameObject compound = new GameObject("TargetCompound");
        compound.transform.position = pivot.transform.position;
        compound.transform.localScale = pivot.transform.lossyScale;
        Compound result = compound.AddComponent<Compound>();
        result.AddAtom(pivot.GetComponent<AtomController>());
        result.RangeX = new RangeAttribute(pivot.transform.position.x, pivot.transform.position.x);
        result.RangeY = new RangeAttribute(pivot.transform.position.y, pivot.transform.position.y);
        result.RangeZ = new RangeAttribute(pivot.transform.position.z, pivot.transform.position.z);
        result.Splashpoint = DragCommand.CreateDragPannel(pivot.transform.position,result.transform);
        result.Splashpoint.FollowedObjectTransform = result.transform;
        result.Splashpoint.StartDrag.Add(result.StartDrag);
        result.Splashpoint.FinishedDrag.Add(result.EndDrag);
        return result;
    }

    public static Compound AddAtomsToCompound(List<AtomController> targetAtoms,Compound comd)
    {
        foreach (AtomController atom in targetAtoms)
        {
            atom.CompoundPivot.RemoveAtom(atom);
            atom.CompoundPivot = null;
        }
        comd.AddAtoms(targetAtoms);
        return comd;
    }

    public static Compound CompoundAtoms(List<AtomController> targetAtoms)
    {
        Compound compound = CreateCompound(targetAtoms[0].transform);
        compound.AddAtoms(targetAtoms);
        return compound;
    }

    public static Compound CompoundAtoms(Bond bondA , Bond bondB)
    {
        AtomController atomA = bondA.RootAtom;
        AtomController atomB = bondB.RootAtom;
        Compound TargetCompound = null;
        
        if (atomA.compoundPivot == null && atomB.compoundPivot == null)
        {
            Debug.Log("创建新的化合物根");
            TargetCompound = CreateCompound(atomA.transform);
            TargetCompound.name = atomA.text3DName.text + atomB.text3DName.text;
            atomA.CompoundPivot = TargetCompound;
            atomB.CompoundPivot = TargetCompound;
            if(!TargetCompound.HasAtom(atomA))
                TargetCompound.AddAtom(atomA);
            if (!TargetCompound.HasAtom(atomB))
                TargetCompound.AddAtom(atomB);
        }
        else
        {
            if (atomA.compoundPivot != null && atomB.compoundPivot == null)
            {
                Debug.Log("atomA不用创建");
                TargetCompound = atomA.compoundPivot;
               // TargetCompound.name += atomB.text3DName.text;
                atomB.CompoundPivot = TargetCompound;
                if (!TargetCompound.HasAtom(atomB))
                    TargetCompound.AddAtom(atomB);
            }

            if (atomA.compoundPivot == null && atomB.compoundPivot != null)
            {
                Debug.Log("atomB不用创建");
                TargetCompound = atomB.compoundPivot;
                //TargetCompound.name += atomA.text3DName.text;
                atomA.CompoundPivot = TargetCompound;
                if (!TargetCompound.HasAtom(atomA))
                    TargetCompound.AddAtom(atomA);
            }

            if(atomA.compoundPivot != null && atomB.compoundPivot != null)
            {
                if(atomA.compoundPivot == atomB.compoundPivot)
                {
                    TargetCompound = atomA.compoundPivot;
                    if (!TargetCompound.HasAtom(atomA))
                        TargetCompound.AddAtom(atomA);
                }
                else
                {
                    Dictionary<string, AtomController> atoms = new Dictionary<string, AtomController>();
                   // atoms.Add(atomB.AtomID, atomB);
                    Intermolecular.GetConnections(atomB.Intermo,ref atoms);
                    List<AtomController> atomsList = new List<AtomController>(atoms.Values);
                    TargetCompound = Compound.AddAtomsToCompound(atomsList,atomA.CompoundPivot);

                    //TargetCompound = atomA.compoundPivot;
                    //特殊情况需要单独考虑
                }
            }
        }
        Debug.LogWarning("化合物:"+TargetCompound.name);
        return TargetCompound;
    }

    public RangeAttribute RangeX = new RangeAttribute(0.0f,0.0f);
    public RangeAttribute RangeY = new RangeAttribute(0.0f, 0.0f);
    public RangeAttribute RangeZ = new RangeAttribute(0.0f, 0.0f);
    void OnDestroy()
    {
        Destroy(Splashpoint.gameObject);
    }
    public Vector3 Core()
    {
        return new Vector3(
            RangeCenter(RangeX),
            RangeCenter(RangeY),
            RangeCenter(RangeZ)
            );
    }

    public static float RangeCenter(RangeAttribute targetAttr)
    {
        return targetAttr.min + (targetAttr.max - targetAttr.min) / 2.0f;
    }
    public static float RangeLength(RangeAttribute targetAttr)
    {
        return (targetAttr.max - targetAttr.min);
    }
    public static RangeAttribute FixRangeAttribute(RangeAttribute targetAttr,float val)
    {
        if (val < targetAttr.min)
        {
            return new RangeAttribute(val,targetAttr.max);
        }
        if (val > targetAttr.max)
        {
            return new RangeAttribute( targetAttr.min,val);
        }
        return
            targetAttr;
    }

    public static void Disconnect(AtomController atom)
    {
/*        if(comp == null)
        {
            return;
        }*/
       Debug.Log(atom.name);
        Intermolecular intermo = atom.GetComponent<Intermolecular>();
        //拆单键原子
        if (intermo.ConnectedBonds.Count == 0)
        {
            // Debug.Log(comp);
            //int startIndex = atom.CompoundPivot.gameObject.name.IndexOf(atom.text3DName.text);
            // atom.CompoundPivot.name = atom.CompoundPivot.gameObject.name.Remove(startIndex, atom.text3DName.text.Length);
            atom.CompoundPivot.RemoveAtom(atom);
            atom.CompoundPivot = null;
        }
        else
        {
            //共价键的时候要改变这个条件
            if (atom.Intermo.mainValence < 0)
            {
                Dictionary<string, AtomController> dictionary = new Dictionary<string, AtomController>();
                Intermolecular.GetConnections(intermo, ref dictionary);
                if (dictionary.Count < atom.CompoundPivot.AtomHash.Count)//环状结构的非常重要的判断
                {
                    List<AtomController> NewCompound = new List<AtomController>();//新化合物的组合
                    NewCompound.Add(atom);
                    foreach (string key in dictionary.Keys)
                    {
                        atom.CompoundPivot.RemoveAtom(dictionary[key]);
                        //***************暂时这么写！一会儿必须改过来！**************
                        if (dictionary[key] != atom)
                        {
                            NewCompound.Add(dictionary[key]);
                            dictionary[key].CompoundPivot = null;
                        }
                        //****************暂时这么写！一会儿必须改过来！**************
                    }
                    atom.CompoundPivot = null;
                    CompoundAtoms(NewCompound);
                }
                else
                {
                    Debug.LogWarning("拆环状结构");
                }
            }
        }
       // atom.transform.SetParent(null, true);
        //从CompoundPivot列表中把化学键删除
    }


    public AtomController[] AtomSubtance(AtomController targetAtoms)
    {
        Dictionary<string, AtomController> results = new Dictionary<string, AtomController>();
        foreach (Bond bond in targetAtoms.Intermo.Connections)
        {

        }
        return null;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Vector3 p0 = new Vector3(RangeX.min, RangeY.max, RangeZ.min);
        Vector3 p1 = new Vector3(RangeX.min, RangeY.max, RangeZ.max);
        Vector3 p2 = new Vector3(RangeX.max, RangeY.max, RangeZ.max);
        Vector3 p3 = new Vector3(RangeX.max, RangeY.max, RangeZ.min);



        Vector3 mp0 = new Vector3(RangeX.min, RangeY.min, RangeZ.min);
        Vector3 mp1 = new Vector3(RangeX.min, RangeY.min, RangeZ.max);
        Vector3 mp2 = new Vector3(RangeX.max, RangeY.min, RangeZ.max);
        Vector3 mp3 = new Vector3(RangeX.max, RangeY.min, RangeZ.min);


        Gizmos.DrawLine(
            new Vector3(RangeX.min, RangeY.min, RangeZ.min), new Vector3(RangeX.max, RangeY.max, RangeZ.max)
            );
        Gizmos.DrawLine(
            new Vector3(RangeX.min, RangeY.min, RangeZ.max), new Vector3(RangeX.max, RangeY.max, RangeZ.min)
            );


        Gizmos.color = Color.blue;
        Gizmos.DrawLine(
            p0,p1
            );
        Gizmos.DrawLine(
            p0, p3
            );
        Gizmos.DrawLine(
            p2, p1
            );
        Gizmos.DrawLine(
            p2, p3
            );

        Gizmos.DrawLine(
           mp0, p0
            );
        Gizmos.DrawLine(
            mp1, p1
            );
        Gizmos.DrawLine(
            mp2, p2
            );
        Gizmos.DrawLine(
            mp3, p3
            );

        Gizmos.DrawLine(
           mp0, mp1
            );
        Gizmos.DrawLine(
            mp0, mp3
            );
        Gizmos.DrawLine(
            mp2, mp1
            );
        Gizmos.DrawLine(
            mp2, mp3
            );
        /*Gizmos.DrawLine(
             new Vector3(RangeX.max, RangeY.min, RangeZ.min), new Vector3(RangeX.min, RangeY.max, RangeZ.max)
             );*/
    }
    public DragCommand Splashpoint;
    public Dictionary<string,AtomController> AtomHash
    {
        get
        {
            if(atomHash == null)
            {
                atomHash = new Dictionary<string, AtomController>();
            }

            return atomHash;
        }
    }
    Dictionary<string, AtomController> atomHash = null;

    public void AddAtoms(List<AtomController> atoms)
    {
        foreach (AtomController atom in atoms)
        {
            atom.CompoundPivot = this;
            AtomHash.Add(atom.AtomID, atom);
            atom.transform.SetParent(transform, true);
            atom.SetPos(atom.transform.position);
           // RefreashSizeBounds(atom);
        }
        RefreashName();
    }

    public void AddAtom(AtomController atom)
    {
        Debug.LogWarning(atom.AtomID);
        AtomHash.Add(atom.AtomID, atom);
        atom.transform.SetParent(transform,true);
        atom.SetPos(atom.transform.position);

        //RefreashSizeBounds(atom);

        RefreashName();
    }

    void Update()
    {
        RangeAttribute rangeX = new RangeAttribute(transform.position.x, transform.position.x);
        RangeAttribute rangeY = new RangeAttribute(transform.position.y, transform.position.y);
        RangeAttribute rangeZ = new RangeAttribute(transform.position.z, transform.position.z);
        foreach (string key in AtomHash.Keys)
        {
            if(AtomHash[key] == null)
            {
                continue;
            }
            RefreashSizeBounds(AtomHash[key], ref rangeX, ref rangeY, ref rangeZ);
        }

        RangeX = rangeX;
        RangeY = rangeY;
        RangeZ= rangeZ;

        RefreashDragPannel();
        Splashpoint.transform.localScale = Vector3.one * 0.009f; 
    }

    bool OnCompoundDrag = false;

    public void StartDrag()
    {
        OnCompoundDrag = true;
    }

    public void EndDrag()
    {
        OnCompoundDrag = false;
    }

    void RefreashDragPannel()
    {
        if (OnCompoundDrag)
        {
            return;
        }
        Vector3 core = Core();
        core.y -= 1.08f*RangeLength(RangeY);
        Splashpoint.InitDragPannelPositionAndRoot(core);
    }

    void RefreashSizeBounds(AtomController atom , ref RangeAttribute rx,ref RangeAttribute ry ,ref RangeAttribute rz)
    {
        rx = FixRangeAttribute(rx, atom.transform.position.x);
        ry = FixRangeAttribute(ry, atom.transform.position.y);
        rz = FixRangeAttribute(rz, atom.transform.position.z);

        /*GetComponent<BoxCollider>().center = new Vector3(
            RangeCenter(RangeX), RangeCenter(RangeY), RangeCenter(RangeZ)
            );
        GetComponent<BoxCollider>().size = (new Vector3(
            RangeLength(RangeX), RangeLength(RangeY), RangeLength(RangeZ)
            )) * -100.0f;*/
    }

    public bool HasAtom(AtomController atom)
    {
        return AtomHash.ContainsKey(atom.AtomID);
    }

    public void RemoveAtom(AtomController atom)
    {
        if(atom.CompoundPivot != null)
        {
            atom.CompoundPivot = null;
        }
        AtomHash.Remove(atom.AtomID);
        atom.transform.SetParent(null, true);
        atom.SetPos(atom.transform.position);
        RefreashName();
    }

    void RefreashName(bool checkDestroy = true)
    {
        string targetName = "";
        foreach (string key in AtomHash.Keys)
        {
            targetName+=AtomHash[key].text3DName.text;
            if(AtomHash[key].transform.parent != transform)
            {
                AtomHash[key].transform.SetParent(transform,true);
            }
        }
        name = targetName;
        if (checkDestroy)
        {
            if(name == "")
            {
                Destroy(gameObject);
            }
        }
    }



}
