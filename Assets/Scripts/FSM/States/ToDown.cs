using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToDown : MonoBehaviour,IElevatorState {

	public void AddTargetFloorRequst(ITargetFloorRequest request){
		request.AddToDictionary ();
		ElevatorController.instance.AddTargetFloorNumberRequest (request.GetFloorNumber());
		ElevatorController.instance.targetFloorNumber = ElevatorController.instance.GetNextTarget (ElevatorController.instance.CurrentFloor);
	}

	public void OpenTheDoor(){
		ElevatorController.instance.ChangeState (new DoorsOpen());
		ElevatorController.instance.OpenTheDoor ();
	}

	public void CloseTheDoor(){
	}

	public void Stop(){
		ElevatorController.instance.StopElevator ();
		ElevatorController.instance.currentState = new StopState ();
	}
}
