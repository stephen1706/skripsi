using UnityEngine;
using System.Collections;

public class GoalKeeper_Script : MonoBehaviour {

	public string Name;	
	
	public enum GoalKeeper_State { 
	   RESTING,
	   GO_ORIGIN,
	   STOLE_BALL,
	   GET_BALL_DOWN,
	   UP_WITH_BALL,
	   PASS_HAND,
	   GOAL_KICK,
	   JUMP_LEFT,
	   JUMP_RIGHT,
	   JUMP_LEFT_DOWN,
	   JUMP_RIGHT_DOWN	  
	};
	
	public GoalKeeper_State state;
	public Transform center_field;
	public Sphere sphere;
	public Vector3 initial_Position;
	public Transform hand_bone;
	private float timeToHoldBall = 1.0f;
	public CapsuleCollider capsuleCollider;	
	public float JumpSpeed;
	public float Speed;
	private float goalWidthFromCenter = 3.75f;
	private float maxPositionXToReachEdgeOfGoal = 15.0f;

	void Start () {
	
		initial_Position = transform.position;
		state = GoalKeeper_State.RESTING;
		animation["running"].speed = 1.0f;		
		animation["goalkeeper_clear_right_up"].speed = 1.0f;
		animation["goalkeeper_clear_left_up"].speed = 1.0f;
		animation["goalkeeper_clear_right_down"].speed = 1.0f;
		animation["goalkeeper_clear_left_down"].speed = 1.0f;
	
	}

	void Update () {

		switch (state) {
	
			case GoalKeeper_State.JUMP_LEFT:
				
				capsuleCollider.direction = 0;
			
				if ( animation["goalkeeper_clear_left_up"].normalizedTime < 0.45f  ) {//klo animasi blm 0.45 bagian,gerakin kiper ke kiri trs
//					transform.position -= transform.right * Time.deltaTime * 7.0f; //gerakin posisi kiper ke kiri
					transform.position = Vector3.Lerp(transform.position, sphere.transform.position,JumpSpeed*Time.deltaTime);

				}
			
			
				if ( !animation.IsPlaying("goalkeeper_clear_left_up") ) {//klo ud abis animasinya bikin kiper ambil bolanya
					state = GoalKeeper_State.STOLE_BALL;		
					capsuleCollider.direction = 1;

				}
			
			break;
	
			case GoalKeeper_State.JUMP_RIGHT:

				capsuleCollider.direction = 0;

				if ( animation["goalkeeper_clear_right_up"].normalizedTime < 0.45f  ) {
//					transform.position += transform.right * Time.deltaTime * 7.0f;
					transform.position = Vector3.Lerp(transform.position, sphere.transform.position,JumpSpeed*Time.deltaTime);

				}		
				if ( !animation.IsPlaying("goalkeeper_clear_right_up") ) {
					state = GoalKeeper_State.STOLE_BALL;		
					capsuleCollider.direction = 1; //1 itu artiny direction collidernya berdasarkan y-axis

				}
				
				
			break;
			
			case GoalKeeper_State.JUMP_LEFT_DOWN:
				
				
				capsuleCollider.direction = 0;
			
				if ( animation["goalkeeper_clear_left_down"].normalizedTime < 0.45f  ) {
//					transform.position -= transform.right * Time.deltaTime * 4.0f;
					transform.position = Vector3.Lerp(transform.position, sphere.transform.position,JumpSpeed*Time.deltaTime);

				}
			
			
				if ( !animation.IsPlaying("goalkeeper_clear_left_down") ) {
					state = GoalKeeper_State.STOLE_BALL;		
					capsuleCollider.direction = 1;

				}
			
			break;
	
			case GoalKeeper_State.JUMP_RIGHT_DOWN:

				capsuleCollider.direction = 0;

				if ( animation["goalkeeper_clear_right_down"].normalizedTime < 0.45f  ) {
//					transform.position += transform.right * Time.deltaTime * 4.0f;
					transform.position = Vector3.Lerp(transform.position, sphere.transform.position,JumpSpeed*Time.deltaTime);

				}		
				if ( !animation.IsPlaying("goalkeeper_clear_right_down") ) {
					state = GoalKeeper_State.STOLE_BALL;		
					capsuleCollider.direction = 1;

				}
				
				
			break;
						
			case GoalKeeper_State.GOAL_KICK:
		
			break;			
			
			case GoalKeeper_State.PASS_HAND:
		
				if ( animation["goalkeeper_throw_out"].normalizedTime < 0.65f && sphere.gameObject.GetComponent<Rigidbody>().isKinematic == true ) {//klo annimasi blm jln 0.65 bagian,posisi bola msh ditgn kiper
					sphere.gameObject.transform.position = hand_bone.position;
					sphere.gameObject.transform.rotation = hand_bone.rotation;
				}
		
				if ( animation["goalkeeper_throw_out"].normalizedTime > 0.65f && sphere.gameObject.GetComponent<Rigidbody>().isKinematic == true ) { //klo ud lwt 0.65 bagian
					sphere.gameObject.GetComponent<Rigidbody>().isKinematic = false;//set bolany kg kinematic lg,jd bs ketarik gravitasi
					sphere.gameObject.GetComponent<Rigidbody>().AddForce( transform.forward*5000.0f + new Vector3(0.0f, 1300.0f, 0.0f) );//gerakin bola maju
				//top velocity dari add force ini adalah = (reference http://answers.unity3d.com/questions/151724/calculating-rigidbody-top-speed.html)
				// float topVelocity = ((addedForce.magnitude / rigidbody.drag) - Time.fixedDeltaTime * addedForce.magnitude) / rigidbody.mass;

				}
		
				if ( !animation.IsPlaying("goalkeeper_throw_out") || !animation.IsPlaying("goalkeeper_throw_out") ) {//klo animasi ud abis
					state  = GoalKeeper_State.GO_ORIGIN;			//balikin posisi kiper ke awal
				}
			
			break;
			

			case GoalKeeper_State.UP_WITH_BALL:
			
			
				if ( !animation.IsPlaying("goalkeeper_catch_ball") ) {//klo bolanya ud ditangkep
				
					sphere.gameObject.GetComponent<Rigidbody>().isKinematic = true;
					sphere.gameObject.transform.position = hand_bone.position;
					sphere.gameObject.transform.rotation = hand_bone.rotation;
	
					timeToHoldBall -= Time.deltaTime;//timt to scara itu wkt pegang bola,lamany 1dtk
					
					if ( timeToHoldBall < 0.0f ) {
						timeToHoldBall = UnityEngine.Random.Range( 2.0f, 5.0f );
						animation.Play("goalkeeper_throw_out");
						state = GoalKeeper_State.PASS_HAND;//klo wkt pegang bola ud abis,lempar bola
					}
				
				
				} else {//klo bolanya lg ditangkep(animasi nangkep msh maen)
				
					sphere.gameObject.transform.position = hand_bone.position;
					sphere.gameObject.transform.rotation = hand_bone.rotation;

					transform.LookAt( center_field.position );//liat ke tngh lapangan
				
				
				}

			break;			
			
			case GoalKeeper_State.GET_BALL_DOWN:
			
				sphere.gameObject.transform.position = hand_bone.position;//set posisi bola jd posisi di tangan kiper krn ud diambil kiper bolanya
				sphere.gameObject.transform.rotation = hand_bone.rotation;
				
				if ( !animation.IsPlaying("goalkeeper_get_ball_front") ) {
					animation.Play("goalkeeper_catch_ball");
					state = GoalKeeper_State.UP_WITH_BALL;
				}
			
			break;
			
			
			case GoalKeeper_State.RESTING:
			
				capsuleCollider.direction = 1;
				if ( !animation.IsPlaying("goalkeeper_rest") )
					animation.Play("goalkeeper_rest");
				
				transform.LookAt( new Vector3( sphere.gameObject.transform.position.x, transform.position.y , sphere.gameObject.transform.position.z)  );
				//lebar stadium 75, lebar gawang 7.5, waktu uda 15 ud hrs mentok
				float destinationX = Mathf.Clamp(sphere.transform.position.x * goalWidthFromCenter / maxPositionXToReachEdgeOfGoal ,
				                                 -goalWidthFromCenter,
				                                 goalWidthFromCenter);
				transform.position = Vector3.Lerp(transform.position, 
				                                  new Vector3(destinationX,transform.position.y,transform.position.z),
				                                  Time.deltaTime);
				float distanceBall = (transform.position - sphere.gameObject.transform.position).magnitude;
		
				if ( distanceBall < 10.0f ) {//klo jarak bola ama kiper < 10 gerakin kiper buat ambl bola
					state = GoalKeeper_Script.GoalKeeper_State.STOLE_BALL;
				} 
			
			
			break;

			case GoalKeeper_State.STOLE_BALL:
				animation.Play("running");

				Vector3 RelativeWaypointPosition = transform.InverseTransformPoint( sphere.gameObject.transform.position );//cari jarak kiper ama bola
	
				float inputSteer = RelativeWaypointPosition.x / RelativeWaypointPosition.magnitude;//bagi jarak x ama jarak total x,y,z

				transform.Rotate(0, inputSteer*10.0f , 0);//rotate kiper sejauh x/total jarak
				transform.position += transform.forward*4.5f*Speed*Time.deltaTime;//gerakin kiper maju kearah bola krn ud dirotate,jd tinggal fwd

			break;
	
	
			case GoalKeeper_State.GO_ORIGIN://blk kearah awal,cara kerja sama ama stole ball,cmn beda targetnya

				animation.Play("running");
				RelativeWaypointPosition = transform.InverseTransformPoint( initial_Position );
	
				inputSteer = RelativeWaypointPosition.x / RelativeWaypointPosition.magnitude;
			
				transform.Rotate(0, inputSteer*10.0f, 0);
				transform.position += transform.forward*4.5f*Speed*Time.deltaTime;

		
				if ( RelativeWaypointPosition.magnitude < 1.0f ) {//klo ud dkt bgt ganti statenya jd rest
			
					state = GoalKeeper_State.RESTING;					
				}
			break;
		}
		
	}

	void OnCollisionStay( Collision coll ) {//saat GK msh pegang bola, is triggerny off biar GK bisa collide ama efek rigidbody kiperny, ntr bs mental krn ga mgkn ada oncollisionstay klo gt
		//klo on trigger true, callbackny ontriggerstay bkn oncollisionstay
		if ( Camera.main.GetComponent<InGameState_Script>().state == InGameState_Script.InGameState.PLAYING ) {
		
			if ( coll.collider.transform.gameObject.tag == "Ball" && state != GoalKeeper_State.UP_WITH_BALL && state != GoalKeeper_State.PASS_HAND && state != GoalKeeper_State.GOAL_KICK &&
				 state != GoalKeeper_State.JUMP_LEFT && state != GoalKeeper_State.JUMP_RIGHT &&
				 state != GoalKeeper_State.JUMP_LEFT_DOWN && state != GoalKeeper_State.JUMP_RIGHT_DOWN) {//klo GK pegang bola
							
				Camera.main.GetComponent<InGameState_Script>().lastTouched = gameObject;//set yg pegang bola itu kiper
				sphere.lastOwner = gameObject;	
	
				Vector3 relativePos = transform.InverseTransformPoint( sphere.gameObject.transform.position );

				if ( relativePos.y < 0.35f ) { //klo bolany < 0.35 dari GK ambil bolanya, slaennya tinggalin
				
					sphere.owner = null;
		
					animation.Play("goalkeeper_get_ball_front");
					state = GoalKeeper_State.GET_BALL_DOWN;
					
				}
				
				
			}
		
		}
		
	}
	
}
