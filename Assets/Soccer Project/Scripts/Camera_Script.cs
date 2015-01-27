using UnityEngine;
using System.Collections;

public class Camera_Script : MonoBehaviour {
	
	public Transform target;	//targetnya bola
	public Vector3 targetOffsetPos;//target offset itu jarak default dr bola ke kamera
	private Vector3 oldPos;	

	void Start () {
	
	}

	void LateUpdate () {//dijalanin telat stlh smuany kelar render, kyk kamera d tv kn emg ada jeda dlm ngikutin bola
		//klo throwin ga perlu diset kameranya krn kamera awal aj settingny,soalny wkt out jg ud kecenter lokasi outnya
		
		if ( GetComponent<InGameState_Script>().state == InGameState_Script.InGameState.PLAYING 
			|| GetComponent<InGameState_Script>().state == InGameState_Script.InGameState.GOAL_KICK_RUNNING
			|| GetComponent<InGameState_Script>().state == InGameState_Script.InGameState.GOAL_KICK_KICKING
		    || GetComponent<InGameState_Script>().state == InGameState_Script.InGameState.PREPARE_TO_KICK_OFF
		    //kalo gamenya lg maen
		    ) {
			oldPos = transform.position;
			Vector3 newPos = new Vector3( target.position.x+targetOffsetPos.x, target.position.y+targetOffsetPos.y, target.position.z+targetOffsetPos.z );
			//set posisi kamera iktin bola + offset(jaraknya)
			float lerpX =  Mathf.Lerp( oldPos.x, newPos.x,  0.05f );//hasil dr lerp = oldpos + 0.05*(new-old)
			float lerpY =  Mathf.Lerp( oldPos.y, newPos.y,  0.05f );//bikin kamera jd telat ikutin bolany soalny geserny cmn 5persen dr sebelumnya
			float lerpZ =  Mathf.Lerp( oldPos.z, newPos.z,  0.05f );
			
			transform.position = new Vector3( lerpX, lerpY, lerpZ );//lerp ini biar kyk broadcast kameranya, jd geraknya cmn kyk dirotate kamerany,kamerany kg gerak full	
			transform.LookAt( target );//rotate ngarah ke bola
		}
		
		if ( GetComponent<InGameState_Script>().state == InGameState_Script.InGameState.CORNER_CHASING
			|| GetComponent<InGameState_Script>().state == InGameState_Script.InGameState.CORNER_DOING) {//klo lg corner
		
		
			GameObject throwin_taker = GetComponent<InGameState_Script>().candidateToThrowIn;//cr pmaen yg ambl throw in
		
			if ( throwin_taker ) {
			
				transform.position = throwin_taker.transform.position - throwin_taker.transform.forward*15.0f + throwin_taker.transform.up*3.0f ;//set kamera jd diblkg dan sdkt diatas pmaen yg ambl throw in
				transform.LookAt( target );
			
			}
			
		
		}		
		
	}
}
