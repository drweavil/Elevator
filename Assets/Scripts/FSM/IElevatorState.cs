using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IElevatorState {
	void AddTargetFloorRequst(ITargetFloorRequest request);
	void OpenTheDoor ();
	void CloseTheDoor();
	void Stop();
}
