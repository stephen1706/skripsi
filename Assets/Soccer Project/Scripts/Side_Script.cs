using UnityEngine;
using System.Collections;

public class Side_Script : MonoBehaviour {
	
	public Sphere sphere;
	public Vector3 direction_throwin;
	
	// Use this for initialization
	void Start () {
		
		sphere = (Sphere)GameObject.FindObjectOfType( typeof(Sphere) );	//refer ke bola
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	

	void OnTriggerEnter( Collider other) {//trigger buat deteect klo pmaen kluar lapangan


		// Detect if Players are outside of field
		if ( (other.gameObject.tag == "PlayerTeam1" || other.gameObject.tag == "OponentTeam") && Camera.main.GetComponent<InGameState_Script>().state == InGameState_Script.InGameState.PLAYING ) {
		
			if ( other.gameObject != sphere.owner ) {//klo pemaen bukan pemilik bola,bikin pmaen kaga bs diselect slama 0.5dtk
			
				other.gameObject.GetComponent<Player_Script>().temporallyUnselectable = true;
				other.gameObject.GetComponent<Player_Script>().timeToBeSelectable = 0.5f;
				other.gameObject.GetComponent<Player_Script>().state = Player_Script.Player_State.GO_ORIGIN;//suruh balik ke posisi awal
			}
			
		}

		// Detect if Ball is outside
		if ( other.gameObject.tag == "Ball" && Camera.main.GetComponent<InGameState_Script>().state == InGameState_Script.InGameState.PLAYING ) {//klo bola yg out
			
			sphere.owner = null;//ga ada yg pegang bola
			Camera.main.GetComponent<InGameState_Script>().timeToChangeState = 2.0f;//set 2 dtk seblm throwin
			Camera.main.GetComponent<InGameState_Script>().state = InGameState_Script.InGameState.THROW_IN;//set state lg throwin
			Camera.main.GetComponent<InGameState_Script>().positionSide = sphere.gameObject.transform.position;		
			
		}
		
		
		
	}
	
	
}
