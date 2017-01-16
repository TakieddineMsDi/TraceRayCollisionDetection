using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Collisions : MonoBehaviour
{
	private GameObject This;
	private GameObject[] Others;
	bool Do = true;
	void Start ()
	{
		This = this.gameObject;
		//ThisLastPos = new Vector3();
		// Test si l'objet contient meshfilter pour recupérer les vertices
		if (This.GetComponent<MeshFilter> () == null) {
			Do = false;
		}
		if (Do) {
			//recupérer tous les objets dans la scene pour effectuer le test de collision avec eux
			Others = GameObject.FindObjectsOfType<GameObject> ();
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		Others = GameObject.FindObjectsOfType<GameObject> ();
		if (Do) {
			int i = 0;
			foreach (GameObject Other in Others) {
				if (This.GetInstanceID () != Other.GetInstanceID ()) {
					if (Other.GetComponent<MeshFilter> () == null) {
						Do = false;
					}
					if (Other.tag == "Out") {
						Do = false;
					}
					if (Do) {
						RayTracedCollisionDetection (This, Other);
					}
				}
				Do = true;
				i++;
			}
		}
	}
	
	void RayTracedCollisionDetection (GameObject Obj1, GameObject Obj2)
	{
		/* P1 et P2 collision pair des 2 objets
		 * P3 is the intersecting point
           between Obj 1 and the ray casted from p 1 with p1 ’s reverse
           normal direction
		 * CollisionPairs Contient tous les Collision Pairs
		 *
		*/
		Vector3 P1, P2, P3, N1, N2, N3;
		List<Vector3> CollisionPairs = new List<Vector3> ();

		for (int i = 0; i < Obj1.GetComponent<MeshFilter> ().mesh.vertexCount; i++) {
			P1 = Obj1.transform.TransformPoint (Obj1.GetComponent<MeshFilter> ().mesh.vertices [i]);
			N1 = Obj1.GetComponent<MeshFilter> ().mesh.normals [i];
			
			TraceRay (P1, -N1, Obj2, out P2, out N2,true);

			// Produit scalaire des 2 vecteur normal si > 0 alors angle < 90 on passe au autres points
			// sinon angle >= 90 sens opposé
			if (Vector3.Dot (N1, N2) > 0) {
				break;
			}
			
			TraceRay (P2, N1, Obj2, out P3, out N3,false);

			// Distance entre les points
			if ((P2 - P1).magnitude <= (P2 - P3).magnitude) {
				print ("Pair Point 1 "+P1);
				print ("Pair Point 2 "+P2);
				CollisionPairs.Add (P1);
				CollisionPairs.Add (P2);
			}
		}

	}
	
	
	void TraceRay (Vector3 InPoint, Vector3 InNormal, GameObject Obj2, out Vector3 OutPoint, out Vector3 OutNormal,bool draw)
	{
		double R, u, v, t;
		int[] Summits = (Obj2.GetComponent<MeshFilter> ().mesh.triangles);
		int Count = Summits.Length / 3;
		// Les 3 sommets de chaque triangle
		Vector3 V1, V2, V3, OutP = new Vector3 (), OutN = new Vector3 ();
		
		for (int i = 0; i < Count; i++) {
			V1 = Obj2.transform.TransformPoint (Obj2.GetComponent<MeshFilter> ().mesh.vertices [Summits [i * 3 + 0]]);
			V2 = Obj2.transform.TransformPoint (Obj2.GetComponent<MeshFilter> ().mesh.vertices [Summits [i * 3 + 1]]);
			V3 = Obj2.transform.TransformPoint (Obj2.GetComponent<MeshFilter> ().mesh.vertices [Summits [i * 3 + 2]]);
			
			intersect_triangle (InPoint, InNormal, V1, V2, V3, out R, out u, out v, out t);
			
			if (R == 1) {
				
				OutP = (float)(1 - u - v) * V1 + (float)u * V2 + (float)v * V3;
				OutN = Vector3.Cross (V3 - V1, V2 - V1);
				if(draw){
				Debug.DrawRay (InPoint, InNormal, Color.red);
				}

			}
		}

		OutPoint = OutP;
		OutNormal = OutN;
	}
	
	void intersect_triangle (Vector3 O, Vector3 D, Vector3 V1, Vector3 V2, Vector3 V3, out double R, out double u, out double v, out double t)
	{
		double epsilon = 0.000001;
		Vector3 Edge1, Edge2, T, P, Q;
		double det, inv_det, Uu, Vv, Tt;
		//Trouver les edges du triangle
		Edge1 = V2 - V1;
		Edge2 = V3 - V1;
		
		//
		P = Vector3.Cross (D, Edge2);
		
		//Determinant
		det = Vector3.Dot (Edge1, P);
		
		//Si le determinant est proche du zero, le rayon se trouve dans le plan du triangle
		if (det < epsilon) {
			R = 0;
			u = v = t = 0;
			return;

		}
		
		//Calculer Distance entre V1 et O
		T = O - V1;
		
		//Calculer u
		Uu = Vector3.Dot (T, P);
		if (Uu < 0.0 || Uu > det) {
			R = 0;
			u = v = t = 0;
			return;
		}
		
		//
		Q = Vector3.Cross (T, Edge1);
		
		//Calculer v
		Vv = Vector3.Dot (D, Q);
		if (Vv < 0.0 || Uu + Vv > det) {
			R = 0;
			u = v = t = 0;
			return;
		}
		
		//Calculer t
		Tt = Vector3.Dot (Edge2, Q);
		inv_det = 1.0 / det;
		
		Tt *= inv_det;
		Uu *= inv_det;
		Vv *= inv_det;
		
		R = 1;
		u = Uu;
		v = Vv;
		t = Tt;
	}
	
}
