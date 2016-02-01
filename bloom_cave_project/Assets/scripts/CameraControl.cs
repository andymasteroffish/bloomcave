using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

	public GridManager manager;
	private TilePlayer player;
	private float stepSize;
	public float timeBetweenMoves;
	private float moveTimer;

	private bool doingStart;

	// Use this for initialization
	void Start () {
		doingStart = true;
		stepSize = 1.0f / 8.0f;
		moveTimer = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (doingStart) {
			return;
		}

		moveTimer -= Time.deltaTime;

		if (!manager.areAnyAnimationsOccurring() && moveTimer<0 && player != null) {
			Vector3 targetPos = player.room.transform.position;
			targetPos.x += (float)player.room.gridSize / 2.0f - stepSize*6;
			targetPos.y += (float)player.room.gridSize / 2.0f - stepSize*6;
			targetPos.z = -10;

			Vector3 newPos = transform.position;
			if (newPos.x > targetPos.x + stepSize){
				newPos.x -= stepSize;
			}
			if (newPos.x < targetPos.x - stepSize){
				newPos.x += stepSize;
			}
			if (newPos.y > targetPos.y + stepSize){
				newPos.y -= stepSize;
			}
			if (newPos.y < targetPos.y - stepSize){
				newPos.y += stepSize;
			}

			transform.position = newPos;
			moveTimer = timeBetweenMoves;
		}
	
	}

	public void setPlayer(TilePlayer _player){
		player = _player;
	}

	public void endStart(){
		doingStart = false;
		//transform.position = new Vector3 (0, 0, -10);
	}
}
