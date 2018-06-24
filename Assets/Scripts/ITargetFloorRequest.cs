using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITargetFloorRequest {
	int GetFloorNumber();
	void AddToDictionary();
	void RemoveFromDictionary();
	bool ItClosesTheDoors ();
}
