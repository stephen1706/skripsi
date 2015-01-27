using UnityEngine;
using System.Collections;

public class Shadow_Script : MonoBehaviour {
	void Start () {
	
	}

	void LateUpdate () {
	
		transform.rotation = Quaternion.Euler( -90, 0, 0);
		//puter bayangan -90 derajat disumbu x,biar bayanganny diem d situ aj kg muter" wkt pmaen muter jg, jd trikny di puter" trs cpt 90 derajat jd gt" aj kliatanny
		
	}
}
