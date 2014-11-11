using UnityEngine;
using System.Collections;

public class ZoneMark_Script : MonoBehaviour {
	public GameObject zoneOwner;
	public Sphere sphere;
	public InGameState_Script inGame;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other){
		if ((other.gameObject.tag == "PlayerTeam1" && zoneOwner.tag == "OponentTeam" && (!sphere.owner || sphere.owner.tag == "PlayerTeam1"))|| //msh rada ngebug sejak diganti ifnya jd buat yg player jg, jd bnyk yg stole ball smua. klo msh ngebug di git stash aj
		    (other.gameObject.tag == "OponentTeam" && zoneOwner.tag == "PlayerTeam1" && (!sphere.owner || sphere.owner.tag == "OponentTeam"))){
			//option 1 : mark enemy yg masuk ke zona dy
			//zoneOwner.GetComponent<Player_Script> ().enemyMarked = other.gameObject;

			//option 2: diemin pemaen yg bukan dy mark seharusnya, kecuali bek
			if(zoneOwner.GetComponent<Player_Script> ().originMarked == other.gameObject){
				zoneOwner.GetComponent<Player_Script> ().state = Player_Script.Player_State.MARK_ENEMY;
				other.gameObject.GetComponent<Player_Script>().whoMarkedMe = zoneOwner;

			} else if(zoneOwner.GetComponent<Player_Script> ().type == Player_Script.TypePlayer.DEFENDER &&
			          !other.gameObject.GetComponent<Player_Script>().whoMarkedMe){//klo buat bek, tempel aj, tp selama blm ada yg jagain dy
				//if(zoneOwner.GetComponent<Player_Script> ().state != Player_Script.Player_State.STOLE_BALL){
					//mencegah biar saat dy mw ambil bola,tp ad enemy yg msk ke zonany, bs tiba" ganti arah
					zoneOwner.GetComponent<Player_Script> ().enemyMarked = other.gameObject;
					zoneOwner.GetComponent<Player_Script> ().state = Player_Script.Player_State.MARK_ENEMY;
					other.gameObject.GetComponent<Player_Script>().whoMarkedMe = zoneOwner;
				//}
			}
			//aaDebug.Log(zoneOwner.name + "'s zone breach by " + other.gameObject.name);
		} else if(other.gameObject.tag == "Ball" && zoneOwner.GetComponent<Player_Script>().type != Player_Script.TypePlayer.ATTACKER){
			if(sphere.whoMarkedMe.Count < 1 && sphere.owner && sphere.owner.tag != gameObject.tag){//biar max 2org yg ngejer bola,bs jg cari smua pemaen statenya lg stole ball kg
				if(!sphere.whoMarkedMe.Contains(gameObject)){
					sphere.whoMarkedMe.Add(gameObject);
				}

				zoneOwner.GetComponent<Player_Script> ().state = Player_Script.Player_State.STOLE_BALL;
			}
		}

	}
	void OnTriggerExit(Collider other){//exit itu trnyt dipanggil tiap x diluar zona, bkn saat kluar zona doang
		if (other.gameObject.tag == "PlayerTeam1" && zoneOwner.tag == "OponentTeam" || 
		    other.gameObject.tag == "OponentTeam" && zoneOwner.tag == "PlayerTeam1" ) {
			if(other.gameObject.GetComponent<Player_Script>().whoMarkedMe == zoneOwner){
				zoneOwner.GetComponent<Player_Script> ().enemyMarked = zoneOwner.GetComponent<Player_Script> ().originMarked;
				zoneOwner.GetComponent<Player_Script> ().state = Player_Script.Player_State.MOVE_AUTOMATIC;
				other.gameObject.GetComponent<Player_Script>().whoMarkedMe = null;
			}

		} else if(other.gameObject.tag == "Ball"){
			sphere.whoMarkedMe.Remove(gameObject);
			zoneOwner.GetComponent<Player_Script> ().state = Player_Script.Player_State.MOVE_AUTOMATIC;
		} else if(other.gameObject == zoneOwner){//klo lg lari kejer bola tp ud kepentok zonany
			zoneOwner.GetComponent<Player_Script> ().timeToStopRest = 0;
			zoneOwner.GetComponent<Player_Script> ().state = Player_Script.Player_State.RESTING;
		}
		
	}

	void OnTriggerStay(Collider other) {
		if (other.gameObject.tag == "Ball") {
			if(inGame.state == InGameState_Script.InGameState.PLAYING && sphere.gameObject.GetComponent<Rigidbody>().isKinematic == false){
				zoneOwner.GetComponent<Player_Script> ().state = Player_Script.Player_State.STOLE_BALL_NO_CHECK;
			} else{
				zoneOwner.GetComponent<Player_Script> ().state = Player_Script.Player_State.GO_ORIGIN;
			}
		}
	}
}
