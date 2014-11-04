using UnityEngine;
using System.Collections;

public class Goal_Script : MonoBehaviour {
	
	public Sphere sphere;
	public GameObject goalKeeper;
	public InGameState_Script ingame;
	public MeshFilter red;
	private Vector3[] arrayOriginalVertices;	

	void Start () {
		// get ball  in scene
		sphere = (Sphere)GameObject.FindObjectOfType( typeof(Sphere) );	

		// get net vertex to modify them
		arrayOriginalVertices = new Vector3[ red.mesh.vertices.Length ];
		
		for (int f=0; f< red.mesh.vertices.Length; f++) {
			arrayOriginalVertices[f] = red.mesh.vertices[f];
		}	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerEnter( Collider other) {//klo bola msk gawang


		if ( other.gameObject.tag == "Ball" /*&& Camera.main.GetComponent<InGameState_Script>().state == InGameState_Script.InGameState.PLAYING*/ ) {// klo bola yg msk berarti goal
			
			sphere.owner = null;
			
			goalKeeper.GetComponent<GoalKeeper_Script>().state = GoalKeeper_Script.GoalKeeper_State.GOAL_KICK;//refer goalkeeper yg di gameobject goal ini
			goalKeeper.animation.PlayQueued("rest");

			// add score depending of goal side
			if ( goalKeeper.tag == "GoalKeeper_Oponent" && ingame.state != InGameState_Script.InGameState.GOAL) {//klo GK musuh jd yg gol playernya
				ingame.localScore++;
				ingame.scoredByLocal = true;
				ingame.scoredByVisiting = false;
			} 
			
			if ( goalKeeper.tag == "GoalKeeper" && ingame.state != InGameState_Script.InGameState.GOAL) {			
				ingame.visitingScore++;
				ingame.scoredByLocal = false;
				ingame.scoredByVisiting = true;
			}
			
			
			Camera.main.GetComponent<InGameState_Script>().timeToChangeState = 2.0f;//2dtk lg reset game
			Camera.main.GetComponent<InGameState_Script>().state = InGameState_Script.InGameState.GOAL;
		}
		
		
		
	}
	void OnTriggerStay( Collider other ) {//buat gerakin jaring klo bola masuuk,dipanggil saat bola ada didalem trigger/gawang
	
		if ( other.gameObject.tag == "Ball" ) {

			Mesh meshRed = red.mesh;
			
			int numberVertex = meshRed.vertexCount;
			Vector3[] arrayVertices = meshRed.vertices;
			
	
			for ( int i=0; i<numberVertex; i++) {
						
				Vector3 worldPos = red.transform.TransformPoint( arrayOriginalVertices[i] );
							
				float distance = (worldPos-other.transform.position).magnitude;
				
				if ( distance < 3.0f ) {

					Vector3 destLocal = red.transform.InverseTransformPoint( other.transform.position );
					Vector3 sourceLocal = arrayOriginalVertices[i];					
					Vector3 dirLocal = (destLocal-sourceLocal);
				
				
					if (  Vector3.Dot( dirLocal, meshRed.normals[i] ) > 0.0f  ) {
						Vector3 finalLocal = sourceLocal + (dirLocal/(distance+0.1f));
						arrayVertices[i] = finalLocal; 
					} else {
						arrayVertices[i] = arrayOriginalVertices[i];					
					}
					
				} else {
					arrayVertices[i] = arrayOriginalVertices[i];
				}

			}
			meshRed.vertices = arrayVertices;
		
		}		
		
	}
	
	
}
