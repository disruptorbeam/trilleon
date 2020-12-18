using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DestroyedEquationCharacter : MonoBehaviour {

	int randomizedPixelPerFrameSpeedX;
	int randomizedPixelPerFrameSpeedY;
	int randomRotationx;
	public string character { 
		get { 
			return GetComponent<Text>().text;
		}
		set { 
			GetComponent<Text>().text = value;
		}
	}

	void Start() {

		randomizedPixelPerFrameSpeedX = new System.Random(Guid.NewGuid().GetHashCode()).Next(225, 300);
		randomizedPixelPerFrameSpeedY = new System.Random(Guid.NewGuid().GetHashCode()).Next(-200, 200);
		randomRotationx = new System.Random(Guid.NewGuid().GetHashCode()).Next(1, 20);

	}

	void Update () {

		RectTransform rect = this.GetComponent<RectTransform>();
		rect.localPosition = new Vector3(rect.localPosition.x + (randomizedPixelPerFrameSpeedY * Time.deltaTime), rect.localPosition.y - (randomizedPixelPerFrameSpeedX * Time.deltaTime), -2.5f);
		rect.Rotate(new Vector3(randomRotationx, 0, 0));

		if(rect.localPosition.y < -Screen.height - 20) {

			Destroy(gameObject);

		}

	}

}