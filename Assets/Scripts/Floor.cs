using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Floor : MonoBehaviour, ITargetFloorRequest {
	
	public bool IsWaiting { get; private set;}
	public int ExpectedDirection { get; private set;}
	public int FloorNumber { get; private set;}
	[SerializeField]private Text floorNumberText;

	[SerializeField]private Text doorStatus;
	[SerializeField]private Text currentElevatorFloor;
	[SerializeField]private FloorButton upButton;
	public FloorButton UpButton{ get{ return upButton;}}
	[SerializeField]private FloorButton downButton;
	public bool DoorsOpened { get; private set;}
	public FloorButton DownButton{ get{ return downButton;}}



	public void ToUp(){
		ExpectedDirection = ElevatorController.DIRECTION_UP;
		IsWaiting = true;
		UpButton.IsActive = true;
		DownButton.IsActive = false;
		ElevatorController.instance.currentState.AddTargetFloorRequst (this);
	}

	public void ToDown(){
		ExpectedDirection = ElevatorController.DIRECTION_DOWN;
		IsWaiting = true;
		UpButton.IsActive = false;
		DownButton.IsActive = true;
		ElevatorController.instance.currentState.AddTargetFloorRequst (this);
	}

	public void StopWaiting(){
		IsWaiting = false;
		UpButton.IsActive = false;
		DownButton.IsActive = false;
	}

	public void SetFloor(int number){
		FloorNumber = number;
		upButton.SetFloor(number);
		upButton.IsActive = false;
		downButton.SetFloor(number);
		downButton.IsActive = false;
		floorNumberText.text = number.ToString ();;


		if (number == 1) {
			downButton.gameObject.SetActive (false);
		}
		if (number == ElevatorController.instance.MaximumFloor) {
			upButton.gameObject.SetActive (false);
		}
	}

	public void SetCurrentElevatorFloorNumber(int number){
		currentElevatorFloor.text = number.ToString ();
	}

	public void SetDoorStatus(bool isOpened){
		DoorsOpened = isOpened;
		if (isOpened) {
			doorStatus.text = "Дверь открыта";
		} else {
			doorStatus.text = "Дверь закрыта";
		}
	}

	public int GetFloorNumber(){
		return FloorNumber;
	}
	public void AddToDictionary(){
	}
	public void RemoveFromDictionary(){
	}
	public bool ItClosesTheDoors(){
		return false;
	}


}
