using UnityEngine;
using System.Collections;

public class GetPoints : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Mesh m = gameObject.GetComponent<MeshFilter> ().mesh;
		print (m.vertexCount);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
