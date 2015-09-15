using UnityEngine;
using System.Collections;

public class MasterState_Manager : MonoBehaviour {

	/// <summary>
	/// Controls the Master State of the Game:
	/// - Load Levels / Scenes
	/// - Track Player Resources across levels
	/// - Save Game
	/// - Manipulate Time to Pause game
	/// - Pause CoRoutines in other scripts by changing Master State
	/// </summary>

	public enum MasterState { WAITING, LOADING, START, PAUSED, CONTINUE }

	private MasterState _mState = MasterState.WAITING;

	[HideInInspector]
	public MasterState mState { get { return _mState; } set { _mState = value; }}


	void Start () 
	{
	
	}
	

	void Update ()
	{
	
	}

	void MasterStateMachine(MasterState _curState)
	{
		switch (_curState) {
		case MasterState.WAITING:
			// Waiting to Load level
			break;
		case MasterState.LOADING:
			// Loading a Level
			// If level has been loaded, state is Start
			break;
		case MasterState.START:
			// Initialize a level
			// If all units and map have been initialized, state is Continue
			break;
		case MasterState.PAUSED:
			// Pause the game
			break;
		case MasterState.CONTINUE:
			// Level has been initialized, run Time as normal
			break;
		default:
			_mState = MasterState.WAITING;
			break;
		}
	}

	public void TestPause()
	{
		_mState = MasterState.PAUSED;
		Debug.Log ("Master State is: " + _mState.ToString() +" Pausing game! ");
		Time.timeScale = 0;
	}

	public void UnPause()
	{
		_mState = MasterState.CONTINUE;
		Debug.Log ("Master State is: " + _mState.ToString() +" Continuing game! ");
		Time.timeScale = 1;
	}
}
