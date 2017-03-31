using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BondCombine : MonoBehaviour {
    Intermolecular intermolecular;
    void Start()
    {
        intermolecular = transform.parent.GetComponent<Intermolecular>();
        
    }

    public Intermolecular TargetIntermolecular;
    public Bond attractBond;


    void OnTriggerExit(Collider other)
    {
        if (other.name != "IntermolecularPivot" && other.gameObject.GetComponent<Intermolecular>() != null)
        {
            Intermolecular OtherInter = other.gameObject.GetComponent<Intermolecular>();
            Debug.LogWarning(other.name + "离开"+intermolecular.name+"的引力场");
            if (intermolecular.HasKey(OtherInter.DirTargetID))
            {
                
                Bond bond = (OtherInter.BondDirTargets.ContainsKey(intermolecular.DirTargetID))?(OtherInter.BondDirTargets[intermolecular.DirTargetID].GetComponent<Bond>()):null;

                intermolecular.BondDirTargets[OtherInter.DirTargetID].GetComponent<Bond>().UnLoadBondTarget();
                intermolecular.UnLoadDirectionBond(OtherInter.DirTargetID);
                
                if (bond != null)
                {
                    List<Transform> targets = new List<Transform>(intermolecular.BondDirTargets.Values);
                    if (targets.Count > 0)
                    {
                        bond.LoadBondTarget(targets[0]);
                    }
                }
                GetComponent<Collider>().enabled = false;
                GetComponent<Collider>().enabled = true;
                //                attractBond.UnLoadBondTarget();
            }

            attractBond = null;
            TargetIntermolecular = null;
            
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!intermolecular.HasBreakBond)
        {
            Debug.LogWarning(intermolecular.name+" 自己没有空闲化学键，无法连接");
            return;
        }else
        if (other.name != "IntermolecularPivot" && other.gameObject.GetComponent<Intermolecular>()!=null)
        {
            Debug.LogWarning(intermolecular.name + ":IntermolecularPivot");
            Intermolecular OtherInter = other.gameObject.GetComponent<Intermolecular>();

            if (OtherInter.HasBreakBond)
            {
                float otherValence = OtherInter.mainValence;
                float thisValecne = intermolecular.mainValence;
                float otherValenceAbs = Mathf.Abs(otherValence);
                float thisValenceAbs = Mathf.Abs(thisValecne);
                if ((otherValence / otherValenceAbs) != (thisValecne / thisValenceAbs))
                {
                    if (OtherInter.ConnectedAtom(intermolecular.Atom))
                    {
                        Debug.LogWarning("已经连接");
                    }
                    else
                    {
                        Bond bondGet = intermolecular.GetNearestBreakBond(OtherInter.Atom);
                        //Bond bondGet = OtherInter.GetNearestBreakBond(intermolecular.Atom);
                        attractBond = bondGet;
                        TargetIntermolecular = OtherInter;
                        if (attractBond != null)
                        {
                            intermolecular.LoadDirectionBond(OtherInter.DirTargetID, attractBond.transform);
                            attractBond.LoadBondTarget(TargetIntermolecular.transform);
                        }
                     //   intermolecular.LoadDirectionBond(OtherInter.DirTargetID, attractBond.transform);
                        Debug.LogWarning("准备吸引:" + OtherInter.Atom.name );
                    }
                }
                else
                {
                    Debug.LogWarning(OtherInter.name + "化合价:" + otherValence + "    " + intermolecular.name + "化合价:" + thisValecne + "化合价同号，不能连接");
                }
            }else
            {

                Debug.LogWarning(OtherInter.name + "没有空闲化学键，无法连接");
            }
            //            Inter.Atom.va
        }
    }
}
