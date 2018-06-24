using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopState : MonoBehaviour,IElevatorState {

	public void AddTargetFloorRequst(ITargetFloorRequest request){
		request.AddToDictionary ();
		ElevatorController.instance.AddTargetFloorNumberRequest (request.GetFloorNumber());
		if (request.GetFloorNumber () == ElevatorController.instance.CurrentFloor) {
			ElevatorController.instance.targetFloorNumber = request.GetFloorNumber ();
			if (ElevatorController.instance.Interfloor < (float)ElevatorController.instance.CurrentFloor) {
				ElevatorController.instance.currentDirection = ElevatorController.DIRECTION_UP;
				ElevatorController.instance.ChangeState (new ToUp ());
			} else {
				ElevatorController.instance.currentDirection = ElevatorController.DIRECTION_DOWN;
				ElevatorController.instance.ChangeState (new ToDown ());
			}
			ElevatorController.instance.StartMoving ();
		} else {
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
	}
	public void OpenTheDoor (){}
	public void CloseTheDoor(){}
	public void Stop(){}
}
