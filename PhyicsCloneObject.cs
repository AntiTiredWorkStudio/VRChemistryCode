using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PhyicsCloneObject : PhyicsPickUpObject
{
    public bool AllowClone = true;
    public override void DragStartAction()
    {
        base.DragStartAction();
        if (!AllowClone)
        {
            return;
        }
        /*if(DisplayWindow.OnSelectedDisplayWindow == null)
        {
            return;
        }*/
        GameObject atoms = 
        Instantiate(gameObject) as GameObject;
        atoms.transform.parent = transform.parent;
        atoms.transform.localPosition = transform.localPosition;
        atoms.transform.localRotation = transform.localRotation;
        atoms.transform.localScale = transform.localScale;
        atoms.name = atoms.name.Replace("(Clone)", "");
        if (DisplayWindow.OnSelectedDisplayWindow != null)
        {
            DisplayWindow.OnSelectedDisplayWindow.atom = atoms.GetComponent<AtomController>();
        }
        else
        {
            DisplayWindow display = atoms.GetComponentInParent<DisplayWindow>();
            display.gameObject.GetComponent<DisplayWindow>().OnSelection();
            DisplayWindow.OnSelectedDisplayWindow.atom = atoms.GetComponent<AtomController>();
        }
        /*foreach(ElectronMovement electron in atoms.GetComponentsInChildren<ElectronMovement>())
        {
            atoms.GetComponent<AtomController>().ELECCTRONS.electronList.Add(electron.gameObject);
        }*/
        transform.parent = null;
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
                layer.transform.localScale = Vector3.one - Vector3.one* ((float)index/(float)dictionary.Keys.Count);
            }
        }
        /* PhyicsPickUpObject ppuo = gameObject.AddComponent<PhyicsPickUpObject>();
         ppuo.FollowedObjectTransform = transform;
         ppuo.MinTriggerdDistance = MinTriggerdDistance;
         Destroy(this);*/
        physRigid.isKinematic = false;
        AllowClone = false;
    }
}
