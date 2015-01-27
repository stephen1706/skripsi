using UnityEngine;
using System.Collections;

public class Area_Script : MonoBehaviour {//buat cek GK kluar dr kotak penalty kaga

	void Start () {
	
	}

	void Update () {
	
	}

	void OnTriggerExit( Collider coll) {//saat GK keluar kotak penalti
		if ( coll.gameObject.tag == "GoalKeeper" || coll.gameObject.tag == "GoalKeeper_Oponent" ) {
					//klo GK kluar kotak PK suruh balik ke tmpt awal
			coll.gameObject.GetComponent<GoalKeeper_Script>().state = GoalKeeper_Script.GoalKeeper_State.GO_ORIGIN;
			
		}
	
	
	}
	
}
