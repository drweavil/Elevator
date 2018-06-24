using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ElevatorController : MonoBehaviour {
	public static ElevatorController instance;
	public IElevatorState currentState;

	[SerializeField]private Transform elevatorPanelButtonsTransform;
	[SerializeField]private Transform floorsTransform;
	[SerializeField]private ScrollRectUpdater elevatorPanelUpdater;
	[SerializeField]private ScrollRectUpdater floorsUpdater;

	public Dictionary<int, Floor> floors = new Dictionary<int,Floor>();
	public Dictionary<int, FloorButton> floorButtons = new Dictionary<int, FloorButton> ();
	public int MaximumFloor{ get; private set;}
	public const int DIRECTION_UP = 1, DIRECTION_DOWN = 0;
	public int currentDirection = DIRECTION_UP;

	[SerializeField]private InputField floorsNumberInput;
	[SerializeField]private GameObject startPage;

	[SerializeField]private int _currentFloor;
	public int CurrentFloor { 
		get{return _currentFloor; }
		private set{
			_currentFloor = value; 
			if (onFloorChanged != null){
				onFloorChanged (_currentFloor);
			}
		}
	}
	[SerializeField] private float _interfloor;
	public float Interfloor { get{return _interfloor;} private set{_interfloor = value;}}
	public bool isMoving = false;
	public int targetFloorNumber;
	[SerializeField]private List<int> targetFloorNumberRequests = new List<int> ();

	[SerializeField]private float currentElevatorPosition = 0;
	[SerializeField]private float doorsElevatorState = 0;
	private const float ONE_FRAME_ELEVATOR_POSITION_MODIFIER = 0.004f;
	private const float ONE_FRAME_ELEVATOR_DOORS_MODIFIER = 0.008f;
	public delegate void ChangeFloorAction(int newFloorNumber);
	public event ChangeFloorAction onFloorChanged;

	void Start () {
		instance = this;
	}

	void Update(){
		if (Input.GetKeyDown (KeyCode.I)) {
			Debug.Log (currentState.GetType ());
		}
	}

	public void BuildElevator(){
		int floorsNumber = 2;
		if (floorsNumberInput.text != "") {
			floorsNumber = System.Convert.ToInt32 (floorsNumberInput.text);
		}
		if (floorsNumber < 2) {
			floorsNumber = 2;
		}
		if (floorsNumber > 100) {
			floorsNumber = 100;
		}

		MaximumFloor = floorsNumber;
		CurrentFloor = 1;
		Interfloor = 1f;
		currentState = new WaitOnFloor ();

		GameObject floorButtonPrefab = (GameObject)Resources.Load ("floorButton");
		GameObject floorPrefab = (GameObject)Resources.Load ("Floor");


		for (int i = 1; i <= floorsNumber; i++) {
			GameObject floorButtonObj = Instantiate (floorButtonPrefab, elevatorPanelButtonsTransform);
			FloorButton floorButton = floorButtonObj.GetComponent<FloorButton> ();
			floorButton.SetFloor(i);
			floorButton.IsActive = false;

			GameObject floorObj = Instantiate (floorPrefab, floorsTransform);
			Floor floor = floorObj.GetComponent<Floor> (); 
			onFloorChanged += floor.SetCurrentElevatorFloorNumber;
			floor.SetFloor (floorsNumber + 1 - i);
			floor.SetCurrentElevatorFloorNumber (CurrentFloor);

			floors.Add (floor.FloorNumber, floor);
		}
		elevatorPanelUpdater.UpdateRect ();
		floorsUpdater.UpdateRect ();



		startPage.SetActive (false);
	}	


	public void ChangeState(IElevatorState newState){
		currentState = newState;
	}
		
	public void AddTargetFloorRequst(ITargetFloorRequest request){
		currentState.AddTargetFloorRequst (request);
	}


	public void AddTargetFloorNumberRequest(int number){
		if (!targetFloorNumberRequests.Contains(number)) {
			targetFloorNumberRequests.Add (number);
			targetFloorNumberRequests.Sort ();
		}
	}

	public void RemoveFloorNumberRequest(int number){
		if (targetFloorNumberRequests.Contains(number)) {
			targetFloorNumberRequests.Remove (number);
		}
	}

	public int TargetFloorNumberRequestsCount(){
		return targetFloorNumberRequests.Count;
	}

	public void RemoveCompleteFloor(int floorNumber){
		floors [floorNumber].StopWaiting ();
		if (floorButtons.ContainsKey (floorNumber)) {
			floorButtons [floorNumber].IsActive = false;
			floorButtons.Remove (floorNumber);
		}
		RemoveFloorNumberRequest (floorNumber);
	}

	public int GetNextTarget(int number){
		if (currentDirection == DIRECTION_UP) {
			List<int>nextTargetNumbersUp = targetFloorNumberRequests.FindAll (t => t > number);
			if (nextTargetNumbersUp.Count != 0) {
				int maximumFloorNumber = nextTargetNumbersUp [nextTargetNumbersUp.Count - 1];
				for (int i = 0; i < nextTargetNumbersUp.Count; i++) {
					if (floors [nextTargetNumbersUp [i]].IsWaiting) {
						if (floors [nextTargetNumbersUp [i]].ExpectedDirection == DIRECTION_UP || floors [nextTargetNumbersUp [i]].FloorNumber == maximumFloorNumber) {
							return nextTargetNumbersUp [i];		
						}
					} else {
						return nextTargetNumbersUp [0];		
					}
				}
			}
		}else if(currentDirection == DIRECTION_DOWN){
			List<int> nextTargetNumbersDown = targetFloorNumberRequests.FindAll (t => t < number);
			nextTargetNumbersDown.Reverse ();
			if (nextTargetNumbersDown.Count != 0) {
				int maximumFloorNumber = nextTargetNumbersDown [nextTargetNumbersDown.Count - 1];
				for (int i = 0; i < nextTargetNumbersDown.Count; i++) {
					if (floors [nextTargetNumbersDown [i]].IsWaiting) {
						if (floors [nextTargetNumbersDown [i]].ExpectedDirection == DIRECTION_DOWN || floors [nextTargetNumbersDown [i]].FloorNumber == maximumFloorNumber) {
							return nextTargetNumbersDown [i];		
						}
					} else {
						return nextTargetNumbersDown [0];		
					}
				}
			}
		}


		return -1;
	}

	public void SwitchDirection(){
		if (currentDirection == DIRECTION_UP) {
			currentDirection = DIRECTION_DOWN;
		}else if (currentDirection == DIRECTION_DOWN ) {
			currentDirection = DIRECTION_UP;
		}
	}

	public void OpenTheDoor(){
		doorsElevatorState = 0;
		floors [CurrentFloor].SetDoorStatus (true);
		StartCoroutine (OpenTheDoorProcess());
	}

	public void CloseTheDoor(){
		doorsElevatorState = 1;
		floors [CurrentFloor].SetDoorStatus (false);
		currentState.CloseTheDoor ();
	}

	IEnumerator OpenTheDoorProcess(){
		doorsElevatorState = 0;
		while (true) {
			doorsElevatorState += ONE_FRAME_ELEVATOR_DOORS_MODIFIER;
			if (doorsElevatorState >= 1f || currentState.GetType () != typeof(DoorsOpen)) {
				if (floors [CurrentFloor].DoorsOpened) {
					CloseTheDoor ();
				}
				yield break;
			} else {
				yield return null;
			}
		}
	}



	public void StartMoving(){
		if (!isMoving) {
			isMoving = true;
			StartCoroutine (MovingProcess ());

		}
	}

	public void StopButton(){
		currentState.Stop ();
	}
	public void StopElevator(){
		isMoving = false;
		List<int> tartgetFloorNumbersRequestsTmpList = new List<int>();
		foreach (int targetFloorNumberTmp in targetFloorNumberRequests) {
			tartgetFloorNumbersRequestsTmpList.Add (targetFloorNumberTmp);
		}

		foreach (int targetFloorNumberRequest in tartgetFloorNumbersRequestsTmpList) {
			RemoveCompleteFloor (targetFloorNumberRequest);
		}
		targetFloorNumberRequests.Clear ();
	}
		

	IEnumerator MovingProcess(){
		while (isMoving) {
			float modifierSign = 1f;
			bool elevatorPositionCondition = false;
			float elevatorPositionNullPoint = 0;

			if (currentState.GetType () == typeof(ToUp)) {
				if (currentElevatorPosition == 1f) {
					currentElevatorPosition = 0;
				}
			} else if (currentState.GetType () == typeof(ToDown)) {
				if (currentElevatorPosition == 0) {
					currentElevatorPosition = 1f;
				}
				modifierSign = -1f;
				elevatorPositionNullPoint = 1f;
			} else {
				yield break;
			}
			float modifier = ONE_FRAME_ELEVATOR_POSITION_MODIFIER * modifierSign;
			currentElevatorPosition += modifier;
			Interfloor +=  modifier;

			if (currentState.GetType () == typeof(ToUp)) {
				elevatorPositionCondition = currentElevatorPosition >= 1f;
			}else if(currentState.GetType () == typeof(ToDown)){
				elevatorPositionCondition = currentElevatorPosition <= 0;
			}


			if (elevatorPositionCondition) {
				CurrentFloor += (int)modifierSign;
				currentElevatorPosition = elevatorPositionNullPoint;
				Interfloor = (float)System.Math.Round (Interfloor, 0);

				if ((int)Interfloor != CurrentFloor) {
					CurrentFloor = (int)Interfloor;
					yield return null;

				}

				if (CurrentFloor == targetFloorNumber) {
					isMoving = false;
					RemoveCompleteFloor (CurrentFloor);
					currentState.OpenTheDoor ();
					yield break;
				} 
			} else {
				yield return null;
			}

		} 
	}


}
