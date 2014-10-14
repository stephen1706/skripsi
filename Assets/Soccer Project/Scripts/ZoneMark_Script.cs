using UnityEngine;
using System.Collections;

public class ZoneMark_Script : MonoBehaviour {
	public GameObject zoneOwner;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other){
		if (other.gameObject.tag == "PlayerTeam1") {
			//option 1 : mark enemy yg masuk ke zona dy
			//zoneOwner.GetComponent<Player_Script> ().enemyMarked = other.gameObject;

			//option 2: diemin pemaen yg bukan dy mark seharusnya, kecuali bek
			if(zoneOwner.GetComponent<Player_Script> ().originMarked == other.gameObject){
				zoneOwner.GetComponent<Player_Script> ().state = Player_Script.Player_State.MARK_ENEMY;
				other.gameObject.GetComponent<Player_Script>().whoMarkedMe = zoneOwner;
			} else if(zoneOwner.GetComponent<Player_Script> ().type == Player_Script.TypePlayer.DEFENDER &&
			          !other.gameObject.GetComponent<Player_Script>().whoMarkedMe){//klo buat bek, tempel aj, tp selama blm ada yg jagain dy
				zoneOwner.GetComponent<Player_Script> ().enemyMarked = other.gameObject;
				zoneOwner.GetComponent<Player_Script> ().state = Player_Script.Player_State.MARK_ENEMY;
				other.gameObject.GetComponent<Player_Script>().whoMarkedMe = zoneOwner;
			}
			Debug.Log(zoneOwner.name + "'s zone breach by " + other.gameObject.name);
		} else if(other.gameObject.tag == "Ball"){
			zoneOwner.GetComponent<Player_Script> ().state = Player_Script.Player_State.STOLE_BALL;
		}

	}
	void OnTriggerExit(Collider other){
		if (other.gameObject.tag == "PlayerTeam1") {
			if(other.gameObject.GetComponent<Player_Script>().whoMarkedMe == zoneOwner){
				zoneOwner.GetComponent<Player_Script> ().enemyMarked = zoneOwner.GetComponent<Player_Script> ().originMarked;
				zoneOwner.GetComponent<Player_Script> ().state = Player_Script.Player_State.MOVE_AUTOMATIC;
				other.gameObject.GetComponent<Player_Script>().whoMarkedMe = null;
			}

		} else if(other.gameObject.tag == "Ball"){
			zoneOwner.GetComponent<Player_Script> ().state = Player_Script.Player_State.MOVE_AUTOMATIC;
		}
		
	}
}
