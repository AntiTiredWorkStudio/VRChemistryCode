using UnityEngine;
using System.Collections;

public class VRMoveRegion : MonoBehaviour {
    void OnEnable()
    {
      //  Debug.LogWarning("On Enable");
        foreach (KunaiRotation k in GetComponentsInChildren<KunaiRotation>())
        {
            k.Initialize();
        }
        foreach (ParticleSystem ps in GetComponentsInChildren<ParticleSystem>())
        {
            ps.Clear();
            ps.Simulate(0.0f);
            ps.Play();
        }
    }



    void OnDisable()
    {
       // Debug.LogWarning("On Disable");
        foreach (KunaiRotation k in GetComponentsInChildren<KunaiRotation>())
        {
            k.isRotation = false;
           // k.Initialize();
        }
        foreach (ParticleSystem ps in GetComponentsInChildren<ParticleSystem>())
        {
            ps.Clear();
            ps.Stop();
        }
    }
}
