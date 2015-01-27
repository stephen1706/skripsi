using UnityEngine;
using System.Collections;

public class ScoreHUD : MonoBehaviour {

	private InGameState_Script inGame;

	void Start () {
		
		inGame = GameObject.FindObjectOfType( typeof( InGameState_Script ) ) as InGameState_Script;		
		
	}

	void LateUpdate () {//update skor
	
		GetComponentInChildren<GUIText> ().text = inGame.localScore + " - " + inGame.visitingScore;
	}
}
