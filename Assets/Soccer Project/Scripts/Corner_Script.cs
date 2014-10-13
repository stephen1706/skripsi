using UnityEngine;
using System.Collections;

public class Corner_Script : MonoBehaviour {
	
	
	public Transform downPosition;
	public Transform upPosition;
	
	public GameObject area;
	public Transform point_goalkick;
	public GameObject goalKeeper;
	
	public Sphere sphere;
	
	// Use this for initialization
	void Start () {
	
		sphere = (Sphere)GameObject.FindObjectOfType( typeof(Sphere) );
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	
	void OnTriggerEnter( Collider other) {//dipanggin klo bolany msk ke area corner trigger,diluar garis
		//pake Camera itu krn main camera di tag MainCamera bisa diakses via Camera. terus main camera yg nampung ingamestate_script
		if ( Camera.main.GetComponent<InGameState_Script>().state != InGameState_Script.InGameState.GOAL ) {//klo bkn goal

			// Detect if Players are outside of field
			if ( (other.gameObject.tag == "PlayerTeam1" || other.gameObject.tag == "OponentTeam") && Camera.main.GetComponent<InGameState_Script>().state == InGameState_Script.InGameState.PLAYING ) {
			
				if ( other.gameObject != sphere.owner ) {//bikin player kg bs select
				
					other.gameObject.GetComponent<Player_Script>().temporallyUnselectable = true;
					other.gameObject.GetComponent<Player_Script>().timeToBeSelectable = 0.5f;
					other.gameObject.GetComponent<Player_Script>().state = Player_Script.Player_State.GO_ORIGIN;
				}
				
			}
	
			// Chekc if is corner-kick or goal-kick
			if ( other.gameObject.tag == "Ball" && Camera.main.GetComponent<InGameState_Script>().state == InGameState_Script.InGameState.PLAYING ) {
				
				sphere.owner = null;
				Camera.main.GetComponent<InGameState_Script>().timeToChangeState = 2.0f;
				Camera.main.GetComponent<InGameState_Script>().areaCorner = area;
				Camera.main.GetComponent<InGameState_Script>().goal_kick = point_goalkick;
				Camera.main.GetComponent<InGameState_Script>().goalKeeper = goalKeeper;
				Camera.main.GetComponent<InGameState_Script>().cornerTrigger = this.gameObject;
				
				
				// loonking for the near corner point
				Vector3 positionBall = sphere.gameObject.transform.position;			
				if ( (positionBall-downPosition.position).magnitude > (positionBall-upPosition.position).magnitude ) {//atur mau conrner kiri ato kanan
					Camera.main.GetComponent<InGameState_Script>().cornerSource = upPosition;//set posisi corner diatas. upPosition itu ada object di hierarchy buat penunjuk lokasinya
				} else {
					Camera.main.GetComponent<InGameState_Script>().cornerSource = downPosition;		
				}
				
				Camera.main.GetComponent<InGameState_Script>().state = InGameState_Script.InGameState.CORNER;
				
			}
		
		}	
		
	}
	
	
		
	
}
