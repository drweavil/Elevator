using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloorButton : MonoBehaviour, ITargetFloorRequest {
	
	public int FloorNumber{ get; private set;}
	[SerializeField]private Text floorText;
	[SerializeField]private GameObject isActiveObj;
	[SerializeField]private bool _isActive;
	public bool IsActive{
		get{ return _isActive;} 
		set{ 
			_isActive = value;
			if (_isActive) {
				isActiveObj.SetActive (true);
			} else {
				isActiveObj.SetActive (false);
			}
		}
	}

	public void SetFloor(int number){
		FloorNumber = number;
		if (floorText != null) {
			floorText.text = number.ToString ();
		}
	}


	public void Click(){
		IsActive = true;
		ElevatorController.instance.currentState.AddTargetFloorRequst (this);
	}


	public int GetFloorNumber(){
		return FloorNumber;
	}
	public void AddToDictionary(){
		if (!ElevatorController.instance.floorButtons.ContainsKey (this.FloorNumber)) {
			ElevatorController.instance.floorButtons.Add (this.FloorNumber, this);
		}
	}
	public void RemoveFromDictionary(){
		if (ElevatorController.instance.floorButtons.ContainsKey (this.FloorNumber)) {
			ElevatorController.instance.floorButtons.Remove (this.FloorNumber);
		}
	}

	public bool ItClosesTheDoors(){
		return true;
	}
}
