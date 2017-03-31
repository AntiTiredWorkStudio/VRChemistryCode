using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ElectronController : MonoBehaviour {
    public GameObject trackPrefab;
    public GameObject electronPrefab;
    public List<GameObject> electronsList;
    public List<GameObject> electronList;
    public Dictionary<int,ElectronLayer> ElectronLayerList;
    void Start()
    {
        trackPrefab.SetActive(false);
        electronPrefab.SetActive(false);
        ElectronLayerList = new Dictionary<int, ElectronLayer>();
    }

    public void ResetElectron()
    {
        if(electronsList != null && electronsList.Count > 0)
        {
            foreach(GameObject ele in electronsList)
            {
                if (Application.isPlaying)
                {
                    Destroy(ele);
                }
                else
                {
                    DestroyImmediate(ele);
                }
            }
            
        }

        if (electronList != null && electronList.Count > 0)
        {
            foreach (GameObject ele in electronList)
            {
                if (Application.isPlaying)
                {
                    Destroy(ele);
                }
                else
                {
                    DestroyImmediate(ele);
                }
            }

        }
        electronsList = new List<GameObject>();
        electronList = new List<GameObject>();
    }

    public void ElectronConfig(Dictionary<string, ElectronLayer> electronLayer,float size)
    {
        electronsList = new List<GameObject>();
        electronList = new List<GameObject>();
        ElectronLayerList = new Dictionary<int, ElectronLayer>();
        foreach (string key in electronLayer.Keys)
        {
            ElectronLayer el = electronLayer[key];
            string trackName = el.LayerName.ToString().Replace("_", "");
            string trackCode = trackName[trackName.Length - 1].ToString();
            int layerIndex = int.Parse(trackName.Replace(trackCode, ""));
            switch (trackCode)
            {
                case "d":
                    layerIndex += 1;
                    break;
                case "f":
                    layerIndex += 2;
                    break;
                default:
                    break;
            }
            if (!ElectronLayerList.ContainsKey(layerIndex))
            {
                ElectronLayerList.Add(layerIndex, new ElectronLayer(el.ElectronCount,el.Speed,el.LayerName));
            }
            else{
                ElectronLayerList[layerIndex].ElectronCount += el.ElectronCount;
            }
        }
        int count = ElectronLayerList.Count;
        int seek = 1;
        float singleRate = 360.0f/(count+1);
        Debug.LogWarning(singleRate);
        while (ElectronLayerList.ContainsKey(seek))
        {
            trackPrefab.SetActive(true);
            GameObject instance = Instantiate(trackPrefab) as GameObject;
            electronsList.Add(instance);
            instance.transform.parent = trackPrefab.transform.parent;
            instance.transform.localPosition = trackPrefab.transform.localPosition;
            Vector3 eularAngle = instance.transform.localEulerAngles;
            eularAngle.y = singleRate * seek;
          //  instance.transform.localEulerAngles = eularAngle;
            instance.transform.localScale = trackPrefab.transform.localScale+Vector3.one*(0.02f*seek);
            PathCircle currentPath = instance.GetComponent<PathCircle>();
            for(int i = 0;i< ElectronLayerList[seek].ElectronCount; i++)
            {
                electronPrefab.SetActive(true);
                GameObject electron = Instantiate(electronPrefab);
                electronList.Add(electron);
                electron.transform.parent = electronPrefab.transform.parent;
                electron.transform.localPosition = electronPrefab.transform.localPosition;
                electron.transform.localScale = electronPrefab.transform.localScale;
                electron.transform.localRotation = electronPrefab.transform.localRotation;
                electron.GetComponent<ElectronMovement>().targetPath = currentPath;
                electron.GetComponent<ElectronMovement>().precentage = 1.0f / ((float)i);
                electron.GetComponent<ElectronMovement>().Speed = (0.24f - 0.04f * seek);
               electronPrefab.SetActive(false);
            }
            Debug.Log(ElectronLayerList[seek].ElectronCount);
            trackPrefab.SetActive(false);
            seek++;
        }
    }

    void OnDrawGizmosSelected()
    {
        trackPrefab = transform.FindChild("electronTrack").gameObject;
        electronPrefab = transform.FindChild("electron").gameObject;
    }
}
