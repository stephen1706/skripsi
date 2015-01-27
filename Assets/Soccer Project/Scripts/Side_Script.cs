using UnityEngine;
using System.Collections;

public class Side_Script : MonoBehaviour {
	
	public Sphere sphere;
	public Vector3 direction_throwin;

	void Start () {
		
		sphere = (Sphere)GameObject.FindObjectOfType( typeof(Sphere) );	//refer ke bola
	}

	void Update () {
	
	}
	

	void OnTriggerEnter( Collider other) {
		//klo pmaen yg kluar lapangan, balikin ke posisi awalnya
		if ( (other.gameObject.tag == "PlayerTeam1" || other.gameObject.tag == "OponentTeam") && Camera.main.GetComponent<InGameState_Script>().state == InGameState_Script.InGameState.PLAYING ) {
		
			if ( other.gameObject != sphere.owner ) {//klo pemaen bukan pemilik bola,bikin pmaen kaga bs diselect slama 0.5dtk
			
				other.gameObject.GetComponent<Player_Script>().temporallyUnselectable = true;
				other.gameObject.GetComponent<Player_Script>().timeToBeSelectable = 0.5f;
				other.gameObject.GetComponent<Player_Script>().state = Player_Script.Player_State.GO_ORIGIN;//suruh balik ke posisi awal
			}
			
		}

		//klo bola kluar lapangan
		if ( other.gameObject.tag == "Ball" && Camera.main.GetComponent<InGameState_Script>().state == InGameState_Script.InGameState.PLAYING ) {//klo bola yg out
			
			sphere.owner = null;//ga ada yg pegang bola
			Camera.main.GetComponent<InGameState_Script>().timeToChangeState = 2.0f;//set 2 dtk seblm throwin
			Camera.main.GetComponent<InGameState_Script>().state = InGameState_Script.InGameState.THROW_IN;//set state lg throwin
			Camera.main.GetComponent<InGameState_Script>().positionSide = sphere.gameObject.transform.position;		
			
		}
		
		
		
	}
	
	
}
