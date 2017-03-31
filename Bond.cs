using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bond : MonoBehaviour {

    public static Bond BondConnect(Bond bondA, Bond bondB)
    {
        Intermolecular interA = bondA.GetComponentInParent<Intermolecular>();
        Intermolecular interB = bondB.GetComponentInParent<Intermolecular>();
        if(interA == null || interB == null)
        {
            Debug.LogWarning("其中一个化学键为空");
            return null;
        }
        Intermolecular root = (interA.ConnectedBonds.Count >= interB.ConnectedBonds.Count) ?interA:interB;
        Intermolecular other = (interA.ConnectedBonds.Count >= interB.ConnectedBonds.Count) ? interB : interA;
        Bond rootBond = (interA.ConnectedBonds.Count >= interB.ConnectedBonds.Count) ? bondA : bondB;
        Bond otherBond = (interA.ConnectedBonds.Count >= interB.ConnectedBonds.Count) ? bondB :bondA;

        Bond connectBond = CreateBond(root.Atom, PlusMinus.COMBINE);

        connectBond.paramaterAtoms.Clear();
        connectBond.paramaterAtoms.Add(root.Atom);
        connectBond.paramaterAtoms.Add(other.Atom);
//        connectBond

        connectBond.paramaterBonds.Clear();
        connectBond.paramaterBonds.Add(rootBond);
        connectBond.paramaterBonds.Add(otherBond);

        rootBond.parentBond = connectBond;
        otherBond.parentBond = connectBond;

        //Compound compound = null;
        Debug.LogWarning(bondA.name + " 和 " + bondB.name + " 相连");
        Compound.CompoundAtoms(bondA, bondB);
    /*    if(compound == null && bondA.RootAtom.CompoundPivot != null )
        {
            Debug.LogWarning(bondA.name + ":有轴点");
            compound = bondA.RootAtom.CompoundPivot;
            if(bondB.RootAtom.CompoundPivot == null)
            {
                Debug.LogWarning(bondB.RootAtom.name + ":没有轴点");
                Vector3 posB = bondB.RootAtom.transform.position;
                bondB.RootAtom.CompoundPivot = compound;
                bondB.RootAtom.CompoundPivot = compound;
                      bondB.RootAtom.transform.SetParent(compound.transform, true);
                       bondB.RootAtom.transform.position = posB;
            }
        }
        else if (compound == null && bondB.RootAtom.CompoundPivot != null)
        {
            Debug.LogWarning(bondB.name + ":有轴点");
            compound = bondA.RootAtom.CompoundPivot;
            Vector3 posA = bondA.RootAtom.transform.position;
            if (bondA.RootAtom.CompoundPivot == null)
            {
                Debug.LogWarning(bondA.RootAtom.name + ":没有轴点");
                bondA.RootAtom.CompoundPivot = compound;
                bondA.RootAtom.transform.SetParent(compound.transform, true);
                bondA.RootAtom.transform.position = posA;
            }
        }
        if(compound == null)
        {
            Debug.LogWarning("创建新的化合物轴点");
            compound = Compound.CreateCompound(bondA.RootAtom.transform);
            Vector3 posA = bondA.RootAtom.transform.position;
            Vector3 posB = bondB.RootAtom.transform.position;
            bondA.RootAtom.CompoundPivot = compound;
              bondA.RootAtom.transform.SetParent(compound.transform, true);
           bondA.RootAtom.transform.position = posA;
            bondB.RootAtom.CompoundPivot = compound;
           bondB.RootAtom.transform.SetParent(compound.transform, true);
            bondB.RootAtom.transform.position = posB;
        }*/


        return connectBond;
    }
    static Object EmptyBond
    {
        get
        {
            return Resources.Load("Prefabs/EmptyBond");
        }
    }
    public static Bond CreateBond(AtomController target, PlusMinus plus)
    {
        Bond targetBond = (Instantiate(EmptyBond) as GameObject).GetComponent<Bond>();

        targetBond.transform.position = target.transform.position;

        targetBond.paramaterAtoms.Add(target);

//        targetBond.bondStatus = BondState.BREAKTRIBLE;

        targetBond.plusMinus = plus;

        return targetBond;
    }


    public const float maxmizeBreakMultiple = 1.4f;

    public static Color32 PositiveColor
    {
        get
        {
            return new Color32((byte)255,(byte)60, (byte)0, (byte)255);
        }
    }
    public static Color32 NagativeColor
    {
        get
        {
            return new Color32((byte)0, (byte)32, (byte)144, (byte)255);
        }
    }
    public enum BondState
    {
        BREAK = 1,
        BREAKDOUBLE = 2,
        BREAKTRIBLE = 3,
        SINGLE = 4,
        DOUBLE = 5,
        TRIBLE = 6
    }
    public enum PlusMinus
    {
        NAGATIVE = -1,
        POSITIVE = 1,
        COMBINE = 2
    }
    public bool isConnectBond
    {
        get
        {
            return paramaterBonds.Count >= 2 && paramaterBonds.Count >= 2;
        }
    }

    public List<AtomController> paramaterAtoms;
    public List<Bond> paramaterBonds;
    public Bond ParentBond
    {
        get
        {
            return parentBond;
        }
        set
        {
            if (value != null)
            {
               // paramaterBonds[0].enabled = false;
              //  paramaterBonds[1].enabled = false;
                bondLine.enabled = false;
            }
            else
            {
                bondLine.enabled = true;
            }
            parentBond = value;
        }
    }
    public Bond parentBond = null;

    /// <summary>
    /// 计算该键占据的总化合价
    /// </summary>
    public int RealValence
    {
         get {
            return ((int)plusMinus * (int)bondStatus);
        }
    }
    [ContextMenu("Show RealValence")]
    void ShowRealValence()
    {
        Debug.LogWarning(RealValence);
    }

    [ContextMenu("Is Child Is False")]
    void OnChildFalse()
    {
        ParentBond = null;
    }
    public bool IsChild
    {
        get
        {
            return ParentBond != null;
        }
    }

    public void OnIsChild(Bond ParentBond)
    {
        switch (ParentBond.paramaterBonds.IndexOf(this)) {
            case 0:
                RootNode.position = ParentBond.RootNode.position;
                HeadNode.position = RootNode.position -  ParentBond.NodeDirection* ParentBond.NodeDistance * 0.5f;
                RootNode.GetComponent<ParticleSystem>().Stop();
                HeadNode.GetComponent<ParticleSystem>().Stop();
                break;
            case 1:
                RootNode.position = ParentBond.HeadNode.position;
                HeadNode.position = RootNode.position + ParentBond.NodeDirection * ParentBond.NodeDistance * 0.5f;
                RootNode.GetComponent<ParticleSystem>().Stop();
                HeadNode.GetComponent<ParticleSystem>().Stop();
                break;
            default:
                break;
        }
    }

    public BondState bondStatus = BondState.BREAK;
    public PlusMinus plusMinus = PlusMinus.POSITIVE;

    /// <summary>
    /// 针对连接化学键，通过其中一个bond，返回另一个bond
    /// </summary>
    /// <param name="bond"></param>
    /// <returns></returns>
    public Bond GetAnotherBond(Bond bond)
    {
        if (((int)bondStatus) > 3)
        {
            return (bond == paramaterBonds[0]) ? paramaterBonds[1] : paramaterBonds[0];
        }
        else
        {
            throw new System.Exception("化学键未连接，无法获取另一端的子键");
            return null;
        }
    }

    /// <summary>
    /// 针对连接化学键，通过其中一个atom，返回另一个atom
    /// </summary>
    /// <param name="atom"></param>
    /// <returns></returns>
    public AtomController GetAnotherAtom(AtomController atom)
    {
        if (((int)bondStatus) > 3)
        {
            return (atom == paramaterAtoms[0]) ? paramaterAtoms[1] : paramaterAtoms[0];
        }
        else
        {
           // throw new System.Exception(name+":化学键未连接，无法获取另一端的子键");
            return null;
        }
    }

    /// <summary>
    /// 获取断裂状态下的化学键的根原子
    /// </summary>
    public AtomController RootAtom
    {
        get
        {
            if (!isConnectBond)
            {
                if (paramaterAtoms != null && paramaterAtoms.Count > 0)
                {
                    return paramaterAtoms[0];
                }
                else
                    return null;
            }
            else
            {
                Debug.LogWarning("Parent:"+ParentBond);
                throw new System.Exception("化学键为连接键，不属于任何一个原子");
                return null;
            }
        }
    }

  // public Bond 
    void UpdateNormal()
    {
        for(int i = 0; i < paramaterAtoms.Count; i++)
        {
            if(paramaterAtoms[i] != null)
            {
                switch (i)
                {
                    case 0:
                       Vector3 pos = transform.position;
                        transform.parent = paramaterAtoms[i].transform;
                        transform.position = pos;
                        break;
                    case 1:
                     //   Vector3 ppos = paramaterAtoms[i].transform.position;
                  //      paramaterAtoms[i].transform.parent = transform;
                      //  transform.position = ppos;
                        break;
                    default:
                        break;
                }
            }
        }
         if (paramaterAtoms.Count == 0)
         {
            transform.parent = null;
            paramaterBonds.Clear();
         }
        if (paramaterAtoms.Count > 0)
        {
            if (paramaterAtoms[0] != null)
            {
                name = ((IsChild) ? ("[" + parentBond.name + "]::" + "{" + paramaterAtoms[0].name + "}") : ("{" + paramaterAtoms[0].name + "}")) + "_" + plusMinus.ToString() + "_" + bondStatus.ToString();
            }
        }
        else
        {
            name = "EmptyBond";
            bondLine.SetVertexCount(0);
        }
    }
    public bool NoticeDestroy = false;
    void OnDestroy()
    {
        if (!NoticeDestroy)
        {
            return;
        }
        foreach (AtomController atom in GetComponentsInChildren<AtomController>())
        {
            if(atom.transform.parent != transform)
            {
                continue;
            }
            Vector3 hp = atom.transform.position;
        //    Debug.LogWarning(hp);
          
            atom.GetComponent<DragCommand>().enabled = false;
            //atom.transform.parent = null;
            atom.transform.SetParent(null);
            atom.transform.position = hp;
            //这里需要测试到底家还是不加这个判断
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                atom.SetPos(hp);
            }
        }
    }

    void Update()
    {
        UpdateNormal();
        if (IsChild)
        {
            return;
        }
        switch (bondStatus)
        {
            case BondState.BREAKDOUBLE:
                BondBreakDoubleUpdate();
                break;
            case BondState.BREAKTRIBLE:
                BondBreakTribleUpdate();
                break;
            case BondState.BREAK:
                BondBreakUpdate();
                break;
            case BondState.DOUBLE:
                BondDoubleUpdate();
                break;
            case BondState.SINGLE:
                BondSingleUpdate();
                break;
            case BondState.TRIBLE:
                BondTribleUpdate();
                break;
            default:
                break;
        }
    }

    bool UseRandomUnitSphere = false;

    public Vector3 UnitSphere
    {
        get
        {
            if(unitSphere == Vector3.zero)
            {
                unitSphere = Random.onUnitSphere;
            }
            return unitSphere;
        }

        set
        {
            unitSphere = value;
        }
    }
    Vector3 unitSphere = Vector3.zero;
    Color32 GetColorFromPlusMinus(Bond plusM)
    {
        //if (plusMinus == PlusMinus.COMBINE)
        //{
        //    Debug.LogWarning(plusM);
        //}
        if (plusM == null)
        {
            return Color.red;
        }
        switch (plusM.plusMinus)
        {
            case PlusMinus.POSITIVE:
                return Bond.PositiveColor;
            case PlusMinus.NAGATIVE:
                return Bond.NagativeColor;
            default:
                return Color.red;
        }
    }
    void ColorTransform()
    {
        switch (plusMinus)
        {
            case PlusMinus.POSITIVE:
                if (paramaterBonds.Count >= 2)
                {
                    plusMinus = PlusMinus.COMBINE;
                    break;
                }
                bondLine.SetColors(Bond.PositiveColor, Bond.PositiveColor);
                break;
            case PlusMinus.NAGATIVE:
                if (paramaterBonds.Count >= 2)
                {
                    plusMinus = PlusMinus.COMBINE;
                    break;
                }
                bondLine.SetColors(Bond.NagativeColor, Bond.NagativeColor);
                break;
            case PlusMinus.COMBINE:
                if (paramaterBonds.Count<2)
                {
                    plusMinus = PlusMinus.POSITIVE;
                    break;
                }
                bondLine.SetColors(GetColorFromPlusMinus(paramaterBonds[0]),
                    GetColorFromPlusMinus(paramaterBonds[1]));
           /*     if(plusMinus == PlusMinus.COMBINE)
                {
                    Debug.LogWarning(paramaterBonds[0].plusMinus + "," + paramaterBonds[1].plusMinus);
                }*/
                break;
            default:
                break;
        }
    }


    void OnDisconnect()
    {
        Intermolecular intermole = RootAtom.GetComponent<Intermolecular>();
        intermole.ConnectedBonds.Remove(this);//ParentBond.GetAnotherBond(this));
        intermole.FreeBonds.Add(this);
      //  Debug.LogWarning(intermole.Atom.name);
      //  Debug.LogWarning(intermole.Atom.CompoundPivot.name);
        Compound.Disconnect(intermole.Atom);
        ParentBond = null;
        //  bondLine.enabled = true;
        // Debug.LogWarning("On Disconnect:" + name);
        RootNode.gameObject.SetActive(true);
        HeadNode.gameObject.SetActive(true);
        RootNode.GetComponent<ParticleSystem>().Play();
        HeadNode.GetComponent<ParticleSystem>().Play();
     /*  foreach (ParticleSystem ps in GetComponentsInChildren<ParticleSystem>())
        {
            if(ps.transform.parent = transform)
            {
                if((ps.transform == RootNode) || (ps.transform == HeadNode))
                {
                    ps.Play();
                }
            }
        }*/
    }

    void OnConnect()
    {
        bondLine.enabled = false;
        //Debug.LogWarning("On Connect:"+name);
        RootNode.gameObject.SetActive(false);
        HeadNode.gameObject.SetActive(false);
        RootNode.GetComponent<ParticleSystem>().Stop();
        HeadNode.GetComponent<ParticleSystem>().Stop();
        Intermolecular intermo = RootAtom.GetComponent<Intermolecular>();
        if (intermo.FreeBonds.Contains(this))
        {
            intermo.FreeBonds.Remove(this);
        }
        if (!intermo.ConnectedBonds.Contains(this))
        {
            intermo.ConnectedBonds.Add(this);
        }
        /*foreach (ParticleSystem ps in GetComponentsInChildren<ParticleSystem>())
        {
            if (ps.transform.parent = transform)
            {
                if ((ps.transform == RootNode) || (ps.transform == HeadNode))
                {
                    ps.Stop();
                }
            }
        }*/
    }

    /// <summary>
    /// 化学键的朝向目标
    /// </summary>
    public Intermolecular BondTarget
    {
        get
        {
            return BondDirTarget.GetComponent<Intermolecular>();
        }
    }
    /// <summary>
    /// 化学键的朝向目标
    /// </summary>
    Transform BondDirTarget = null;
    /// <summary>
    ///加载化学键的朝向目标
    /// </summary>
    /// <param name="target"></param>
    public void LoadBondTarget(Transform target)
    {
        BondDirTarget = target;
    //    BondDirTargets.Add(target.name, target);
    }
    /// <summary>
    /// 卸载化学键的朝向目标目标
    /// </summary>
    public void UnLoadBondTarget()
    {
     /*   if (BondDirTargets.ContainsKey(target.name))
        {
            BondDirTargets.Remove(target.name);
        }*/
        BondDirTarget = null;
    }


    void BondBreakUpdate()
    {
       // Debug.Log("BondBreak:" + name);
        if (paramaterAtoms.Count >= 2 && paramaterBonds.Count >= 2)
        {
            plusMinus = PlusMinus.COMBINE;
            bondStatus = BondState.SINGLE;
            return;
        }
        if (paramaterAtoms.Count > 0 && paramaterAtoms[0] != null)
        {
            ColorTransform();
            AtomController atom = paramaterAtoms[0];
            transform.position = paramaterAtoms[0].transform.position;
            bondLine.SetVertexCount(2);

            Vector3 randSphere = UnitSphere;
            Vector3 currentDir = (HeadNode.transform.position - RootNode.transform.position).normalized;
            Vector3 dir = Vector3.one;
            if (BondDirTarget == null)
            {
                if (!UseRandomUnitSphere)
                {
                    UseRandomUnitSphere = true;
                    dir =Vector3.Lerp(currentDir,(randSphere - transform.position).normalized,Time.deltaTime*5.0f);
                }
                else
                {
                    dir = Vector3.Lerp(currentDir, randSphere, Time.deltaTime * 5.0f);
                }
            }
            else
            {
                //和别的化学键相互吸引
                dir = Vector3.Lerp(currentDir, (BondDirTarget.position - transform.position).normalized,Time.deltaTime);
                //         Debug.LogWarning(BondDirTarget.name);
                Intermolecular TargetIntermo = BondDirTarget.gameObject.GetComponent<Intermolecular>();
                Intermolecular OwnIntermo = RootAtom.GetComponent<Intermolecular>();
                Bond targetBond = null;//RootAtom.GetComponent<Intermolecular>().GetDirectionBond(BondDirTarget.GetComponent<Intermolecular>().DirTargetID);
                if(TargetIntermo!=null)
                if (!TargetIntermo.HasKey(OwnIntermo.DirTargetID))
                {
//                    Debug.LogWarning("其中一个原子化学键还未进入范围");
                }
                else
                {
                    targetBond = TargetIntermo.GetDirectionBond(RootAtom.GetComponent<Intermolecular>().DirTargetID);
                    if (this.plusMinus == PlusMinus.POSITIVE && !targetBond.IsChild)
                    {
                        float distance = Vector3.Distance(targetBond.HeadNode.position, HeadNode.position);
                        if (distance < 0.00101f)
                        {
                            Debug.LogWarning(targetBond.name + "  & " + name + " connected!");

                            Bond.BondConnect(this, targetBond);
                        }
                        else
                        {
                       //     Debug.LogWarning(distance);
                        }
                    }
                }
            }

                 Vector3 rootPos = Vector3.zero;
                  Vector3 headPos = Vector3.zero;
                 if (IsChild)
                 { 
                      RootNode.transform.localPosition = rootPos;
                      HeadNode.transform.localPosition = headPos;
                  }
                  else
                  {
                      rootPos = transform.position + dir * atom.size / 2.0f;
                      headPos = transform.position + dir * 2f * atom.size / 2.0f;
                      RootNode.transform.position = rootPos;
                      HeadNode.transform.position = headPos;
                  }

                 bondLine.SetVertexCount(2);
                  bondLine.SetPosition(0, rootPos);
                  bondLine.SetPosition(1, headPos);
        }
        bondLine.material.mainTextureScale = Vector2.one;
    }
    void BondSingleUpdate()
    {
        if (paramaterAtoms.Count >= 2 && paramaterBonds.Count >= 2 &&
            paramaterAtoms[0] != null &&
            paramaterAtoms[1] != null &&
            paramaterBonds[0] != null && 
            paramaterBonds[1] != null &&
            paramaterBonds[0].paramaterAtoms[0] == paramaterAtoms[0] &&
            paramaterBonds[1].paramaterAtoms[0] == paramaterAtoms[1] &&
            paramaterBonds[0].plusMinus != paramaterBonds[1].plusMinus
            )
        {
            if (!paramaterBonds[0].IsChild)
            {
                paramaterBonds[0].ParentBond = this;
                paramaterBonds[0].OnIsChild(this);
            }

            if (!paramaterBonds[1].IsChild)
            {
                paramaterBonds[1].ParentBond = this;
                paramaterBonds[1].OnIsChild(this);
            }

            if(paramaterBonds[0].bondStatus == BondState.BREAKDOUBLE &&
                paramaterBonds[1].bondStatus == BondState.BREAKDOUBLE)
            {
                bondStatus = BondState.DOUBLE;
            }else 
            if (paramaterBonds[0].bondStatus == BondState.BREAKTRIBLE &&
                paramaterBonds[1].bondStatus == BondState.BREAKTRIBLE)
            {
                bondStatus = BondState.TRIBLE;
            }

            //  Debug.LogWarning("single update");
            plusMinus = PlusMinus.COMBINE;
            AtomController atom01 = paramaterAtoms[0];
            AtomController atom02 = paramaterAtoms[1];

            paramaterBonds[0].UnitSphere = (RootNode.position - atom01.transform.position).normalized;
            paramaterBonds[1].UnitSphere = (HeadNode.position - atom02.transform.position).normalized;

            Debug.DrawRay(RootNode.position, paramaterBonds[0].UnitSphere* 0.003f, Color.black);
            Debug.DrawRay(HeadNode.position, paramaterBonds[1].UnitSphere * 0.003f, Color.red);
            //            paramaterBonds[1]


            Vector3 dir = (atom01.transform.position - atom02.transform.position).normalized;
            Vector3 rootPos = transform.position - dir * atom01.size / 2.0f;
            Vector3 headPos = atom02.transform.position + dir * atom02.size / 2.0f;
            RootNode.transform.position = rootPos;
            HeadNode.transform.position = headPos;
            ParticleSystem ps = HeadNode.GetComponent<ParticleSystem>();
            ps.startLifetime = 100;
            ps.startSize = 0.06f;

            transform.position = atom01.transform.position;

            bondLine.SetVertexCount(2);


            bondLine.SetPosition(0, rootPos);
            bondLine.SetPosition(1, headPos);

            //计算距离
            float maxmizeDistance = maxmizeBreakMultiple * (atom01.size + atom02.size);

            float normalDistance =  (atom01.size + atom02.size);

            float realDistance = Vector3.Distance(atom01.transform.position, atom02.transform.position);
            //化学键断裂
            if (realDistance > maxmizeDistance)
            {

                if (!Application.isPlaying)
                {
                    Vector3 dest = atom01.transform.position + (atom02.transform.position- atom01.transform.position).normalized * (atom01.size + atom02.size);

                    atom02.transform.position = dest;
                }
                else
                {
                    /*    paramaterBonds[1].parentBond = null;

                      //  Vector3 A2Pos = HeadNode.transform.position;

                        atom02.transform.parent = null;
                        atom02.transform.localPosition = HeadNode.transform.position;
                        //atom02.transform.position = A2Pos;

                        paramaterBonds[0].parentBond = null;*/
                    NoticeDestroy = true;
                    paramaterBonds[0].OnDisconnect();// = null;
                    paramaterBonds[1].OnDisconnect();// = null;


                    atom02.GetComponent<DragCommand>().enabled = false;
                    paramaterAtoms.Clear();
                  //  NoticeDestroy = true;
                    Destroy(gameObject);
                }
            }
            else
            {
                paramaterBonds[0].OnConnect();// = null;
                paramaterBonds[1].OnConnect();// = null;
                if (realDistance > normalDistance)
                {
                   float parVal = 5.0f - 5.0f * (realDistance - normalDistance) / (maxmizeDistance - normalDistance);
                    bondLine.material.SetFloat("_Emission",parVal);
                }
                else
                {
                    bondLine.material.SetFloat("_Emission", 5.0f);
                }
            }

          //  Debug.LogWarning("Single Update");


            //Vector3 randSphere = UnitSphere;
            ColorTransform();

            bondLine.material.mainTextureScale = Vector2.one;
        }
        else
        {
            bondStatus = BondState.BREAK;
        }
    }

    void BondBreakDoubleUpdate()
    {
        BondBreakUpdate();
        bondLine.material.mainTextureScale = new Vector2(1.0f, 2.0f);
    }

    void BondBreakTribleUpdate()
    {
        BondBreakUpdate();
        bondLine.material.mainTextureScale = new Vector2(1.0f, 3.0f);
    }
    void BondDoubleUpdate()
    {
        BondSingleUpdate();
        bondLine.material.mainTextureScale = new Vector2(1.0f, 2.0f);
    }
    void BondTribleUpdate()
    {
        BondSingleUpdate();
        bondLine.material.mainTextureScale = new Vector2(1.0f, 3.0f);
    }

    LineRenderer bdLine = null;
    public LineRenderer bondLine
    {
        get
        {
            if(bdLine == null)
            {
                bdLine = GetComponent<LineRenderer>();
            }
            return bdLine;
        }
    }


    public float NodeDistance
    {
        get
        {
            return Vector3.Distance(RootNode.position, HeadNode.position);//.normalized;
        }
    }


    public Vector3 NodeDirection
    {
        get
        {
            return (RootNode.position - HeadNode.position).normalized;
        }
    }

    public Transform RootNode
    {
        get
        {
            if(root == null)
            {
                root = transform.FindChild("Root");
            }
            return root;
        }
    }
    Transform root;
    public Transform HeadNode
    {
        get
        {
            if (head == null)
            {
                head = transform.FindChild("Head");
            }
            return head;
        }
    }
    Transform head;
    void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            Update();
        }

        /*List<Vector3> positions = new List<Vector3>();
        Transform[] transGrp = GetComponentsInChildren<Transform>();
        bondLine.SetVertexCount(transGrp.Length );
        foreach (Transform t in transGrp)
        {
       //     if (t != transform)
      //      {
                positions.Add(t.position);
    //        }
        }
        bondLine.SetPositions(positions.ToArray());*/
    }
}
