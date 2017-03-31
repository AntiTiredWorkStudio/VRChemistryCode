using UnityEngine;
using System.Collections;

public class Rotation_ : MonoBehaviour {
	private GameObject atom;
	private GameObject electron;
	// Use this for initialization
	void Start () {
		atom = GameObject.Find ("atom01");
		electron=GameObject.Find ("electron01");
	}
	
	// Update is called once per frame
	void Update () {
		electron.transform.RotateAround (atom.transform.position, Vector3.forward, 5f);
	}
}
