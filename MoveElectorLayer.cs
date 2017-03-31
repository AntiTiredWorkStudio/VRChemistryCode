using UnityEngine;
using System.Collections;

public class MoveElectorLayer : MonoBehaviour {
    public float speed = 100.0f;
    Vector3 dir = Vector3.one;
    Vector3[] directions = new Vector3[]
    {
        Vector3.left,
        Vector3.right
    };
    void Start()
    {
        dir = directions[(int)(Random.Range(0,directions.Length-1))];
    }

	void Update () {
        transform.Rotate(dir * speed * Random.Range(1.5f,5.0f) * Time.deltaTime);
	}
}
