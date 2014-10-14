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
			zoneOwner.GetComponent<Player_Script> ().enemyMarked = other.gameObject;
			zoneOwner.GetComponent<Player_Script> ().state = Player_Script.Player_State.MARK_ENEMY;
			Debug.Log(zoneOwner.name + "'s zone breach by " + other.gameObject.name);
		} else if(other.gameObject.tag == "Ball"){
			zoneOwner.GetComponent<Player_Script> ().state = Player_Script.Player_State.STOLE_BALL;
		}

	}
	void OnTriggerExit(Collider other){
		if (other.gameObject.tag == "PlayerTeam1") {
			zoneOwner.GetComponent<Player_Script> ().enemyMarked = zoneOwner.GetComponent<Player_Script> ().originMarked;
			zoneOwner.GetComponent<Player_Script> ().state = Player_Script.Player_State.MOVE_AUTOMATIC;
		} else if(other.gameObject.tag == "Ball"){
			zoneOwner.GetComponent<Player_Script> ().state = Player_Script.Player_State.MOVE_AUTOMATIC;
		}
		
	}
}
