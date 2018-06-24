using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PolygonButton : MonoBehaviour {
	[SerializeField]private Image image;

	void Start(){
		image.alphaHitTestMinimumThreshold = 0.1f;
	}
}
