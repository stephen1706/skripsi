using UnityEngine;
using System;
using System.Collections;
public class Player_Script : MonoBehaviour {
	
	// player name
	public string Name;
	public TypePlayer type = TypePlayer.DEFENDER;
	public float Speed = 1.0f;	
	public float Strong = 1.0f;
	public float Control = 1.0f;
	public Vector3 attackingPos;
	
	private const float STAMINA_DIVIDER = 64.0f;
	private const float STAMINA_MIN = 0.5f;	
	private const float STAMINA_MAX = 1.0f;	
	
	public enum TypePlayer {
		DEFENDER,
		MIDDLER,
		ATTACKER
	};
	
	public Vector3 actualVelocityPlayer;
	private Vector3 oldVelocityPlayer;
	public Sphere sphere;
	private GameObject[] players;
	private GameObject[] oponents;
	public Vector3 resetPosition;
	public Vector3 initialPosition;
	public Vector3 kondisiAwal;
	private float inputSteer;
	private const float initialDisplacement = 20.0f;	
	public Transform goalPosition;
	public Transform headTransform;	
	[HideInInspector]	
	public bool temporallyUnselectable = true;
	[HideInInspector]	
	public float timeToBeSelectable = 1.0f;	
	public float maxDistanceFromPosition = 20.0f;	
	
	public enum Player_State { 
		PREPARE_TO_KICK_OFF,
		KICK_OFFER,
		RESTING,
		GO_ORIGIN,
		CONTROLLING,
		PASSING,
		SHOOTING,
		MOVE_AUTOMATIC,
		ONE_STEP_BACK,
		STOLE_BALL,
		OPONENT_ATTACK,
		PICK_BALL,
		CHANGE_DIRECTION,
		THROW_IN,
		CORNER_KICK,
		TACKLE,
		MARK_ENEMY,
		GET_BALL,
		TEMPORARY_RESTING
	};
	
	public Player_State state;
	private float timeToRemove = 3.0f;	
	private float timeToPass = 1.0f;
	
	// hand of player in sque	leton hierarchy
	public Transform hand_bone;
	
	public InGameState_Script inGame;
	
	public Texture barTexture;
	public Texture barStaminaTexture;
	private int barPosition=0;
	private Quaternion initialRotation;	
	
	public float stamina = 64.0f;	
	
	//changed
	public GameObject enemyMarked;
	public GameObject originMarked;
	public GameObject whoMarkedMe;
	public float timeToStopRest;

	void  Awake () {
		
		animation.Stop();
		if (gameObject.tag == "PlayerTeam1") {
			state = Player_State.PREPARE_TO_KICK_OFF;
		}

	}

	void  Start (){
		kondisiAwal = transform.position;
		players = GameObject.FindGameObjectsWithTag("PlayerTeam1");//tampung plaer dan oponent dlm array
		oponents = GameObject.FindGameObjectsWithTag("OponentTeam");
		
		
		resetPosition = new Vector3( transform.position.x, transform.position.y, transform.position.z );//set lokasi reset posisi pemaen
		
		if ( gameObject.tag == "PlayerTeam1" )
			initialPosition = new Vector3( transform.position.x, transform.position.y, transform.position.z+initialDisplacement ); 
		
		if ( gameObject.tag == "OponentTeam" )
			initialPosition = new Vector3( transform.position.x, transform.position.y, transform.position.z-initialDisplacement ); 
		
		animation["jump_backwards_bucle"].speed = 1.5f;
		animation["starting"].speed = 1.0f;
		animation["starting_ball"].speed = 1.0f;
		animation["running"].speed = 1.2f;
		animation["running_ball"].speed = 1.0f;
		animation["pass"].speed = 1.8f;
		animation["rest"].speed = 1.0f;
		animation["turn"].speed = 1.3f;
		animation["tackle"].speed = 3.0f;
		
		animation["fight"].speed = 1.2f;	
		
		animation.Play("rest");	
		
		initialRotation = transform.rotation * headTransform.rotation;
	}
	
//		void OnTriggerEnter(Collider other){//trigger bru buat ai defense doang
//			//Debug.Log ("in");
//			if (sphere.owner && other.gameObject.tag == "Ball" && sphere.owner.tag == "PlayerTeam1") {
//				state = Player_State.STOLE_BALL;
//			//TODO hrs limit org yg ngejer bola, klo ud ad jgn dikejer lg
//			} else if(gameObject.tag == "OponentTeam" && other.gameObject.tag == "PlayerTeam1"){
//				//todo klo zonal diemin orgny, klo man to man tempel
//				if(!enemyMarked && type == TypePlayer.DEFENDER){//klo bek kn kg ad yg di mark, jd klo ad yg msk lgsg di mark,sbnerny bs jg ke ujung zonalnya mendekati bola
//					enemyMarked = other.gameObject;
//				} else if(!enemyMarked && type == TypePlayer.ATTACKER){//striker kg perlu mark sapa", jd ttp kejer bola aj
//					return;
//				}
//				//enemyMarked = other.gameObject;
//				if(state != Player_State.STOLE_BALL){
//					state = Player_State.MARK_ENEMY;
//				}
//			} 
//		}
//		void OnTriggerExit(Collider other){
//			//Debug.Log ("out");
//			if (other.gameObject.tag == "Ball") {
//				//todo ngejer bola
//				state = Player_State.MOVE_AUTOMATIC;
//				//state = Player_State.MARK_ENEMY;
//			}else if(gameObject.tag == "OponentTeam" && other.gameObject.tag == "PlayerTeam1"){
//				//todo klo zonal diemin orgny, klo man to man tempel
//				if(other.gameObject == enemyMarked){//balik fokus ke awal
//					enemyMarked = originMarked;
//				}
//			}
//		}
	void Case_Controlling() {
		
		if ( sphere.inputPlayer == gameObject ) {//klo pemaen ini pmaen yg dikontrol user

			if ( sphere.fVertical != 0.0f || sphere.fHorizontal != 0.0f ) {//klo playernya ada input gerak
				
				oldVelocityPlayer = actualVelocityPlayer;
				
				Vector3 right = inGame.transform.right;//cari arah kiri kanan lapanganp berdasarkan kemana
				Vector3 forward = inGame.transform.forward;
				
				right *= sphere.fHorizontal;//gerakin player
				forward *= sphere.fVertical;
				
				Vector3 target = transform.position + right + forward;//set target gerak pemaen ini,targetnya itu sesuai input player x dan y digabung
				target.y = transform.position.y;
				
				float speedForAnimation = 5.0f;
				
				
				if ( sphere.owner == gameObject ) {//klo player ini pemegang bola
					
					if ( animation.IsPlaying("rest") ) {//ganti animasi rest/diem jd animasi mulai pegang bola
						animation.Play("starting_ball");
						speedForAnimation = 1.0f;
					}
					
					if ( animation.IsPlaying("starting_ball") == false )//klo start pegang bola animasinya abis,play running ball
						animation.Play("running_ball");
					
				}
				else {//klo pmaenny dikontrol user tapi ga pegang bola maenin animasi running biasa
					
					if ( animation.IsPlaying("rest") ) {
						animation.Play("starting");
						speedForAnimation = 1.0f;
					}
					
					if ( animation.IsPlaying("starting") == false )
						animation.Play("running");
					
				}
				
				
				transform.LookAt( target );//set arah gerak player yg dikontrol
				float staminaTemp = Mathf.Clamp ((stamina/STAMINA_DIVIDER), STAMINA_MIN ,STAMINA_MAX );
				actualVelocityPlayer = transform.forward*speedForAnimation*Time.deltaTime*staminaTemp*Speed;//set kecepetan player
				transform.position += actualVelocityPlayer;//gerakin pmaen maju sesuai kemiringan target
				
				float dotp = Vector3.Dot( oldVelocityPlayer.normalized, actualVelocityPlayer.normalized );//hasilny ini hasil mutlak dotproduct trs di cos, jd klo 0 itu tegak lurus, klo 1 itu sejajar, -1 itu berlawanan arah  
				
				if ( dotp < 0.0f && sphere.owner == gameObject ) {//klo pergerakan player bnyk bgt beloknya ksh animasi ubah arah
					
					animation.Play("turn");
					state = Player_State.CHANGE_DIRECTION;
					transform.forward = -transform.forward;
					sphere.owner = null;
					gameObject.GetComponent<CapsuleCollider>().enabled = false;
					sphere.gameObject.GetComponent<Rigidbody>().AddForce(  -transform.forward.x*1500.0f, -transform.forward.y*1500.0f, -transform.forward.z*1500.0f );
					
				}
				
				
			} else {//klo pemaenny ga dikotnrol user di suruh rest aja
				//TODO ksh AI gerakin otomatis temen user
				animation.Play("rest");
			}
			
			
			// klo tombol pass dipencet dan pemilik bola itu pemaen ini
			if ( sphere.bPassButton && sphere.owner == gameObject ) {
				animation.Play("pass");
				timeToBeSelectable = 2.0f;
				state = Player_State.PASSING;
				sphere.pressiPhonePassButton = false;
			}
			
			// shoot
			if ( sphere.bShootButtonFinished && sphere.owner == gameObject ) {
				animation.Play("shoot");
				timeToBeSelectable = 2.0f;
				state = Player_State.SHOOTING;
				sphere.pressiPhoneShootButton = false;
				sphere.bShootButtonFinished = false;
			}
			
			
			if ( sphere.bPassButton && sphere.owner != gameObject ) {
				animation.Play("tackle");
				//			timeToBeSelectable = 2.0f;
				state = Player_State.TACKLE;
				sphere.pressiPhonePassButton = false;
			}
			
			
			
		} else {//klo player ini kaga diselect, gerak otomatis
			
			state = Player_State.MOVE_AUTOMATIC;
			
		}
		
	}
	
	// cek klo ada pemaen di depannya
	bool SomeoneInFront( GameObject[] team_players ) {
		
		
		foreach( GameObject go in team_players ) {
			
			Vector3 relativePos = transform.InverseTransformPoint( go.transform.position ); 
			
			if ( relativePos.z > 0.0f )
				return true;		
		}
		
		return false;
		
	}
	
	
	// Oponent control
	void Case_Opponent_Attack() {

		actualVelocityPlayer = transform.forward*5.0f*Time.deltaTime;
		animation.Play("running_ball");
		Vector3 RelativeWaypointPosition = transform.InverseTransformPoint(goalPosition.position);
		inputSteer = RelativeWaypointPosition.x / RelativeWaypointPosition.magnitude;//cr arah gawang dimana
		transform.Rotate(0, inputSteer*10.0f , 0);//rotate player biar ke arah gawang,pdhl bs jg pk Mathf.Asin(inputsheer)
		float staminaTemp = Mathf.Clamp ((stamina/STAMINA_DIVIDER), STAMINA_MIN ,STAMINA_MAX );
		transform.position += transform.forward*4.0f*Time.deltaTime*staminaTemp*Speed;
		
		timeToPass -= Time.deltaTime;
		
		if ( timeToPass < 0.0f && SomeoneInFront(players) &&/* SomeoneInFront( oponents ) && */ detectDistanceToNearestEnemy() < 5.0f) {//klo ud saatnya passing dan msh ada musuh di depannya jd pass aja
			//timeToPass = UnityEngine.Random.Range( 1.0f, 5.0f);	//set wkt utk pass selanjutnya
			timeToPass = 0.5f;
			Debug.Log("harus pass");
			state = Player_State.PASSING;
			timeToBeSelectable = 1.0f;
			temporallyUnselectable = true;
		}
		
		float distance = (goalPosition.position - transform.position).magnitude;//cek jarak gawang musuh ama pmaen musuh
		Vector3 relative = transform.InverseTransformPoint(goalPosition.position);
		
		if ( distance < 20.0f && relative.z > 0 ) {//klo jaraknya ga gt jauh shoot aja
			//TODO bisa cek buat ad bek di depannya kaga
			state = Player_State.SHOOTING;
			animation.Play("shoot");
			timeToBeSelectable = 1.0f;
			temporallyUnselectable = true;
			
		}
		
	}
	
	void LateUpdate() {
		
		// turn head if necesary
		Vector3 relativePos = transform.InverseTransformPoint( sphere.gameObject.transform.position );
		
		if ( relativePos.z > 0.0f ) {
			
			Quaternion lookRotation = Quaternion.LookRotation (sphere.transform.position + new Vector3(0, 1.0f,0) - headTransform.position);
			headTransform.rotation = lookRotation * initialRotation ;			
			headTransform.eulerAngles = new Vector3( headTransform.eulerAngles.x, headTransform.eulerAngles.y,  -90.0f);
			
		}
		
	}
	
	void  Update() {
		//TODO harus buat enemy wkt nyerang nyebar, biar kaga ngerumun, trs tim kita jg jd ikt ngerumun krn saling ngejagain
		//TODO masalhnya 1bola bisa dipunyain ama 3org enemy krn berhimpit
		//Debug.Log ("State dari : " + name + " : " + state);
		//aaDebug.Log ("nearest dari " + name + " : " + detectDistanceToNearestEnemy ());
		//		if (sphere.owner) {
		//			Debug.Log ("owner : " + sphere.owner.tag);
		//		}else{
		//			Debug.Log ("owner : none");
		//		}
		if (inGame.isKickOff && state != Player_State.KICK_OFFER ) {
			state = Player_State.PREPARE_TO_KICK_OFF;
		}
		if (inGame.state == InGameState_Script.InGameState.PLAYING && (state == Player_State.PREPARE_TO_KICK_OFF || state == Player_State.KICK_OFFER)) {//changed
			//biar pemaenny abis kickoff kaga diem doang
			if(!inGame.isKickOff && name != "Player_Calvo_2")//player 2 soalny yg penerimany, biar ga lgsg lari
				state = Player_State.MOVE_AUTOMATIC ; 
		}
		
		stamina += 2.0f * Time.deltaTime;//tmbhin stamina buat player yg kaga lg dikontrol
		stamina = Mathf.Clamp(stamina, 1, 64);		//min 1,max 64,stamina ngaruh ke lari player
		
		switch ( state ) {
			
			
		case Player_State.PREPARE_TO_KICK_OFF:
			transform.LookAt( new Vector3(sphere.transform.position.x, transform.position.y, sphere.transform.position.z) );
			break;
			
			
		case Player_State.KICK_OFFER:
			
			if ( sphere.bPassButton || this.gameObject.tag == "OponentTeam" ) {//klo ini AI yg kickoff
				
				animation.Play("pass");//lgsg pass aja
				timeToBeSelectable = 2.0f;
				state = Player_State.PASSING;

				inGame.state = InGameState_Script.InGameState.PLAYING;
				inGame.isKickOff = false;
			}
			
			break;
			
		case Player_State.THROW_IN:
			
			break;
			
		case Player_State.CORNER_KICK:
			//klo abis corner suka ngebug kg mau balik lg orgny ikt maju
			break;
			
		case Player_State.CHANGE_DIRECTION:
			
			if ( !animation.IsPlaying("turn")) {//klo ud kelar turnnya
				gameObject.GetComponent<CapsuleCollider>().enabled = true;
				transform.forward = -transform.forward;
				animation.Play("running");//tdnya rest
				state = Player_State.CONTROLLING;
			}
			
			break;
			
			
		case Player_State.CONTROLLING:
			if ( gameObject.tag == "PlayerTeam1" ) //klo in player yg user lg kontrol
				Case_Controlling();			
			else if (gameObject.tag == "OponentTeam"){
				state = Player_State.OPONENT_ATTACK;
			}
			break;
			
		case Player_State.OPONENT_ATTACK://klo musuh yg lg attack
			if(sphere.owner == gameObject){
				Case_Opponent_Attack();		
			}else{
				state = Player_State.MOVE_AUTOMATIC;
			}	
			break;
			
			
		case Player_State.PICK_BALL://pick ball itu tackle diri
			transform.position += transform.forward * Time.deltaTime * 5.0f;
			
			if (animation.IsPlaying("fight") == false) {//klo ud kelar ambil bolanya mulai serang klo itu musuh
				
				if ( gameObject.tag == "OponentTeam" )
					state = Player_State.OPONENT_ATTACK;
				else
					state = Player_State.MOVE_AUTOMATIC;
				
			}
			
			break;
			
			
		case Player_State.SHOOTING:
			
			if (animation.IsPlaying("shoot") == false)//klo shoot ud kelar gerak otomatis lg
				state = Player_State.MOVE_AUTOMATIC;
			
			
			if (animation["shoot"].normalizedTime > 0.2f && sphere.owner == this.gameObject) {//wkt animasi ud 0.2 bagian, bola ud lepas dr player,ownerny jd kosong
				state = Player_State.MOVE_AUTOMATIC;
				sphere.owner = null;
				sphere.lastOwner = gameObject;

				if ( gameObject.tag == "PlayerTeam1" ) {
					sphere.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(transform.forward.x*30.0f, 5.0f, transform.forward.z*30.0f );
					barPosition = 0;//munculin shooting bar
					//TODO PERGUNAIN POWER SHOOT DI KOORDINAT Y,tp slalu 0 ntah knp lengthpressnya yg di class sphere
				}
				else {
					transform.LookAt (goalPosition);//liat ke arah gawang biar kg jauh bgt melesetnya
					
					float valueRndY = UnityEngine.Random.Range( 1.0f, 10.0f );//random tinggi tembakan musuh
					
					float valueRndX = UnityEngine.Random.Range(-5F,5F );//TADINYA FORWARD.X BIASA TP DIRANDOM BIAR BS OUT
					sphere.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(transform.forward.x*30.0f+valueRndX, valueRndY, transform.forward.z*30.0f );//kg pake right krn  right itu (1,0,0),sedangkan kita butuh x aja
				}
				
			}
			break;
			
		case Player_State.PASSING:
			if (animation.IsPlaying("pass") == false ){//biar bs bedain yg msh holdup ama ud kelar passing
				state = Player_State.MOVE_AUTOMATIC;
				collider.enabled = true;
			}
			
			if (sphere.owner == this.gameObject) {//wkt animasi ud 0.3 bagian, bola ud lepas dr player,ownerny jd kosong
				sphere.owner = null;
				sphere.lastOwner = gameObject;

				GameObject bestCandidatePlayer = null;
				float bestCandidateCoord = 1000.0f;
				
				
				if ( gameObject.tag == "PlayerTeam1" ) {//klo player pass bola
					
					foreach ( GameObject go in players ) {//cari kandidat buat nerima passing dr player
						
						if ( go != gameObject ) {
							Vector3 relativePos = transform.InverseTransformPoint( new Vector3( go.transform.position.x, go.transform.position.y, go.transform.position.z  ) );//cr jarak antara passer dan [enerima
							
							float magnitude = relativePos.magnitude;//cari jarak total
							float direction = Mathf.Abs(relativePos.x);//cari jarak posisi x,krn klo x-nya jauh jg ssh
							
							if ( relativePos.z > 0.0f && direction < 5.0f && magnitude < 15.0f && (direction < bestCandidateCoord) ) {//z>0 krn pstin posisiny didepan player
								bestCandidateCoord = direction;
								bestCandidatePlayer = go;
								
							}
						}
						
					}
					
				} else {//enemy
					
					foreach ( GameObject go in oponents ) {
						
						if ( go != gameObject ) {
							Vector3 relativePos = transform.InverseTransformPoint( new Vector3( go.transform.position.x, go.transform.position.y, go.transform.position.z  ) );
							
							float magnitude = relativePos.magnitude;
							float direction = Mathf.Abs(relativePos.x);
							float distanceWithMe = (go.transform.position - transform.position).magnitude;
							if ( /*relativePos.z > 0.0f && */direction < 25.0f && (magnitude+direction < bestCandidateCoord) 
							    && distanceWithMe > 5.0f  && distanceWithMe < 25.0f 
							    && calculateFriendDistanceToNearestEnemy(go) > 10.0f) {//syarat terakhir itu buat tmnny hrs bebas dr musuh jg
								if(bestCandidatePlayer){
									if(bestCandidatePlayer.transform.position.z > go.transform.position.z){
										bestCandidateCoord = magnitude+direction;
										bestCandidatePlayer = go;		
									}
								} else{
									bestCandidateCoord = magnitude+direction;
									bestCandidatePlayer = go;		
								}
							}
						}
					}

					if ( bestCandidateCoord == 1000.0f ) {//kalo ga ada yg bs dipass cr tmn terjauh
						Debug.Log ("ga ad yg bisa dipass");
						float min = bestCandidateCoord;
						foreach ( GameObject go in oponents ) {

							if ( go != gameObject ) {
								Vector3 relativePos = transform.InverseTransformPoint( new Vector3( go.transform.position.x, go.transform.position.y, go.transform.position.z  ) );
								float magnitude = relativePos.magnitude;
								float direction = Mathf.Abs(relativePos.x);

								if ( go.transform.position.z < min){//lbh dpn posisiny
									bestCandidateCoord = magnitude+direction;
									bestCandidatePlayer = go;	
									min =  go.transform.position.z;
								}
							}
						}
					}
				}
				
				if ( bestCandidateCoord != 1000.0f ) {
					Debug.Log(gameObject.name + " pass to " + bestCandidatePlayer.name);

//					GameObject s = sphere.GetComponent<Sphere>().gameObject;
//					BoxCollider bc = (BoxCollider) s.AddComponent("BoxCollider");
//					bc.center = sphere.transform.position;
//					bc.size = new Vector3(10,10,10);
					//coba bikin collider buat test bolany bkl diblok kaga, tp kykny malah kna jd throwin trs bolany melayang

					Vector3 relPos = transform.InverseTransformPoint( bestCandidatePlayer.transform.position );
					inputSteer = relPos.x / relPos.magnitude;
					transform.Rotate(0, inputSteer*20.0f , 0);
					collider.enabled = false;

					animation.Play("pass");
					sphere.inputPlayer = bestCandidatePlayer;//set pemaen yg diselected itu yg terdekat
					Vector3 directionBall = (bestCandidatePlayer.transform.position - transform.position).normalized;//itung arah bola biar bs ngarah ke penerima passer,caranya set sumbu x velocity bola sesuai direction
					float distanceBall = (bestCandidatePlayer.transform.position - transform.position).magnitude*1.0f;//itung jarak total bola dr sumbu x,y,z
					distanceBall = Mathf.Clamp( distanceBall, 15.0f, 40.0f );//jarak max passing 40
					sphere.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(directionBall.x*distanceBall, distanceBall/5.0f, directionBall.z*distanceBall );

				} else {
					//klo ga ada penerima yg layak (khusus player krn enemy klo ga ad tmn pst pass k yg plg dpn)
					sphere.gameObject.GetComponent<Rigidbody>().velocity = transform.forward*20.0f;
				}
					
			}
			break;
		case Player_State.GO_ORIGIN:
			if (type == TypePlayer.DEFENDER 
			    || (type == TypePlayer.ATTACKER && sphere.lastOwner.tag != gameObject.tag) //go origin klo last touchny emg dr td tim musuh
			    || inGame.state != InGameState_Script.InGameState.PLAYING) {//klo lg goalkick,dll ttp hrs balik
				animation.Play("running");
				Vector3 RelativeWaypointPosition = transform.InverseTransformPoint(new Vector3( 
				                                                                               initialPosition.x, 
				                                                                               initialPosition.y, 
				                                                                               initialPosition.z ) );//cari jarak initial position ama posisi player skrg
				
//				// by dividing the horizontal position by the magnitude, we get a decimal percentage of the turn angle that we can use to drive the wheels
//				inputSteer = RelativeWaypointPosition.x / RelativeWaypointPosition.magnitude;//x/magnitude itu buat cr arah rotasi ke tujuan pokokny
//				
//				if ( inputSteer == 0 && RelativeWaypointPosition.z < 0 )
//					inputSteer = 10.0f;
//				
//				transform.Rotate(0, inputSteer*10.0f , 0);

				transform.LookAt(initialPosition);
				float staminaTemp = Mathf.Clamp ((stamina/STAMINA_DIVIDER), STAMINA_MIN ,STAMINA_MAX );
				transform.position += transform.forward*3.0f*Time.deltaTime*staminaTemp*Speed;			
				//transform.position += transform.forward*3.0f*Time.deltaTime;
				
				if ( RelativeWaypointPosition.magnitude < 1.0f ) {//klo jarakny ud sgt dkt suruh move automatic aja
					state = Player_State.RESTING;	//changed hrsny move auto				
				}
				
			} // klo bkn bek/striker jgn prnh go back to irigin
			else {
				state = Player_State.MOVE_AUTOMATIC;
			}
			
			
			break;
			
		case Player_State.MOVE_AUTOMATIC:
			timeToRemove += Time.deltaTime;				
			float distance = (transform.position - initialPosition).magnitude;//itung jarak initial dan skrg
			
			// cr jarak player ke bola
			float distanceBall = (transform.position - sphere.transform.position).magnitude;
			float distanceToAttackingPos = (transform.position - attackingPos).magnitude;
			
			// klo jarak player jauh bgt dr posisi originnya, return ke posisi awal aja
			if ( distanceBall > maxDistanceFromPosition && type == TypePlayer.DEFENDER ) {
				//TODO klo ad bola lg kosong msk k zona bek, ttp kg diambil, malah go origin
				state = Player_State.GO_ORIGIN;
			} // klo jaraknya ga terlalu jauh,lari ke arah bola aja
			else {	
				bool hisTeamOnAttack = false;
				if(sphere.owner && tag ==  sphere.owner.tag) {
					hisTeamOnAttack = true;
				} 
				Debug.Log(name + "'s team is on attack : " + hisTeamOnAttack);
				if(!hisTeamOnAttack || attackingPos.z == 0){//klo attackingpos ga diset, atau lagi bertahan, kejer bola aj	
					if(enemyMarked != null && type != TypePlayer.ATTACKER){//klo pny musuh yg perlu dimark
						//aaDebug.Log(name + " marking " + enemyMarked.name);
						//diem aj klo emg ga lg nyerang ato keujung zona dktin bola, kecuali zonany di breach
						state = Player_State.MARK_ENEMY;
					} else{ // casenya attacker, ga perlu kejar bola
						if(sphere.lastOwner.tag != gameObject.tag){
							state = Player_State.GO_ORIGIN;
						} else {
							goToDestination(attackingPos); //klo striker ini abis pass/ shoot ttp hrs maju k attackpos
						}
						//Debug.Log(name + " going after ball");
					}
				}
				else {//klo bola lg ditim kita,pemaen lari ke tujuan masing"
					Debug.Log(name + " going to attacking pos (" + attackingPos.x + "," + attackingPos.z + ")");
					goToDestination(attackingPos);
				}
			}
			
			break;
			
			
			
		case Player_State.RESTING:
			transform.LookAt( new Vector3( sphere.GetComponent<Transform>().position.x, transform.position.y ,sphere.GetComponent<Transform>().position.z)  );
			animation.Play("rest"); 		  
			if(type != TypePlayer.DEFENDER && sphere.owner && sphere.owner.tag == gameObject.tag){
				state = Player_State.MOVE_AUTOMATIC;
			} else if(type == TypePlayer.DEFENDER && sphere.owner && sphere.owner == gameObject){//klo bekny kebetulan pegang bola, kyk lg KO ga blh diem aj
				state = Player_State.MOVE_AUTOMATIC;
			}
			break;

		case Player_State.TEMPORARY_RESTING://TODO masih bug bikin ngerumun
			if(sphere.owner && sphere.owner.tag == tag){
				state = Player_State.MOVE_AUTOMATIC;
				break;
			}

			if(timeToStopRest < 3.0f){
				transform.LookAt( new Vector3( sphere.GetComponent<Transform>().position.x, transform.position.y ,sphere.GetComponent<Transform>().position.z)  );
				animation.Play("rest"); 		  
			} else{
				timeToStopRest = 0;
				state = Player_State.MOVE_AUTOMATIC;
			}

			timeToStopRest += Time.deltaTime;
			break;

		case Player_State.ONE_STEP_BACK:
			
			if (animation.IsPlaying("jump_backwards_bucle") == false)//klo ud abis animasi mundurnya otomatis lg geraknya
				state = Player_State.MOVE_AUTOMATIC;
			
			transform.position -= transform.forward*Time.deltaTime*4.0f;	//lari mundur buat ksh jarak ke tmn
			
			break;
			
			
		case Player_State.STOLE_BALL:
			collider.enabled = true;

			bool otherPlayerAlsoStoleBall = false;
			if(tag == "OponentTeam"){
				foreach(GameObject oponent in oponents){
					if(oponent != gameObject && oponent.GetComponent<Player_Script>().state == Player_State.STOLE_BALL){
						otherPlayerAlsoStoleBall  = true;
						break;
					}
				}
			}

			if((sphere.owner == null || sphere.lastOwner.tag != gameObject.tag)){//tdmya ga pake ginian if tp jd ngebug kiperny ttp ngejjar walopun ud diambel tmnnya
				//TODO HARUS DICEK APAKAH UDA ADA TEMEN LAEN YG STOLE BALL JG
			
				Vector3 relPos = transform.InverseTransformPoint( sphere.transform.position );
				inputSteer = relPos.x / relPos.magnitude;
				transform.Rotate(0, inputSteer*20.0f , 0);
				
				animation.Play("running");
				float staminaTemp3 = Mathf.Clamp ((stamina/STAMINA_DIVIDER), STAMINA_MIN ,STAMINA_MAX );
				transform.position += transform.forward*4.5f*Time.deltaTime*staminaTemp3*Speed;
			} else{
				state = Player_State.MOVE_AUTOMATIC;
			}
			
			
			
			break;
			
	
		case Player_State.MARK_ENEMY://changed
			//TODO hrs cek biar kg kluar dr zonanya
			if(sphere.owner && sphere.owner.tag != tag){//mark enemy ngebug lari ditempat karena loop ke mark enemy trs move auto trs blk lg
				//cara cegah loop itu, jd klo emg bolanya ud bola ditim kita bru balikin ke move enemy
				Vector3 relPos2 = transform.InverseTransformPoint( enemyMarked.transform.position );
				inputSteer = relPos2.x / relPos2.magnitude;
				transform.Rotate(0, inputSteer*20.0f , 0);
				
				animation.Play("running");
				float staminaTemp4 = Mathf.Clamp ((stamina/STAMINA_DIVIDER), STAMINA_MIN ,STAMINA_MAX );
				transform.position += transform.forward*4.5f*Time.deltaTime*staminaTemp4*Speed;
			} else if(sphere.owner && sphere.owner.tag == tag) {
				state = Player_State.MOVE_AUTOMATIC;
			} else{
				Vector3 relPos2 = transform.InverseTransformPoint( enemyMarked.transform.position );
				inputSteer = relPos2.x / relPos2.magnitude;
				transform.Rotate(0, inputSteer*20.0f , 0);
				
				animation.Play("running");
				float staminaTemp4 = Mathf.Clamp ((stamina/STAMINA_DIVIDER), STAMINA_MIN ,STAMINA_MAX );
				transform.position += transform.forward*4.5f*Time.deltaTime*staminaTemp4*Speed;
			}
			break;
			
		case Player_State.TACKLE:
			
			if ( animation.IsPlaying("tackle") ) {
				
				transform.position += transform.forward * (Time.deltaTime * (1.0f-animation["tackle"].normalizedTime) * 10.0f);//makin akhir animasinya makin dikit majunya
				
			} else {//klo ud kelar tacklenya
				
				animation.Play ("rest");
				temporallyUnselectable = false;
				state = Player_State.MOVE_AUTOMATIC;
				
			}
			
			break;
			
			
		};
		
		
		// kurangin wkt unselectable tiap render
		timeToBeSelectable -= Time.deltaTime;
		
		if ( timeToBeSelectable < 0.0f )
			temporallyUnselectable = false;
		else
			temporallyUnselectable = true;
		
	}
	
	
	void OnCollisionStay( Collision coll ) {//saat player pegang bola
		
		if ( coll.collider.transform.gameObject.tag == "Ball" && !gameObject.GetComponent<Player_Script>().temporallyUnselectable ) {
			
			inGame.lastTouched = gameObject;//bikin status klo player yg pegang bola
			if ( state == Player_State.TACKLE ) {
				
				sphere.transform.position += transform.forward;
				
			}
			
			Vector3 relativePos = transform.InverseTransformPoint( sphere.gameObject.transform.position );
			
			if ( relativePos.y < 0.35f ) { //bola jd milik player ini klo bolanya ada di darat bkn di udara
				
				coll.rigidbody.rotation = Quaternion.identity;
				GameObject ball = coll.collider.transform.gameObject;
				ball.GetComponent<Sphere>().owner = gameObject;
				
				if ( gameObject.tag == "OponentTeam" ) {//klo ini enemy,suruh nyerang enemyny
					state = Player_Script.Player_State.OPONENT_ATTACK;
				}
				
				
			}
		}
		
	}
	
	void OnGUI() {
		
		if ( sphere.timeShootButtonPressed > 0.0f && sphere.inputPlayer == this.gameObject) {
			
			Vector3 posBar = Camera.main.WorldToScreenPoint( headTransform.position + new Vector3(0,0.8f,0) );//set posisi bar shot
			GUI.DrawTexture( new Rect( posBar.x-30, (Screen.height-posBar.y), barPosition, 10 ), barTexture );//buat munculin texturenya
			
			barPosition = (int)(sphere.timeShootButtonPressed * 128.0f);
			if ( barPosition >= 63 )//set max
				barPosition = 63;
			
		}
		
		if ( sphere.owner == this.gameObject ) {//draw bar stamina
			
			Vector3 posBar = Camera.main.WorldToScreenPoint( headTransform.position + new Vector3(0,1.0f,0) );
			GUI.DrawTexture( new Rect( posBar.x-30, (Screen.height-posBar.y), (int)stamina, 10 ), barStaminaTexture );
			stamina -= 1.5f * Time.deltaTime;
			
		}
		
	}

	void goToDestination (Vector3 destination){
		float distanceToDestination = (destination - transform.position).magnitude;
		Vector3 direction = (destination - transform.position).normalized;//klo jarak trs di normalized jg bs buat nentuin arah slaen x/magnitude
		Vector3 posFinal;
		if (destination == sphere.transform.position) {
			posFinal = initialPosition + (direction * maxDistanceFromPosition); //cari jarak terjauh yg dibolehin pmaen itu bwt lari k situ,dgn arah sesuai bola
		} else {
			posFinal = destination;
		}

		Vector3 relPos = transform.InverseTransformPoint( destination );
		inputSteer = relPos.x / relPos.magnitude;
		transform.Rotate(0, inputSteer*20.0f , 0);
		
		animation.Play("running");
		float staminaTemp3 = Mathf.Clamp ((stamina/STAMINA_DIVIDER), STAMINA_MIN ,STAMINA_MAX );
		transform.position += transform.forward*4.5f*Time.deltaTime*staminaTemp3*Speed;

//		Vector3 RelativeWaypointP = new Vector3 (posFinal.x, posFinal.y, posFinal.z);
//		if (distanceToDestination > 5.0f) {//lari trs ke arah bola
//			RelativeWaypointP = transform.InverseTransformPoint (new Vector3 (//itung jarak posisi terjauh yg dibolehin dgn posisi skrg
//			                                                                  posFinal.x, 
//			                                                                  posFinal.y, 
//			                                                                  posFinal.z));
//			
//		} else if (distanceToDestination < 5.0f && distanceToDestination > 2.0f) {
//			
//			// klo ud mulai dkt kita stop aja,jd relative waypoint diset = 0 dgn cr cari jarak diri sndiri ke diri sendiri
//			RelativeWaypointP = transform.InverseTransformPoint (new Vector3 (
//				transform.position.x, 
//				transform.position.y, 
//				transform.position.z));
//			
//		} else if (distanceToDestination < 2.0f) {//klo terlalu dkt pake animasi buat mundur ksh jarak buat yg pegang bola,enemy ga bkl lakuin ini kecuali wkt mw shoot,hrsny blok
//			animation.Play ("jump_backwards_bucle");
//			state = Player_State.ONE_STEP_BACK;
//			return;
//		}
//		
//		inputSteer = RelativeWaypointP.x / RelativeWaypointP.magnitude;
//		
//		if (inputSteer == 0 && RelativeWaypointP.z < 0)
//			inputSteer = 10.0f;
//		
//		if (inputSteer > 0.0f)
//			transform.Rotate (0, inputSteer * 20.0f, 0);
//		
//		
//		if (RelativeWaypointP.magnitude < 1.5f) {//cek klo posisi pmaen ud dkt
//			
//			transform.LookAt (new Vector3 (sphere.GetComponent<Transform> ().position.x, transform.position.y, sphere.GetComponent<Transform> ().position.z));//liat ke bola
//			animation.Play ("rest");		
//			timeToRemove = 0.0f;
//			
//		} else {			
//			
//			
//			if (timeToRemove > 1.0f) {					
//				animation.Play ("running");
//				float staminaTemp2 = Mathf.Clamp ((stamina / STAMINA_DIVIDER), STAMINA_MIN, STAMINA_MAX);
//				transform.position += transform.forward * 5.5f * Time.deltaTime * staminaTemp2 * Speed;
//			}
//		}
	}

	float detectDistanceToNearestEnemy(){
		float minDistance = 1000.0f;

		if (gameObject.tag == "PlayerTeam1") {
			foreach(GameObject oponent in oponents){
				float distance = (oponent.transform.position - transform.position).magnitude;
				if(distance < minDistance){
					minDistance = distance;
				}
			}
		} else if(gameObject.tag == "OponentTeam"){
			foreach(GameObject player in players){
				float distance = (player.transform.position - transform.position).magnitude;
				if(distance < minDistance){
					minDistance = distance;
				}
			}
		}
		return minDistance;
	}

	float calculateFriendDistanceToNearestEnemy(GameObject go){
		//TODO kliatannya msh ngebug
		float minDistance = 1000.0f;

		if (gameObject.tag == "PlayerTeam1") {
			foreach(GameObject oponent in oponents){
				float distance = (oponent.transform.position - go.transform.position).magnitude;
				if(distance < minDistance){
					minDistance = distance;
				}
			}
		} else if(gameObject.tag == "OponentTeam"){
			foreach(GameObject player in players){
				float distance = (player.transform.position - go.transform.position).magnitude;
				if(distance < minDistance){
					minDistance = distance;
				}
			}
		}
		return minDistance;
	}
}

