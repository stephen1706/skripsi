using UnityEngine;
using System.Collections;

public class GoalKeeperJump : MonoBehaviour {//script dari box collider box yg didepan GK, buat detect klo bolany ud didepan kiper blm

	public GoalKeeper_Script goalKeeper;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	
	void OnTriggerEnter( Collider other ) {//klo bolany ud depan kiper
	
		// Box triggers are used to know if goalkeeper need to throw to catch the ball
		if ( other.tag == "Ball" ) {
					
			Vector3 dir_goalkeeper = goalKeeper.transform.forward;//dptin arah GK dgn cara cr arah forwardnya kmn
			Vector3 dir_ball = other.gameObject.GetComponent<Rigidbody>().velocity;//dptin v dari bola
			dir_ball.Normalize();//normalize bikin x^+y^+z^ = 1,jd diturunin angka"ny
			
			float det = Vector3.Dot( dir_goalkeeper, dir_ball );//dptin hasil cos dr sudut antara GK dan bola
						
//			Debug.Log("det " + Mathf.Acos(det) * 57.0f + " speed " + other.gameObject.GetComponent<Rigidbody>().velocity.magnitude);
			float degree = Mathf.Acos(det) * 57.0f;//dptin arah lompatnya GK,acos buat cari sudut antara GK ama bola
			
			if ( degree > 90.0f && degree < 270.0f && other.gameObject.GetComponent<Rigidbody>().velocity.magnitude > 5.0f && !other.gameObject.GetComponent<Rigidbody>().isKinematic) {//klo jarak bola cukup jauh,hrs lompat		
				//TODO blm ada code buat nepis bola
				if ( tag == "GoalKeeper_Jump_Left" ) {//klo bolanya kna trigger yg lompat kiri,berarti animasiny lompat kiri, set stateny lompat kiri jg
				
					goalKeeper.state = GoalKeeper_Script.GoalKeeper_State.JUMP_LEFT;
					goalKeeper.gameObject.animation.Play("goalkeeper_clear_left_up");
				
					
					Debug.Log("Left");
				}
	
				if ( tag == "GoalKeeper_Jump_Right" ) {
	
					goalKeeper.state = GoalKeeper_Script.GoalKeeper_State.JUMP_RIGHT;
					goalKeeper.gameObject.animation.Play("goalkeeper_clear_right_up");
					
					Debug.Log("Right");
	
				}
		
			}
		
		}
		
		
	}
	
}
