using UnityEngine;
using System.Collections;

public class SetShield : MonoBehaviour {

	public string localOrVisiting;

	void Start () {
	//buat ganti warna logo tim
		string nameTeam = PlayerPrefs.GetString( localOrVisiting );//ambil dr shared pref warnany
		Sprite spr = Resources.Load<Sprite>("Textures/" + "Shield_" + nameTeam);
		SpriteRenderer sprRenderer= (SpriteRenderer)renderer;
		sprRenderer.sprite = spr;

	}

	void Update () {
	
	}
}
