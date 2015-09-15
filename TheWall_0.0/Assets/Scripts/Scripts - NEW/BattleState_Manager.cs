using UnityEngine;
using System.Collections;

public class BattleState_Manager : MonoBehaviour {

	public class Battle {

		// how many player units participating (MAX:  4)
		private int _playerCount;
		public int playerCount{get{return _playerCount;} set{_playerCount = Mathf.Clamp(value, 1, 4);}}

		// how many Enemies participating  (MAX:  4)
		private int _enemyCount;
		public int enemyCount {get{return _enemyCount;} set{_enemyCount = Mathf.Clamp(value, 1, 4);}}

		public GameObject[] playerUnits, enemyUnits;

		public void Init(int pCount, int eCount)
		{
			playerCount = pCount;
			enemyCount = eCount;

			// Initialize the Unit arrays using these lenghts
			InitUnits (playerCount, enemyCount);
		}

		void InitUnits(int pCount, int eCount)
		{
			playerUnits = new GameObject[pCount];

			enemyUnits = new GameObject[eCount];
		}
	}

	public Battle newBattle = new Battle();

	enum State { START, ENEMY_TURN, PLAYER_TURN, PLAYER_DEATH, CHECK };

	private State _state = State.START;

	public GameObject[] players, enemies; 

	void Start () 
	{
		// Since this script is Loaded by another script, player and enemy arrays have already been initialized

		// TEST:
		newBattle.Init (3, 3);

		// Fill the unit arrays from the public arrays loaded by another script
		SetUnitArrays ();

	}

	void SetUnitArrays()
	{
		for (int x =0; x < newBattle.playerUnits.Length; x++) {
			newBattle.playerUnits[x] = players[x];
			Debug.Log("Player Unit " + x + " is " + newBattle.playerUnits[x].name); 
		} 

		for (int y = 0; y < newBattle.enemyUnits.Length; y++) {
			newBattle.enemyUnits[y] = enemies[y];
			Debug.Log("Enemy Unit " + y + " is " + newBattle.enemyUnits[y].name); 
		}
		Debug.Log ("BATTLE MAN: Finished loading the Unit arrays!");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void BattleStateMachine(State _curState)
	{
		switch (_curState) {
		case State.START:
			// calculate initiative if unit arrays are not null
			if (newBattle.playerUnits[0] != null && newBattle.enemyUnits[0] != null)
				CalcTurnOrder();
			break;
		case State.ENEMY_TURN:
			// Enemy takes their turn
			// player can't access menus
			break;
		case State.PLAYER_TURN:
			// Player takes their turn.
			break;
		case State.PLAYER_DEATH:
			// Always after Enemy Turn loop through Player units to see if at least ONE is still alive
			// IF NOT, End battle immdiately and load a game over/retry screen
			break;
		case State.CHECK:
			// Loop through the enemies array and verify that at least ONE is still alive
			// If none are alive load Victory Screen
			break;
		default:
			// Go to Check to see if this battle should continue
			_state = State.CHECK;
			break;
		}
	}

	void CalcTurnOrder()
	{

	}
}
