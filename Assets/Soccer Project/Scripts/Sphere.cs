
using UnityEngine;
using System.Collections;


public class Sphere : MonoBehaviour {
	
	public GameObject owner;	// player yg pegang bola
	public GameObject inputPlayer;	// pemaen yg aktif dikontrol user
	public GameObject lastInputPlayer;	// pmaen aktif terakhir yg dikontrol user
	private GameObject[] players;
	private GameObject[] oponents;
	public Transform shadowBall;
	public Transform blobPlayerSelected;
	public float timeToSelectAgain = 0.0f;
	public GameObject lastCandidatePlayer;
	public ArrayList  whoMarkedMe;
	[HideInInspector]	
	public float fHorizontal;//Vx bola
	[HideInInspector]	
	public float fVertical;//Vy bola
	[HideInInspector]	
	public bool bPassButton;
	[HideInInspector]	
	public bool bShootButton;
	[HideInInspector]
	public bool bShootButtonFinished;
	[HideInInspector]		
	public bool pressiPhoneShootButton = false;
	[HideInInspector]	
	public bool pressiPhonePassButton = false;
	[HideInInspector]	
	public bool pressiPhoneShootButtonEnded = false;
	
	public Joystick_Script joystick;	
	public InGameState_Script inGame;
	public float timeShootButtonPressed = 0.0f;

	public GameObject lastOwner;

	void Start () {
		whoMarkedMe = new ArrayList ();
		players = GameObject.FindGameObjectsWithTag("PlayerTeam1");		
		oponents = GameObject.FindGameObjectsWithTag("OponentTeam");
		joystick = GameObject.FindGameObjectWithTag("joystick").GetComponent<Joystick_Script>();
		inGame = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<InGameState_Script>();
		blobPlayerSelected = GameObject.FindGameObjectWithTag("PlayerSelected").transform;		
	}


	void LateUpdate() {
	
		shadowBall.position = new Vector3( transform.position.x, 0.35f ,transform.position.z );
		shadowBall.rotation = Quaternion.identity;

	}

	void Update () {
		fVertical = Input.GetAxis("Vertical");
		fHorizontal = Input.GetAxis("Horizontal");
		fVertical += joystick.position.y;
		fHorizontal += joystick.position.x;

		bPassButton = Input.GetKey(KeyCode.Space) || pressiPhonePassButton;
		bShootButton = Input.GetKey(KeyCode.LeftControl) || pressiPhoneShootButton;


		if ( Input.GetKeyUp(KeyCode.LeftControl) || pressiPhoneShootButtonEnded) {
			
			bShootButtonFinished = true;
		}
		
		
		if ( bShootButton ) {
			timeShootButtonPressed += Time.deltaTime;
		
		} else {
			timeShootButtonPressed = 0.0f;
		}
				
	
		if ( owner ) {//klo bolany pny owner, bikin biar bolany nempel dikaki player
			
			lastOwner = owner;

	 		transform.position = owner.transform.position + owner.transform.forward/1.5f + owner.transform.up/5.0f; //set posisi bola jd nempel ama pemiliknya
			float velocity = owner.GetComponent<Player_Script>().actualVelocityPlayer.magnitude;//iktin speed pemiliknya
			
			if ( fVertical == 0.0f && fHorizontal == 0.0f  && owner.tag == "PlayerTeam1" ) {//klo pemiliknya player dan pemiliknya lg diem
				velocity = 0.0f;
				gameObject.GetComponent<Rigidbody>().angularVelocity = new Vector3(0,0,0);
			}
			
			transform.RotateAround( owner.transform.right, velocity*10.0f );//rotatearound buat rotate bendanya
		}		
		
		
		
		if ( inGame.state ==  InGameState_Script.InGameState.PLAYING ) {
			
			ActivateNearestPlayer();
	
			if ( !owner || owner.tag == "PlayerTeam1" ){
				ActivateNearestOpponent(); //TEMPORARILY OFF, KARENA UD PK ZONE TRIGGER, bikin ancur klo diaktifin,org jd lari ditempat
			
			}
		
		}
		
	}

	// activate nearest oponent to ball;
	void ActivateNearestOpponent() {//klo bolanya ga ada yg pny suruh cpu buat ngejer bolanya
	
		float distance = 100000.0f;
		GameObject candidatePlayer = null;
		foreach ( GameObject oponent in oponents ) {			
			
			if ( !oponent.GetComponent<Player_Script>().temporallyUnselectable ) {//pemaenny hrs dlm keadaan selectable,kg abis shoot,pass,dll
				
				//oponent.GetComponent<Player_Script>().state = Player_Script.Player_State.MOVE_AUTOMATIC;//ini penyebab errorny kykny
				//ini dimatiin biar kg error
				Vector3 relativePos = transform.InverseTransformPoint( oponent.transform.position );//set relative pos bola dari global jd local(berdasarkan jarak bola dgn musuh)
				
				float newdistance = relativePos.magnitude;//itung jaraknya berdasarkan x^2+y^2+z^2
				
				if ( newdistance < distance ) {//cari yg paling kcl jaraknya
				
					distance = newdistance;
					candidatePlayer = oponent;
					
				}
			}
			
		}

		if ( candidatePlayer )//suruh org terdekat kejer bolany
			candidatePlayer.GetComponent<Player_Script>().state = Player_Script.Player_State.STOLE_BALL;//ganti state itu org jd mau ngejer bola
		
		
	}

	void ActivateNearestPlayer() {//ini buat select player plg dkt buat kejer bola,jadiin pmaennya yg selected skrg
		
		lastInputPlayer = inputPlayer;
		
		float distance = 1000000.0f;
		GameObject candidatePlayer = null;
		foreach ( GameObject player in players ) {			
			
			if ( !player.GetComponent<Player_Script>().temporallyUnselectable ) {
				
				Vector3 relativePos = transform.InverseTransformPoint( player.transform.position );
				
				float newdistance = relativePos.magnitude;
				
				if ( newdistance < distance ) {
				
					distance = newdistance;
					candidatePlayer = player;
					
				}
			}
			
		}
		
		timeToSelectAgain += Time.deltaTime;
		if ( timeToSelectAgain > 0.5f ) {
			inputPlayer = candidatePlayer;
			timeToSelectAgain = 0.0f;
		} else {
			candidatePlayer = lastCandidatePlayer;
		}
		
		lastCandidatePlayer = candidatePlayer;
		
		
		if ( inputPlayer != null && candidatePlayer ) {
			blobPlayerSelected.transform.position = new Vector3( candidatePlayer.transform.position.x, candidatePlayer.transform.position.y+0.1f, candidatePlayer.transform.position.z);
			blobPlayerSelected.transform.LookAt( new Vector3( blobPlayerSelected.position.x + fHorizontal, blobPlayerSelected.position.y, blobPlayerSelected.position.z + fVertical  ) );//lookat buat rotate pmaen ke target
		
			// klo orgny ga lg ngap", jd control aj
			if ( inputPlayer.GetComponent<Player_Script>().state != Player_Script.Player_State.PASSING &&
			     inputPlayer.GetComponent<Player_Script>().state != Player_Script.Player_State.SHOOTING &&
			     inputPlayer.GetComponent<Player_Script>().state != Player_Script.Player_State.PICK_BALL &&
			     inputPlayer.GetComponent<Player_Script>().state != Player_Script.Player_State.CHANGE_DIRECTION &&
			     inputPlayer.GetComponent<Player_Script>().state != Player_Script.Player_State.TACKLE){

				inputPlayer.GetComponent<Player_Script>().state = Player_Script.Player_State.CONTROLLING;
			}
		} 
	}
}
