using UnityEngine;
using System.Collections;

public class ZoneMark_Script : MonoBehaviour {
	public GameObject zoneOwner;
	public Sphere sphere;
	public InGameState_Script inGame;

	void Start () {

	}

	void Update () {
	
	}

	void OnTriggerEnter(Collider other){
		if ((other.gameObject.tag == "PlayerTeam1" && zoneOwner.tag == "OponentTeam" && (!sphere.owner || sphere.owner.tag == "PlayerTeam1"))|| //msh rada ngebug sejak diganti ifnya jd buat yg player jg, jd bnyk yg stole ball smua. klo msh ngebug di git stash aj
		    (other.gameObject.tag == "OponentTeam" && zoneOwner.tag == "PlayerTeam1" && (!sphere.owner || sphere.owner.tag == "OponentTeam"))){
			//diemin pemaen yg bukan dy mark seharusnya, kecuali bek
			if(zoneOwner.GetComponent<Player_Script> ().originMarked == other.gameObject){
				zoneOwner.GetComponent<Player_Script> ().state = Player_Script.Player_State.MARK_ENEMY;
				//keuntungan: fokus jaga siapa, kg kyk zombie ngejer bola trs
				//kerugian: klo lg kejer bola yg kosong bisa bs kg jd ngejer
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
			if(!sphere.owner){//biar max 2org yg ngejer bola,bs jg cari smua pemaen statenya lg stole ball kg
//				if(!sphere.whoMarkedMe.Contains(gameObject)){
//					sphere.whoMarkedMe.Add(gameObject);
//				}

				zoneOwner.GetComponent<Player_Script> ().state = Player_Script.Player_State.STOLE_BALL;
			}
		}

	}
	void OnTriggerExit(Collider other){//exit itu trnyt dipanggil tiap x diluar zona, bkn saat kluar zona doang
		if ((other.gameObject.tag == "PlayerTeam1" && zoneOwner.tag == "OponentTeam" && (!sphere.owner || sphere.owner.tag == "PlayerTeam1"))|| //msh rada ngebug sejak diganti ifnya jd buat yg player jg, jd bnyk yg stole ball smua. klo msh ngebug di git stash aj
		    (other.gameObject.tag == "OponentTeam" && zoneOwner.tag == "PlayerTeam1" && (!sphere.owner || sphere.owner.tag == "OponentTeam"))){
			if(other.gameObject.GetComponent<Player_Script>().whoMarkedMe == zoneOwner){
				zoneOwner.GetComponent<Player_Script> ().enemyMarked = zoneOwner.GetComponent<Player_Script> ().originMarked;
				zoneOwner.GetComponent<Player_Script> ().state = Player_Script.Player_State.MOVE_AUTOMATIC;
				other.gameObject.GetComponent<Player_Script>().whoMarkedMe = null;
			}

		} else if(other.gameObject.tag == "Ball"){
			//sphere.whoMarkedMe.Remove(gameObject);
			zoneOwner.GetComponent<Player_Script> ().state = Player_Script.Player_State.MOVE_AUTOMATIC;
		} else if(other.gameObject == zoneOwner){//klo lg lari kejer bola tp ud kepentok zonany
			zoneOwner.GetComponent<Player_Script> ().timeToStopRest = 0;
			zoneOwner.GetComponent<Player_Script> ().state = Player_Script.Player_State.RESTING;
		}
		
	}

	void OnTriggerStay(Collider other) {
		//dibuat gini demi ai bs ngejer bola lg ga ad owner, tp kerugianny jd slalu kejer bola yg msk zonany walopun ud ada yg kejer
			if (other.gameObject.tag == "Ball" && (!sphere.owner || sphere.owner.tag != zoneOwner.tag)) {
				if(inGame.state == InGameState_Script.InGameState.PLAYING && sphere.gameObject.GetComponent<Rigidbody>().isKinematic == false){
					if(zoneOwner.GetComponent<Player_Script>().state != Player_Script.Player_State.MARK_ENEMY){
						zoneOwner.GetComponent<Player_Script> ().state = Player_Script.Player_State.STOLE_BALL_NO_CHECK;
					}
				} 
			}

	}
}
