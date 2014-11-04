using UnityEngine;
using System.Collections;

public class Pass_Script : MonoBehaviour {

public GameObject sphere;


	void Update () {//buat nerima pass button di hp

		for (int touchIndex = 0; touchIndex<Input.touchCount; touchIndex++){
      		Touch currentTouch = Input.touches[touchIndex];//proses setiap touch
      		if(currentTouch.phase == TouchPhase.Ended && guiTexture.HitTest(currentTouch.position)){
				//klo ud diangkat jarinya dan posisi touch kena gui texture pass buttonnya (gui texture kyk rigidbody bs diakses dr code)

				sphere.GetComponent<Sphere>().pressiPhonePassButton = true;//set klo pass button dipencet
            
			} else {
		
				sphere.GetComponent<Sphere>().pressiPhonePassButton = false;
				
				
			}
    	}
	
	}
	
}
