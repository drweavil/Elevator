using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorsOpen : MonoBehaviour,IElevatorState {
	public void AddTargetFloorRequst(ITargetFloorRequest request){
		request.AddToDictionary ();
		if (request.GetFloorNumber () == ElevatorController.instance.CurrentFloor) {
			ElevatorController.instance.RemoveCompleteFloor (ElevatorController.instance.CurrentFloor);
		} else {
			ElevatorController.instance.AddTargetFloorNumberRequest (request.GetFloorNumber());
			if (request.ItClosesTheDoors ()) {
				ElevatorController.instance.CloseTheDoor ();
			}
		}
	}
	public void OpenTheDoor (){}

	public void CloseTheDoor(){
		int nextTarget = ElevatorController.instance.GetNextTarget (ElevatorController.instance.CurrentFloor);
		bool targetNotFound = false;

		if (nextTarget == -1) {
			ElevatorController.instance.SwitchDirection ();
			nextTarget = ElevatorController.instance.GetNextTarget (ElevatorController.instance.CurrentFloor);
			if (nextTarget == -1) {
				targetNotFound = true;
				ElevatorController.instance.currentState = new WaitOnFloor ();
			}
		} 

		if (!targetNotFound) {
			if (ElevatorController.instance.currentDirection == ElevatorController.DIRECTION_UP) {
				ElevatorController.instance.ChangeState (new ToUp ());
			} else {
				ElevatorController.instance.ChangeState (new ToDown ());
			}

			ElevatorController.instance.targetFloorNumber = nextTarget;
			ElevatorController.instance.StartMoving ();
		}

	}

	public void  Stop(){
		ElevatorController.instance.StopElevator ();
		ElevatorController.instance.currentState = new StopState ();
	}
}
