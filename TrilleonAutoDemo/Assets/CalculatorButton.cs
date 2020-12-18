using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CalculatorButton :  MonoBehaviour {

	public string Value;

	void Start() {

		this.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(Clicked);

	}

	public void Clicked() {
		
		Calculator.ButtonClick(Value);

	}

}