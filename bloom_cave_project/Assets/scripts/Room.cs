using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Room : MonoBehaviour {

	private GridManager manager;

	public int gridSize;
	private Tile [,] grid;

	private GameObject [,] indicators;			//KILL THIS
	public GameObject gridIndicatorPrefab;

	public GameObject revealTilePrefab;

	private List<TileRune> runes = new List<TileRune>();

	//[System.NonSerialized]
	public int xPos, yPos;

	//level generation
	public GameObject plantTilePrefab, runePrefab;

	public int minNumPlants, maxNumPlants;
	public int minNumRunes, maxNumRunes;
	public int numPlantSprites;

	//stuff for the first room
	public bool isFirstRoom;
	private List<RevealTile> firstRoomRevealTiles;
	public GameObject firstRoomBorder;
	public RevealTile gameTitle, byLine;

	public void setup(int _x, int _y, GridManager _manager){

		xPos = _x;
		yPos = _y;
		manager = _manager;

		gameObject.name = "room_" + xPos + "_" + yPos;

		float offset = (float)gridSize / 2.0f;
		//Debug.Log ("offset: " + offset);
		transform.position = new Vector3 (xPos * gridSize - offset, yPos * gridSize - offset);

		grid = new Tile[gridSize, gridSize];
		indicators = new GameObject[gridSize, gridSize];

		if (isFirstRoom) {
			firstRoomRevealTiles = new List<RevealTile>();
		}

		//add the grid indicators and default the grid to null
		for (int x=0; x<gridSize; x++) {
			for (int y=0; y<gridSize; y++) {
				GameObject newIndicator = Instantiate(gridIndicatorPrefab, Vector3.zero, new Quaternion(0,0,0,0)) as GameObject;
				newIndicator.transform.parent = transform;
				newIndicator.transform.localPosition = new Vector3 (x, y, 1);
				indicators[x,y] = newIndicator;
				grid[x,y] = null;

				//put a reveal tile here
				GameObject revealObj = Instantiate(revealTilePrefab, newIndicator.transform.position, Quaternion.identity) as GameObject;
				if (isFirstRoom) {
					RevealTile thisReveal = revealObj.GetComponent<RevealTile> ();
					if (x < 1 || x > 2 || y < 1 || y > 2) {
						thisReveal.isFrozen = true;
					} else {
						thisReveal.isFrozen = false;
					}
					firstRoomRevealTiles.Add (thisReveal);
				}
			}
		}

	}

	public void seedRoom(int seed){
		Random.seed = seed;

		//add runes
		int numRunes = (int)Random.Range (minNumRunes, maxNumRunes);
		for (int i = 0; i < numRunes; i++) {
			int xPos = (int)Random.Range (1, gridSize-1);
			int yPos = (int)Random.Range (1, gridSize-1);
			if (grid [xPos, yPos] == null) {
				addTileToGrid (runePrefab, xPos, yPos);
				TileRune thisRune = (TileRune)grid [xPos, yPos];
				runes.Add (thisRune);
			}
		}

		//add plants
		int middlePlantSprite = Random.Range(0, numPlantSprites);
		int numPlants = (int)Random.Range(minNumPlants, maxNumPlants);
		for (int i = 0; i < numPlants; i++) {
			int xPos = (int)Random.Range (0, gridSize);
			int yPos = (int)Random.Range (0, gridSize);
			if (grid [xPos, yPos] == null) {
				addTileToGrid (plantTilePrefab, xPos, yPos);
				grid [xPos, yPos].gameObject.SendMessage("setSprite",middlePlantSprite);
			}
		}
	}

	public void seedFirstRoom(){
		Random.seed = 0;

		addTileToGrid (runePrefab, 3, 3);
		TileRune thisRune = (TileRune)grid [3, 3];
		runes.Add (thisRune);
	}

	public void doRuneAnimations(){
		for (int i = 0; i < runes.Count; i++) {
			runes [i].startRuneAnimation ();
		}
	}

	public int getSeedFromRunes(){
		int total = 10;
		for (int i = 0; i < runes.Count; i++) {
			total += runes [i].getSeedVal ();
		}
		return total;
	}
	
	// Update is called once per frame
	void Update () {
		//the first room needs to check some stuff
		if (isFirstRoom) {

			//did the player leave the starting area?
			if (manager.PlayerTile.col < 1 || manager.PlayerTile.col > 2 || manager.PlayerTile.row < 1 || manager.PlayerTile.row > 2) {
				endFirstRoom ();
			}
		}

	}

	public void addTileToGrid(GameObject tilePrefab, int col, int row){
		GameObject newTileObj = Instantiate (tilePrefab, new Vector3 (0,0, 0), new Quaternion (0, 0, 0, 0)) as GameObject;
		newTileObj.transform.parent = transform;
		newTileObj.transform.localPosition = new Vector3 (col, row, 0);

		grid [col, row] = newTileObj.GetComponent<Tile> ();
		grid [col, row].setup (this);
		grid [col, row].setPos (col, row);
	}


	public bool tryToMovePlayerToRoom(TilePlayer player, Tile.Direction dir){

		int targetX = player.col;
		int targetY = player.row;

		if (dir == Tile.Direction.Up) targetY = 0;
		if (dir == Tile.Direction.Down) targetY = gridSize-1;
		if (dir == Tile.Direction.Left) targetX = gridSize-1;
		if (dir == Tile.Direction.Right) targetX = 0;

		//if that spot clear?
		if (grid [targetX, targetY] == null) {
			player.moveToNewRoom (this, targetX, targetY, dir);
			player.audioSource.clip = player.moveSounds[ (int)Random.Range(0, player.moveSounds.Length)];
			player.audioSource.Play ();
			return true;
		} else {

			//try moving whatever is in there to see if they can - or not. this will take too long

			//play the bonk sound
			player.audioSource.clip = player.bonkSound;
			player.audioSource.Play ();
			//if not, bounce it without doing anything
			return false;
		}

	}


	public bool areAnyAnimationsOccurring(){
		
		for (int i = 0; i < runes.Count; i++) {
			if (runes [i].DoingAnimation) {
				return true;
			}
		}

		return false;
	}


	public bool areRunesMoving(){
		for (int i=0; i<runes.Count; i++){
			if (runes[i].DoingAnimation){
				return true;
			}
		}
		return false;
	}


	void endFirstRoom(){
		Destroy (firstRoomBorder);
		gameTitle.isFrozen = false;
		byLine.isFrozen = false;
		for (int i = 0; i < firstRoomRevealTiles.Count; i++) {
			firstRoomRevealTiles [i].isFrozen = false;
		}
		firstRoomRevealTiles.Clear ();
		isFirstRoom = false;
		//manager.audioSource.clip = manager.revealSound;
		manager.revealSoundSource.Play ();
		GameObject.Find ("Main Camera").SendMessage ("endStart");
	}

	//setters getters

	public Tile[,] Grid {
		get {
			return this.grid;
		}
		set {
			grid = value;
		}
	}

	public GameObject[,] Indicators {
		get {
			return this.indicators;
		}
	}

	public GridManager Manager {
		get {
			return this.manager;
		}
	}



}
