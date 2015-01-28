using UnityEngine;
using System.Collections;
using System.Xml;
using System.IO;

public class InGameState_Script : MonoBehaviour {

	
	public enum InGameState {

		PLAYING,
		PREPARE_TO_KICK_OFF,
		KICK_OFF,
		GOAL,
		THROW_IN,
		THROW_IN_CHASING,
		THROW_IN_DOING,
		THROW_IN_DONE,
		CORNER,
		CORNER_CHASING,
		CORNER_DOING,
		CORNER_DOING_2,
		CORNER_DONE,
		GOAL_KICK,
		GOAL_KICK_RUNNING,
		GOAL_KICK_KICKING
		
	};
	

	public Material localTeam;
	public Material visitTeam;

	public Player_Script passer;
	public Player_Script passed;
	public Player_Script passer_opponent;
	public Player_Script passed_opponent;

	public bool scoredByLocal = false;
	public bool scoredByVisiting = true;

	public InGameState state;
	private GameObject[] players;
	private GameObject[] opponents;
	private GameObject keeper;
	private GameObject keeper_opponent;
	public GameObject lastTouched;
	public float timeToChangeState = 0.0f;
	public Vector3 positionSide;
	public Sphere sphere;
	public Transform center;
	public Vector3 targetThrowIn;
	private GameObject whoLastTouched;
	public GameObject candidateToThrowIn;
	private float timeToThrowInByOpponent = 3.0f;
	
	public Transform cornerSource;
	public GameObject areaCorner;
	public Transform goal_kick;
	public GameObject goalKeeper;
	public GameObject cornerTrigger;
	
	public Mesh[] meshes;
	public Material[] mat;
	
	private float timeToKickOff = 4.0f;
	public GameObject lastCandidate = null;
	
	public int localScore = 0;
	public int visitingScore = 0;
	
	public GameObject[] playerPrefab;
	public GameObject goalKeeperPrefab;
	public GameObject ballPrefab;

	public Transform targetOpponentGoal;

	public ScorerTimeHUD scorerTime;
	public int bFirstHalf = 0;	

	public Material localMaterial;
	public Material visitMaterial;

	public bool isKickOff;

	
	void Awake() {
		isKickOff = true;
	
	}

	void Start () {

		players = GameObject.FindGameObjectsWithTag("PlayerTeam1");
		opponents = GameObject.FindGameObjectsWithTag("OponentTeam");
		keeper = GameObject.FindGameObjectWithTag("GoalKeeper");
		keeper_opponent = GameObject.FindGameObjectWithTag("GoalKeeper_Oponent");
		
		state = InGameState.PREPARE_TO_KICK_OFF;

		bFirstHalf = 0;	

		LoadTeams ();
	}

	void LoadTeams ()//load texture warna pemaen dr preference, get pathnya dari resources/textures folder
	{
		localMaterial.mainTexture = Resources.Load ("Textures/" + "player_" + PlayerPrefs.GetString ("Local") + "_texture") as Texture2D;
		visitMaterial.mainTexture = Resources.Load ("Textures/" + "player_" + PlayerPrefs.GetString ("Visit") + "_texture") as Texture2D;
	}	
	

	double findMostBackEnemy(){
		double maxZ = 0;
		int i = 0;
		foreach (GameObject opponent in opponents) {
			//Debug.Log (i + " : " + opponent.transform.position.z);
			if(opponent.transform.position.z > maxZ){
				maxZ = opponent.transform.position.z;
			}
			i++;
		}
		return maxZ;
	}

	void Update () {
	
		// kurangin wkt seblm ganti state, timetostatechange dipake buat reset game stlh kejebol jd kickoff, biar ada jeda, corner, throw in jg
		timeToChangeState -= Time.deltaTime;
		
		if ( timeToChangeState < 0.0f ) {//saatny ganti state klo timernya abis

			switch (state) {
				
			case InGameState.PLAYING:
				isKickOff = false;//
				if(sphere.owner && sphere.owner.tag == "PlayerTeam1"){//changed
					double maxZ = findMostBackEnemy();
					bool isOffside = false;
					foreach (GameObject player in players){
						//Debug.Log("pos player : " + player.transform.position.z);
						if(/*sphere.owner != player && */player.transform.position.z > maxZ){
								isOffside = false; 
								break;
						}
					}
					//Debug.Log("pos max : " + maxZ);
					if(isOffside){
						Debug.Log ("offside");
					}
				}

				if ( scorerTime.minutes > 44.0f && bFirstHalf == 0 ) {

					bFirstHalf = 1;
				}
				
				if ( scorerTime.minutes > 45.0f && bFirstHalf == 1 ) {//klo ud half time

					sphere.transform.position = center.position;//bola ketengahin

					foreach ( GameObject player in players) {//reset posisi player
						
						player.transform.position = player.GetComponent<Player_Script>().resetPosition;
						player.animation.Play("rest");
						
					}

					foreach ( GameObject player in opponents) {//musuh di reset jg
						
						player.transform.position = player.GetComponent<Player_Script>().resetPosition;
						player.animation.Play("rest");

					}

					bFirstHalf = 2;//set babak kedua

					scoredByLocal = true;//buat set babak kedua yg kickoff musuhny
					scoredByVisiting = false;
					state = InGameState.PREPARE_TO_KICK_OFF;
					isKickOff = true;//

				}

				if ( scorerTime.minutes > 90.0f && bFirstHalf == 2) {//klo abis babaknya

					PlayerPrefs.SetInt("ScoreLocal", localScore );
					PlayerPrefs.SetInt("ScoreVisit", visitingScore );

					Application.LoadLevel( "Select_Team" );//klaurin pilih tim

				}


				break;
	
				case InGameState.THROW_IN:
				
					whoLastTouched = lastTouched;	
				
					foreach ( GameObject go in players ) {//wkt throw in bikin posisi pmaen jd idle
						go.GetComponent<Player_Script>().state = Player_Script.Player_State.RESTING;
					}
					foreach ( GameObject go in opponents ) {
						go.GetComponent<Player_Script>().state = Player_Script.Player_State.RESTING;
					}
				
				
					sphere.owner = null;
				
					if ( whoLastTouched.tag == "PlayerTeam1" )//klo player yg bikin out,cari musuh terdkt bwt throwin
						candidateToThrowIn = SearchPlayerNearBall( opponents );
					else	
						candidateToThrowIn = SearchPlayerNearBall( players );
				
				
					candidateToThrowIn.transform.position = new Vector3( positionSide.x, candidateToThrowIn.transform.position.y, positionSide.z);//set posisi thrower
				
					if ( whoLastTouched.tag == "PlayerTeam1" ) {
					
						candidateToThrowIn.GetComponent<Player_Script>().temporallyUnselectable = true;
						candidateToThrowIn.GetComponent<Player_Script>().timeToBeSelectable = 1.0f;//bikin oponent yg ambil throwin jd ga bs diselect
				
						candidateToThrowIn.transform.LookAt( SearchPlayerNearBall( opponents ).transform.position);//set posisi pemaen yg throw in jd ngadep ke tmn terdekat
					}
					else
						candidateToThrowIn.transform.LookAt( center ); //klo player yg throwin bikin jd ketengah aj liatnya
	
				
					candidateToThrowIn.transform.Rotate(0, sphere.fHorizontal*10.0f, 0);//rotate posisi thrower berdasarkan Vx bola
					candidateToThrowIn.GetComponent<Player_Script>().state = Player_Script.Player_State.THROW_IN;
				
					sphere.GetComponent<Rigidbody>().isKinematic = true;//biar bolany ga ketarik gravitasi
					sphere.gameObject.transform.position = candidateToThrowIn.GetComponent<Player_Script>().hand_bone.position;//set posisi bola ditangan throwin taker
				
					targetThrowIn = candidateToThrowIn.transform.position + candidateToThrowIn.transform.forward;//target buat throwin itu tujuan pmaen bkl lemparnya
				
				
					candidateToThrowIn.animation.Play("throw_in");//start animasi mau throwin, biar gayanya ud mulai tp blm gerak
					candidateToThrowIn.animation["throw_in"].time = 0.1f;
					candidateToThrowIn.animation["throw_in"].speed = 0.0f;
				
					state = InGameState.THROW_IN_CHASING;
				
				break;
				case InGameState.THROW_IN_CHASING://stelah thowin seblm throwin done,buat play animasi
				

					candidateToThrowIn.transform.position = new Vector3( positionSide.x, candidateToThrowIn.transform.position.y, positionSide.z);
					candidateToThrowIn.transform.LookAt( targetThrowIn );
					candidateToThrowIn.GetComponent<Player_Script>().state = Player_Script.Player_State.THROW_IN;
				
					sphere.GetComponent<Rigidbody>().isKinematic = true;
					sphere.gameObject.transform.position = candidateToThrowIn.GetComponent<Player_Script>().hand_bone.position;

					if ( whoLastTouched.tag != "PlayerTeam1" ) {//klo player yg ambl throwin
				
						targetThrowIn += new Vector3( 0,0,sphere.fHorizontal/10.0f);
					
						if (sphere.bPassButton) {//klo tombol pass dipencet lempar bolanya
							candidateToThrowIn.animation.Play("throw_in");
							state = InGameState.THROW_IN_DOING;
		
						}
						
					} else {
					
						timeToThrowInByOpponent -= Time.deltaTime;
					
						if ( timeToThrowInByOpponent < 0.0f ) {//klo wkt bwt nunggu bwt throw abis,lempar bolany otomatis
							timeToThrowInByOpponent = 3.0f;
							sphere.gameObject.GetComponent<Rigidbody>().isKinematic = true;
							candidateToThrowIn.animation.Play("throw_in");
							state = InGameState.THROW_IN_DOING;
						}
					
					}
				
				break;	
				
				case InGameState.THROW_IN_DOING://buat pmaen yg lg lempar bola
					
					candidateToThrowIn.animation["throw_in"].speed = 1.0f;

					if ( candidateToThrowIn.animation["throw_in"].normalizedTime < 0.5f && sphere.gameObject.GetComponent<Rigidbody>().isKinematic == true ) {
						//wkt msh stngh jalan animasiny,posisi bola msh iktin posisi tgn pelempar
						sphere.gameObject.transform.position = candidateToThrowIn.GetComponent<Player_Script>().hand_bone.position;
					}

					if ( candidateToThrowIn.animation["throw_in"].normalizedTime >= 0.5f && sphere.gameObject.GetComponent<Rigidbody>().isKinematic == true ) {
						//wkt ud lepas dr tgn bikin bola ketarik gravitasi dan gerak maju
						sphere.gameObject.GetComponent<Rigidbody>().isKinematic = false;
						sphere.gameObject.GetComponent<Rigidbody>().AddForce( candidateToThrowIn.transform.forward*4000.0f + new Vector3(0.0f, 1300.0f, 0.0f) );					
					} 
				
				
				
					if ( candidateToThrowIn.animation.IsPlaying("throw_in") == false ) {//klo animasi abis,ganti state jd throwin done
						state = InGameState.THROW_IN_DONE;
					}
				
				
				break;

				case InGameState.THROW_IN_DONE:
					candidateToThrowIn.GetComponent<Player_Script>().state = Player_Script.Player_State.MOVE_AUTOMATIC;//set thrower jd awal klo ud kelar throwin
					state = InGameState.PLAYING;
				
				break;
				
				
				
				
				case InGameState.CORNER:
				
					whoLastTouched = lastTouched;	
				
					if ( whoLastTouched.tag == "GoalKeeper_Oponent" )//pastiin klo kiper plyaer jd player yg kna
						whoLastTouched.tag = "OponentTeam";
					if ( whoLastTouched.tag == "GoalKeeper" )
						whoLastTouched.tag = "PlayerTeam1";
				

				
					if ( cornerTrigger.tag == "Corner_Oponent" && whoLastTouched.tag == "PlayerTeam1") {//klo yg last touch player tp out di gawang oponent berarti goal kick
						state = InGameState.GOAL_KICK;
						break;
					}
					if ( cornerTrigger.tag != "Corner_Oponent" && whoLastTouched.tag == "OponentTeam" ) {//bgitu jg sebaliknya
						state = InGameState.GOAL_KICK;
						break;
					}
				
				
				
				
					foreach ( GameObject go in players ) {
						go.GetComponent<Player_Script>().state = Player_Script.Player_State.RESTING;
					}
					foreach ( GameObject go in opponents ) {
						go.GetComponent<Player_Script>().state = Player_Script.Player_State.RESTING;
					}
				
				
					sphere.owner = null;
				
					if ( whoLastTouched.tag == "PlayerTeam1" ) {//klo oponent yg corner,gerakin pmaen buat bertahan
						PutPlayersInCornerArea( players, Player_Script.TypePlayer.DEFENDER );
						PutPlayersInCornerArea( players, Player_Script.TypePlayer.MIDDLER );
						PutPlayersInCornerArea( opponents, Player_Script.TypePlayer.ATTACKER );
						PutPlayersInCornerArea( opponents, Player_Script.TypePlayer.MIDDLER );
						candidateToThrowIn = SearchPlayerNearBall( opponents );//cari oponent buat ambil corner
					}
					else {	
						PutPlayersInCornerArea( opponents, Player_Script.TypePlayer.DEFENDER );
						PutPlayersInCornerArea( opponents, Player_Script.TypePlayer.MIDDLER );
						PutPlayersInCornerArea( players, Player_Script.TypePlayer.ATTACKER );
						PutPlayersInCornerArea( players, Player_Script.TypePlayer.MIDDLER );
						candidateToThrowIn = SearchPlayerNearBall( players );
					}				
				
					candidateToThrowIn.transform.position = new Vector3 ( cornerSource.position.x, candidateToThrowIn.transform.position.y, cornerSource.position.z);
					
				
					if ( whoLastTouched.tag == "PlayerTeam1" ) {
					
						candidateToThrowIn.GetComponent<Player_Script>().temporallyUnselectable = true;
						candidateToThrowIn.GetComponent<Player_Script>().timeToBeSelectable = 1.0f;
				
						candidateToThrowIn.transform.LookAt( SearchPlayerNearBall( opponents ).transform.position);

					}
					else
						candidateToThrowIn.transform.LookAt( center ); 
	
				
	
				
					candidateToThrowIn.transform.Rotate(0, sphere.fHorizontal*10.0f, 0);
					candidateToThrowIn.GetComponent<Player_Script>().state = Player_Script.Player_State.CORNER_KICK;
				
					sphere.GetComponent<Rigidbody>().isKinematic = true;
//					sphere.gameObject.transform.position = candidateTosaqueBanda.GetComponent<Player_Script>().hand_bone.position;
				
					sphere.gameObject.transform.position = cornerSource.position;//letakin bola dilokasi corner
				
				
					targetThrowIn = candidateToThrowIn.transform.position + candidateToThrowIn.transform.forward;
				
				
					candidateToThrowIn.animation.Play("rest");//buat pmaen diem
					state = InGameState.CORNER_CHASING;
				
				break;

					
			case InGameState.CORNER_CHASING:


				candidateToThrowIn.transform.LookAt( targetThrowIn );//bikin player jd liat ke target throwin
				candidateToThrowIn.GetComponent<Player_Script>().state = Player_Script.Player_State.CORNER_KICK;
				
				sphere.GetComponent<Rigidbody>().isKinematic = true;

				if ( whoLastTouched.tag != "PlayerTeam1" ) {//klo player yg corner
				
					targetThrowIn += Camera.main.transform.right*(sphere.fHorizontal/10.0f);
					
					if (sphere.bPassButton) {//klo ud dipencet pass gerakin animasi mw nendang pmaennya
						candidateToThrowIn.animation.Play("backwards");
						state = InGameState.CORNER_DOING;
		
					}
						
				} else {//klo cpu gerakin mundur stlh 3dtk
					
					timeToThrowInByOpponent -= Time.deltaTime;
					
					if ( timeToThrowInByOpponent < 0.0f ) {					
						timeToThrowInByOpponent = 3.0f;
						sphere.gameObject.GetComponent<Rigidbody>().isKinematic = true;
						candidateToThrowIn.animation.Play("backwards");
						state = InGameState.CORNER_DOING;
					}
					
				}

				
				
			break;

				
			case InGameState.CORNER_DOING:
			
				candidateToThrowIn.transform.position -= candidateToThrowIn.transform.forward * Time.deltaTime;//bikin player gerak mundur
				
				if ( candidateToThrowIn.animation.IsPlaying("backwards") == false ) {//klo ud kelar animasi mundurnya
					
					candidateToThrowIn.animation.Play("kick_ball");//play animasi nendang corner
					state = InGameState.CORNER_DOING_2;
				}
				
			break;				
				
				
			case InGameState.CORNER_DOING_2:
				

				if ( candidateToThrowIn.animation["kick_ball"].normalizedTime >= 0.5f && sphere.gameObject.GetComponent<Rigidbody>().isKinematic == true ) {//klo animasi ud stngh jalan,bikin bola bs gravitasi dan gerak maju
					sphere.gameObject.GetComponent<Rigidbody>().isKinematic = false;
					sphere.gameObject.GetComponent<Rigidbody>().AddForce( candidateToThrowIn.transform.forward*7000.0f + new Vector3(0.0f, 3300.0f, 0.0f) );		//forward itu = (0,0,1)			
				} 
				
				
				if ( candidateToThrowIn.animation.IsPlaying("kick_ball") == false ) {
					state = InGameState.CORNER_DONE;
				}
				
				
				
			break;
				
				
				
			case InGameState.CORNER_DONE:
				
				candidateToThrowIn.GetComponent<Player_Script>().state = Player_Script.Player_State.MOVE_AUTOMATIC;//suruh player move automatic				
				state = InGameState.PLAYING;
				
			break;
				
				
			case InGameState.GOAL_KICK:
				
				sphere.transform.position = goal_kick.position;//set bola di lokasi goal kick
				sphere.gameObject.GetComponent<Rigidbody>().isKinematic = true;//bikin bola kinematic
				goalKeeper.transform.rotation = goal_kick.transform.rotation;//set rotasi kiper ikutin rotasi goalkick
				goalKeeper.transform.position = new Vector3( goal_kick.transform.position.x, goalKeeper.transform.position.y ,goal_kick.transform.position.z)- (goalKeeper.transform.forward*1.0f);//set posisi kiper jd dibelakang posisi bola dikit
				goalKeeper.GetComponent<GoalKeeper_Script>().state = GoalKeeper_Script.GoalKeeper_State.GOAL_KICK;
							
		
				foreach ( GameObject go in players ) {//suruh smua player ke posisi awal
					go.GetComponent<Player_Script>().state = Player_Script.Player_State.GO_ORIGIN;
				}
				foreach ( GameObject go in opponents ) {
					go.GetComponent<Player_Script>().state = Player_Script.Player_State.GO_ORIGIN;
				}
				
				sphere.owner = null;

			
				goalKeeper.animation.Play("backwards");	
				state = InGameState.GOAL_KICK_RUNNING;
				
				
			break;
			case InGameState.GOAL_KICK_RUNNING:
				
				goalKeeper.transform.position -= goalKeeper.transform.forward * Time.deltaTime;//bikin GK gerak mundur
				
				if ( goalKeeper.animation.IsPlaying("backwards") == false ) {
					goalKeeper.animation.Play("kick_ball");	
					state = InGameState.GOAL_KICK_KICKING;
				}
			
				
			break;	
				
			case InGameState.GOAL_KICK_KICKING:
				
				goalKeeper.transform.position += goalKeeper.transform.forward * Time.deltaTime;//bikin GK gerak maju nendang

				if ( goalKeeper.animation["kick_ball"].normalizedTime >= 0.5f && sphere.gameObject.GetComponent<Rigidbody>().isKinematic == true) {
					sphere.gameObject.GetComponent<Rigidbody>().isKinematic = false;
					float force = Random.Range(5000.0f, 12000.0f);//random kekuatan tendangan gol kick
					sphere.gameObject.GetComponent<Rigidbody>().AddForce( (goalKeeper.transform.forward*force) + new Vector3(0,3000.0f,0) );
				}
	
				if ( goalKeeper.animation.IsPlaying("kick_ball") == false ) {//klo goal kick ud kelar

					goalKeeper.GetComponent<GoalKeeper_Script>().state = GoalKeeper_Script.GoalKeeper_State.GO_ORIGIN;	
					state = InGameState.PLAYING;
					
				}
				
			break;

			case InGameState.GOAL:
				
				
				foreach ( GameObject go in players ) {
					go.GetComponent<Player_Script>().state = Player_Script.Player_State.THROW_IN;//biar diem pmaenny kg lgsg ngejer bola
					go.transform.position = go.GetComponent<Player_Script>().kondisiAwal;//reset posisi pmaen kyk awal. klo initial condition bs error
					go.animation.Play("rest");
				}
				foreach ( GameObject go in opponents ) {
					go.GetComponent<Player_Script>().state = Player_Script.Player_State.THROW_IN;
					go.transform.position = go.GetComponent<Player_Script>().kondisiAwal;
					go.animation.Play("rest");
				}
				
					keeper_opponent.GetComponent<GoalKeeper_Script>().state = GoalKeeper_Script.GoalKeeper_State.RESTING;
					keeper.GetComponent<GoalKeeper_Script>().state = GoalKeeper_Script.GoalKeeper_State.RESTING;
				
				timeToKickOff -= Time.deltaTime;//mulai countdown 4dtk seblm mulai kick off
				
				if ( timeToKickOff < 0.0f ) {
					timeToKickOff = 4.0f;
					state = InGameState_Script.InGameState.PREPARE_TO_KICK_OFF;
					isKickOff = true;
				}
				
				
			break;


				
			case InGameState.KICK_OFF://buat Kick off pertama
				
				
				foreach ( GameObject go in players ) {//bikin player gerak ndiri menuju posisi awal
					go.GetComponent<Player_Script>().state = Player_Script.Player_State.MOVE_AUTOMATIC;
					go.transform.position = go.GetComponent<Player_Script>().initialPosition;
				}
				foreach ( GameObject go in opponents ) {
					go.GetComponent<Player_Script>().state = Player_Script.Player_State.MOVE_AUTOMATIC;
					go.transform.position = go.GetComponent<Player_Script>().initialPosition;
				}
				
				keeper.GetComponent<GoalKeeper_Script>().state = GoalKeeper_Script.GoalKeeper_State.RESTING;
				keeper_opponent.GetComponent<GoalKeeper_Script>().state = GoalKeeper_Script.GoalKeeper_State.RESTING;
				
				sphere.owner = null;
				sphere.gameObject.transform.position = center.position;//taro bola ditngh
				sphere.gameObject.GetComponent<Rigidbody>().drag = 0.3f;//set gaya gesek bola 0.3,awalnya 0,5
				state = InGameState_Script.InGameState.PLAYING;
				isKickOff = true;
				break;


			case InGameState.PREPARE_TO_KICK_OFF:


				sphere.transform.position = center.position;



				foreach ( GameObject go in players ) {//seluruh player liat bola
					go.transform.LookAt( sphere.transform );
				}
				foreach ( GameObject go in opponents ) {
					go.transform.LookAt( sphere.transform );
				}


				if ( scoredByVisiting  ) {//klo oponent yg goal
					//passer itu penendang terakhir
					if(passer && passed){//changed, diubah krn suka kosong ga ad org yg kickoff, jd hrs di random
						passer.transform.position = sphere.transform.position + new Vector3( 0.0f, 0, 1.0f );//set posisi penendang
						passer.transform.LookAt( sphere.transform.position );
						passed.transform.position = passer.transform.position + (passer.transform.forward * 5.0f);//set posisi penerima
						passer.state = Player_Script.Player_State.KICK_OFFER;
						sphere.owner = passer.gameObject;
						isKickOff = true;
					}
					else{
						//yg kickoff dr team player itu nmr 2 ama 8 krn dy striker
						int index = 2;
						passer = players[index].GetComponent<Player_Script>();
						int index2 = 8;
						passed = players[index2].GetComponent<Player_Script>();

						passer.transform.position = sphere.transform.position + new Vector3( 0.0f, 0, 1.0f );//set posisi penendang
						passer.transform.LookAt( sphere.transform.position );
						passed.transform.position = passer.transform.position + (passer.transform.forward * 5.0f);//set posisi penerima
						passer.state = Player_Script.Player_State.KICK_OFFER;
						sphere.owner = passer.gameObject;
						isKickOff = true;
					}
				}

				if ( scoredByLocal  ) {//klo player yg goal
					if(passer_opponent && passed_opponent){
						passer_opponent.transform.position = sphere.transform.position + new Vector3( 0.0f, 0, -1.0f );
						passer_opponent.transform.LookAt( sphere.transform.position );
						passed_opponent.transform.position = passer_opponent.transform.position + (passer_opponent.transform.forward * 5.0f);
						passer_opponent.state = Player_Script.Player_State.KICK_OFFER;
						sphere.owner = passer_opponent.gameObject;
						isKickOff = true;
					}else{
						int index = 5;
						passer_opponent = opponents[index].GetComponent<Player_Script>();
						int index2 = 6;
						passed_opponent = opponents[index2].GetComponent<Player_Script>();

						passer_opponent.transform.position = sphere.transform.position + new Vector3( 0.0f, 0, -1.0f );
						passer_opponent.transform.LookAt( sphere.transform.position );
						passed_opponent.transform.position = passer_opponent.transform.position + (passer_opponent.transform.forward * 5.0f);
						passer_opponent.state = Player_Script.Player_State.KICK_OFFER;
						sphere.owner = passer_opponent.gameObject;
						isKickOff = true;
					}
				}

				
				scoredByLocal = false;
				scoredByVisiting = false;
				
				break;
				
				
				
			}
		
		}
		
	}

	// buat cari pmaen yg paling dkt ama bola
	GameObject SearchPlayerNearBall( GameObject[] arrayPlayers) {
		
	    GameObject candidatePlayer = null;
		float distance = 1000.0f;
		foreach ( GameObject player in arrayPlayers ) {			
			
			if ( !player.GetComponent<Player_Script>().temporallyUnselectable ) {//selama pemaennya bs diselect
				
				Vector3 relativePos = sphere.transform.InverseTransformPoint( player.transform.position );	//jdin local jaraknya bkn global dgn bola	
				float newdistance = relativePos.magnitude;
				
				if ( newdistance < distance ) {
				
					distance = newdistance;					
					candidatePlayer = player;					

				}
			}
			
		}
						
		return candidatePlayer;	
	}
	
	
	// bikin pmaen ada di dlm box saat corner,bikin pmaen yg bertipe itu ada di dlm box
	
	void PutPlayersInCornerArea( GameObject[] arrayPlayers, Player_Script.TypePlayer type) {
	
		
		foreach ( GameObject player in arrayPlayers ) {			
			
			if ( player.GetComponent<Player_Script>().type == type ) {
			
				//area corner diisi secara dynamic dari corner_script, nentuin area kotak penalty mana
				float xmin = areaCorner.GetComponent<BoxCollider>().bounds.min.x;
				float xmax = areaCorner.GetComponent<BoxCollider>().bounds.max.x;
				float zmin = areaCorner.GetComponent<BoxCollider>().bounds.min.z;
				float zmax = areaCorner.GetComponent<BoxCollider>().bounds.max.z;
				
				float x = Random.Range( xmin, xmax );
				float z = Random.Range( zmin, zmax );//random posisinya selama masih di dalem box
				
				player.transform.position = new Vector3( x, player.transform.position.y ,z);
				
				
			}
			
			
		}		
		
		
		
	}
	
	
	
}
