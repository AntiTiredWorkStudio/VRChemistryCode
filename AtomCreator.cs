using UnityEngine;
using System.Collections;
[RequireComponent(typeof(AtomController))]
public class AtomCreator : MonoBehaviour {
    public string atomSysbolName;
	[ContextMenu("Load Atom")]
    void LoadAtom()
    {
        AtomController atom = GetComponent<AtomController>();
           CEDisplayWindow CedisplayWnd = 
            CardSpawner.Instance.CrerateCEDisplayWindow(atomSysbolName);
        if(CedisplayWnd == null)
        {
            Debug.LogWarning("未找到元素符号:"+atomSysbolName);
            return;
        }
        atom.ResetAtom();
        atom.transform.localScale = 0.01054689f * Vector3.one;
        try
        {
            atom.GetComponent<Renderer>().material.shader = Shader.Find("Standard");
        atom.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
        atom.GetComponent<Renderer>().material.SetFloat("_Glossiness", 0.843f);
//            atom.GetComponent<Renderer>().
        }
        catch
        {

        }
        atom.LoadAtom(CedisplayWnd.Symbol, 
            AtomController.AColorToColor(CedisplayWnd.AColor),
            CedisplayWnd.Electron, 
                CedisplayWnd.ARadius, 
            CedisplayWnd.Valence);
        name = atomSysbolName;
        ShowElectrons = true;


        foreach (Transform sphere in GetComponentsInChildren<Transform>())
        {
            if (sphere.name == "Sphere")
            {
                DestroyImmediate(sphere.gameObject);
            }
        }
        if (HighQuality)
        {
            ShowElectrons = false;
            atom.HighQualityView();
        }
    }
    public bool HighQuality = false;
    public bool AutoChange = false;
    public bool ShowElectrons = true;
    void OnDrawGizmosSelected()
    {
        if (!AutoChange)
        {
            return;
        }
        if(GetComponent<AtomController>().text3DName.text != atomSysbolName)
        {
            LoadAtom();
            GetComponent<AtomController>().ELECCTRONS.gameObject.SetActive(ShowElectrons);
        }
    }
}
