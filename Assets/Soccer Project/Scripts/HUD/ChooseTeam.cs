using UnityEngine;
using System.Collections;

public class ChooseTeam : MonoBehaviour {

	
	public Material normalMaterial;
	public Material selectMaterial;

	public ShieldMenu[] shields;

	public string Selected;
	public string localOrVisit;

	void Start () {


		if ( localOrVisit == "Local") {
			Selected = "Blue";//awal yg kepilih buat tim user
		} else {
			Selected = "Green";
		}
	
	}

	void Update () {
	
		if ( Input.GetMouseButton(0) ) {

			Vector3 inputPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			inputPos.z = 0;

			RaycastHit2D hit;
			hit = Physics2D.Raycast( inputPos, new Vector2( 0,0 ) );

			if ( hit.collider != null ) {//klo klik mouse ad yg kena di collider logo2 timny

				foreach ( ShieldMenu shield in shields ) {//ShieldMenu ini objek logo2 timny, tiap logo pny script ShieldMenu

					if ( hit.collider == shield.GetComponent<BoxCollider2D>() ) {

						foreach ( ShieldMenu _shield in shields ) {//set smua logo ga kepilih, materialny jd default
							_shield.renderer.material = normalMaterial; 
						}

						shield.renderer.material = selectMaterial;
						//klo logo ini yg diklik, ganti materialny jd yg selected material, kyk ad jaring" gt materialny
						Selected = shield.nameTeam;//ambil nama tim yg kepilih, cthnya blue
					}
				}
			}

			if ( hit.collider && hit.collider.tag == "playbutton" ) {
				//simpen warna yg dipilih ke shared pref
				PlayerPrefs.SetString( localOrVisit, Selected  );
				Application.LoadLevel("Football_match");
			}
		}
	}
}

