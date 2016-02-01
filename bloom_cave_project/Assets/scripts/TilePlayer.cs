using UnityEngine;
using System.Collections;

public class TilePlayer : Tile {

	public GridManager manager;
	public Sprite[] playerSprites;

	//private Room curRoom;

	public float baseMoveSoundVol;

	public AudioClip bonkSound;

	public override void typeSetup(){
		spriteRenderer.sprite = playerSprites [(int)Random.Range(0,playerSprites.Length)];
		GameObject.Find ("Main Camera").SendMessage("setPlayer", this);
	}

	public override void typeUpdate(){

		if (!manager.areAnyAnimationsOccurring()) {

			if (Input.GetKeyDown (KeyCode.LeftArrow)) {
				tryToMove(Tile.Direction.Left);
			}
			if (Input.GetKeyDown (KeyCode.RightArrow)) {
				tryToMove(Tile.Direction.Right);
			}
			if (Input.GetKeyDown (KeyCode.UpArrow)) {
				tryToMove(Tile.Direction.Up);
			}
			if (Input.GetKeyDown (KeyCode.DownArrow)) {
				tryToMove(Tile.Direction.Down);
			}

		}

		//if any tiles are moving, mute our sound
		if (room.areRunesMoving ()) {
			audioSource.volume = 0;
			if (audioSource.isPlaying) {
				audioSource.Stop ();
			}
		} else {
			audioSource.volume = baseMoveSoundVol;
		}

		//camObject.transform.position = transform.position + new Vector3 (0, 0, -10);

	}

	public void tryToMove(Tile.Direction direction){
		bool didMove = false;

		//chekc if the player might be hopping rooms
		if (direction == Tile.Direction.Left && col == 0) {
			manager.tryToChangePlayerRoom (Tile.Direction.Left);
			return;
		}
		if (direction == Tile.Direction.Right && col == room.gridSize-1) {
			manager.tryToChangePlayerRoom (Tile.Direction.Right);
			return;
		}
		if (direction == Tile.Direction.Up && row == room.gridSize-1) {
			manager.tryToChangePlayerRoom (Tile.Direction.Up);
			return;
		}
		if (direction == Tile.Direction.Down && row == 0) {
			manager.tryToChangePlayerRoom (Tile.Direction.Down);
			return;
		}

		//check normal moves
		if (direction == Tile.Direction.Left) {
			didMove = checkCanMove (-1, 0, true);
		}
		if (direction == Tile.Direction.Right) {
			didMove = checkCanMove (1, 0, true);
		}
		if (direction == Tile.Direction.Up) {
			didMove = checkCanMove (0, 1, true);
		}
		if (direction == Tile.Direction.Down) {
			didMove = checkCanMove (0, -1, true);
		}
		
		if (didMove) {
			audioSource.clip = moveSounds [(int)Random.Range (0, moveSounds.Length)];
			audioSource.Play ();
			//manager.endTurn ();
		} else {
			audioSource.clip = bonkSound;
			audioSource.Play ();
		}
	}


	public void moveToNewRoom(Room newRoom, int newCol, int newRow, Tile.Direction dir){
		
		//empty the spot in the old room
		room.Grid [col, row] = null;

		//set our room
		room = newRoom;
		transform.parent = room.transform;

		//set our X and Y in that room
		col = newCol;
		row = newRow;

		//set the grid to store where this tile is now
		room.Grid [col, row] = this;

		//move animation
		Vector3 offset = new Vector3(0,0,0);
		if (dir == Tile.Direction.Up) offset.y = 1;
		if (dir == Tile.Direction.Down) offset.y = -1;
		if (dir == Tile.Direction.Left) offset.x = -1;
		if (dir == Tile.Direction.Right) offset.x = 1;

		StartCoroutine (doMoveAnimation (transform.localPosition + offset));

	}

}
