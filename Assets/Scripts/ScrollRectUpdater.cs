using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollRectUpdater : MonoBehaviour {
	[SerializeField]private RectTransform updatedRect;
	[SerializeField]private Scrollbar scroll;


	public void UpdateRect (){
		StartCoroutine (UpdateRectCoroutine()); 
	}

	IEnumerator UpdateRectCoroutine(){
		for (int i = 0; i <= 4; i++) {
			if (i == 1) {
				updatedRect.sizeDelta = new Vector2 (updatedRect.sizeDelta.x, updatedRect.sizeDelta.y + 0.001f); 
				yield return null;
			} else if (i == 3) {
				if (scroll.IsActive ()) {
					scroll.value = 0;
				}
				yield break;
			} else {
				yield return null;
			}
		}
	}
}
