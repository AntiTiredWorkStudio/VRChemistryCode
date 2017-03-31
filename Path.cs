using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace atom
{
    public class Path : MonoBehaviour
    {
        List<Transform> subPoint = null;
        public bool ShowGizmosPoint = true;
        public bool ShowGizmosLine = true;
        public Transform[] PathTransforms
        {
            get
            {
                if (subPoint == null || subPoint.Count == 0)
                {
                    int index = 0;
                    subPoint = new List<Transform>();
                    foreach (Vector3 v3 in GetPath().ToArray())
                    {
                        GameObject point = new GameObject("point_" + index);
                        index++;
                        point.transform.position = v3;
                        point.transform.parent = transform;
                        //  point.hideFlags = HideFlags.HideInHierarchy;
                        if(point.transform == null)
                        {
                            continue;
                        }
                        subPoint.Add(point.transform);
                    }
                }

                return subPoint.ToArray();
            }
        }
        // Use this for initialization
        void Start()
        {
            Reset();
        }

        // Update is called once per frame
        void Update()
        {

        }
        Color signalColor = Color.black;
        /// <summary>
        /// 需要在重置时保留的属性，请在此设定
        /// </summary>
        protected virtual void Reset()
        {
            signalColor = new Color(
                    Random.Range(0.0f, 1.0f),
                    Random.Range(0.0f, 1.0f),
                    Random.Range(0.0f, 1.0f)
                );
            signalColor.a = 1.0f;
            if (transform.childCount > 0)
            {
                subPoint = new List<Transform>();
                for (int i = 0; i < transform.childCount; i++)
                {
                    if (transform.FindChild("point_" + i))
                    {
                        subPoint.Add(transform.FindChild("point_" + i));
                    }
                }
                return;
            }
            for (int i = 0; i < transform.childCount; i++)
            {
                DestroyImmediate(
                    transform.GetChild(i).gameObject);
            }
            subPoint = null;
        }

        void OnDrawGizmos()
        {
            if (subPoint == null)
            {
                return;
            }

            Gizmos.color = signalColor;
            if (ShowGizmosLine)
            {

                Gizmos.color = signalColor;

                iTween.DrawPathGizmos(subPoint.ToArray(), signalColor);
            }
            if (ShowGizmosPoint)
            {
                foreach (Transform t in subPoint.ToArray())
                {
                    if (t == null)
                    {
                        continue;
                    }
                    Gizmos.color = signalColor;
                    Gizmos.DrawWireSphere(t.position, 0.3f);
                }
            }
        }

        void OnDrawGizmosSelected()
        {
            if (subPoint == null || subPoint.Count == 0)
            {
                Reset();
                int index = 0;
                subPoint = new List<Transform>();
                foreach (Vector3 v3 in GetPath().ToArray())
                {
                    GameObject point = null;
                    if (transform.FindChild("point_" + index))
                    {
                        point = transform.FindChild("point_" + index).gameObject;
                        subPoint.Add(point.transform);
                    }
                    else
                    {
                        point = new GameObject("point_" + index);
                        point.transform.position = v3;
                        point.transform.parent = transform;
                        subPoint.Add(point.transform);
                    }
                    index++;
                }
            }
        }

        public virtual List<Vector3> GetPath()
        {
            List<Vector3> v3 = new List<Vector3>();
            return v3;
        }
    }
}