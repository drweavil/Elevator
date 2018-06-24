using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitOnFloor : MonoBehaviour,IElevatorState {
	public static WaitOnFloor instance = new WaitOnFloor();

	public void AddTargetFloorRequst(ITargetFloorRequest request){
		request.AddToDictionary ();
		if (request.GetFloorNumber () == ElevatorController.instance.CurrentFloor) {
			ElevatorController.instance.RemoveCompleteFloor (ElevatorController.instance.CurrentFloor);
			ElevatorController.instance.ChangeState (new DoorsOpen());
			ElevatorController.instance.OpenTheDoor ();
		}else if (request.GetFloorNumber() > ElevatorController.instance.CurrentFloor) {
			ElevatorController.instance.ChangeState (new ToUp());
			ElevatorController.instance.targetFloorNumber = request.GetFloorNumber ();
			ElevatorController.instance.AddTargetFloorNumberRequest (request.GetFloorNumber ());
			ElevatorController.instance.currentDirection = ElevatorController.DIRECTION_UP;
			ElevatorController.instance.StartMoving ();
		}else if (request.GetFloorNumber() < ElevatorController.instance.CurrentFloor) {
			ElevatorController.instance.ChangeState (new ToDown());
			ElevatorController.instance.targetFloorNumber = request.GetFloorNumber ();
			ElevatorController.instance.AddTargetFloorNumberRequest (request.GetFloorNumber ());
			ElevatorController.instance.currentDirection = ElevatorController.DIRECTION_DOWN;
			ElevatorController.instance.StartMoving ();
		}
	}
	public void OpenTheDoor(){
		ElevatorController.instance.OpenTheDoor ();
	}

	public void CloseTheDoor(){
	}

	public void Stop(){
	}
}
