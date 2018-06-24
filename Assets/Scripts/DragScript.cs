using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragScript : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler {
	[SerializeField]private Vector2 startPosition;
	[SerializeField]private RectTransform rectTransform;
	[SerializeField]private Text textTest;
	[SerializeField]private Text textTest2;
	void Start(){
		startPosition = rectTransform.position;
	}

	public void OnDrag(PointerEventData data){
		rectTransform.position = Input.mousePosition;
		textTest.text = "Двигается";
	}


	public void OnBeginDrag(PointerEventData data){
		textTest2.gameObject.SetActive (true);
		textTest.text = "Начал двигаться";
	}

	public void OnEndDrag(PointerEventData data){
		rectTransform.position = startPosition;
		textTest.text = "Круг для драг интерфейсов";
		textTest2.gameObject.SetActive (false);
	}



	
}
