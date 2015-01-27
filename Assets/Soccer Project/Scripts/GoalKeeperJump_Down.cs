using UnityEngine;
using System.Collections;

public class GoalKeeperJump_Down : MonoBehaviour {//sama ama yg jump up,cmn beda di play animasi yg mana

	public GoalKeeper_Script goalKeeper;
	void Start () {
	
	}

	void Update () {
	
	}
	
	
	void OnTriggerEnter( Collider other ) {
		if ( other.tag == "Ball" ) {
		
			Vector3 dir_goalkeeper = goalKeeper.transform.forward;
			Vector3 dir_ball = other.gameObject.GetComponent<Rigidbody>().velocity;
			dir_ball.Normalize();
			
			float det = Vector3.Dot( dir_goalkeeper, dir_ball );
						
			Debug.Log("det " + Mathf.Acos(det) * 57.0f + " speed " + other.gameObject.GetComponent<Rigidbody>().velocity.magnitude);
			float degree = Mathf.Acos(det) * 57.0f;
			
			if ( degree > 90.0f && degree < 270.0f && other.gameObject.GetComponent<Rigidbody>().velocity.magnitude > 5.0f && !other.gameObject.GetComponent<Rigidbody>().isKinematic) {			

				if ( tag == "GoalKeeper_Jump_Left" ) {
				
					goalKeeper.state = GoalKeeper_Script.GoalKeeper_State.JUMP_LEFT_DOWN;
					goalKeeper.gameObject.animation.Play("goalkeeper_clear_left_down");
				
					
//					Debug.Log("Left");
				}
	
				if ( tag == "GoalKeeper_Jump_Right" ) {
	
					goalKeeper.state = GoalKeeper_Script.GoalKeeper_State.JUMP_RIGHT_DOWN;
					goalKeeper.gameObject.animation.Play("goalkeeper_clear_right_down");
					
//					Debug.Log("Right");
	
				}
		
			}
		
		}
		
		
	}
	
}
