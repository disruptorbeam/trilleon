using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyedEquation : MonoBehaviour {

	public DestroyedEquationCharacter de1;
	public DestroyedEquationCharacter do1;
	public DestroyedEquationCharacter de2;
	public DestroyedEquationCharacter do2;
	public DestroyedEquationCharacter de3;

	void Update() {
		
		if(this.gameObject.transform.childCount == 0) {

			Destroy(gameObject);

		}

	}

}