using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridManager : MonoBehaviour {

	private TilePlayer playerTile;

	public GameObject roomPrefab;
	private List<Room> rooms;

	public GameObject playerTilePrefab, firstRoomPrefab;


	//keeping track of animations/effects that will remove themselves
	//we use this to check if there are animaitons ocurring
	//this is where dead tiles will be until they die
	private List<GameObject> dyingAnimationObjects = new List<GameObject>();

	public AudioSource runeSoundSource, revealSoundSource;

	// Use this for initialization
	void Start () {

		rooms = new List<Room> ();

		StartCoroutine( addRoom (0, 0, null, Tile.Direction.Up) );

	}
	
	// Update is called once per frame
	void Update () {
	
		//every frame, check all of the dying objects to see if they are dead and can be rmeoved form the list
		for (int i=dyingAnimationObjects.Count-1; i>=0; i--) {
			if (dyingAnimationObjects [i] == null) {
				dyingAnimationObjects.RemoveAt(i);
			}
		}
	}

	IEnumerator addRoom(int xPos, int yPos, Room sourceRoom, Tile.Direction dir){
		if (sourceRoom != null) {
			//audioSource.clip = runeSound;
			runeSoundSource.Play ();
			sourceRoom.doRuneAnimations ();
		}
		yield return null;
		while (areAnyAnimationsOccurring ()) {
			yield return null;
		}

		GameObject roomObj = Instantiate (sourceRoom==null ? firstRoomPrefab : roomPrefab, transform.position, Quaternion.identity) as GameObject;
		Room thisRoom = roomObj.GetComponent<Room> ();
		thisRoom.setup (xPos, yPos, this);

		//audioSource.clip = revealSound;
		revealSoundSource.Play ();

		if (sourceRoom != null) {
			int runeSeedFromCurRoom = sourceRoom.getSeedFromRunes ();
			thisRoom.seedRoom (runeSeedFromCurRoom);
		} else {
			thisRoom.seedFirstRoom ();

			//add the player
			thisRoom.addTileToGrid (playerTilePrefab, 1, 1);
			playerTile = (TilePlayer) thisRoom.Grid [1, 1];
			playerTile.manager = this;
		}

		yield return null;
		while (areAnyAnimationsOccurring ()) {
			yield return null;
		}

		rooms.Add (thisRoom);

		//if there was a source room, we shoudl move the player if possible
		if (sourceRoom != null) {
			rooms [rooms.Count - 1].tryToMovePlayerToRoom (playerTile, dir);
		}
	}

	public void tryToChangePlayerRoom(Tile.Direction dir){
		int curRoomX = playerTile.room.xPos;
		int curRoomY = playerTile.room.yPos;

		int newRoomX = curRoomX;
		int newRoomY = curRoomY;

		if (dir == Tile.Direction.Up)	newRoomY += 1;
		if (dir == Tile.Direction.Down)	newRoomY -= 1;
		if (dir == Tile.Direction.Left)	newRoomX -= 1;
		if (dir == Tile.Direction.Right)newRoomX += 1;

		//does this room already exist?
		bool doesExist = false;
		for (int i = 0; i < rooms.Count; i++) {
			if (rooms [i].xPos == newRoomX && rooms [i].yPos == newRoomY) {
				doesExist = true;
				//check if the spot theplayer is trying to move into is clear
				bool didMove = rooms[i].tryToMovePlayerToRoom(playerTile, dir);
				if (!didMove) {
					playerTile.audioSource.clip = playerTile.bonkSound;
				} else {
					playerTile.audioSource.clip = playerTile.moveSounds[ (int)Random.Range(0, playerTile.moveSounds.Length)];
				}
				playerTile.audioSource.Play ();
				//if it is move the player there
			}
		}

		//if there was no room there, we need a new one
		if (!doesExist) {
			StartCoroutine( addRoom (newRoomX, newRoomY, playerTile.room, dir) );
		}
	}




	//returns true if any tile is doing an animation
	public bool areAnyAnimationsOccurring(){
		for (int i = 0; i < rooms.Count; i++) {
			if (rooms [i].areAnyAnimationsOccurring ()) {
				return true;
			}
		}

		if (playerTile != null) {
			if (playerTile.DoingAnimation) {
				return true;
			}
		}
			
		if (dyingAnimationObjects.Count > 0) {
			return true;
		}
		return false;
	}


	//setters and getters


	public List<GameObject> DyingAnimationObjects {
		get {
			return this.dyingAnimationObjects;
		}
		set {
			dyingAnimationObjects = value;
		}
	}

	public TilePlayer PlayerTile{
		get {
			return this.playerTile;
		}
	}
}
