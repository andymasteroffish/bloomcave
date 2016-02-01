using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {

	public enum TileType{Blank, Plant, Rune, Player};
	public enum Direction{Up, Left, Down, Right};

	public bool positionLocked;

	public SpriteRenderer spriteRenderer;

	public int col, row;
	public TileType tileType;

	public Room room;

	private bool mouseIsOver;

	//marking it as innactive when it is first placed
	private bool isInactive;


	//animations
	private bool doingAnimation;
	private float moveTime = 0.03f;//0.2f;
	private float innactiveAnimationTime = 0.2f;


	public AudioSource audioSource;
	public AudioClip[] moveSounds;

	public void setup(Room _room){
		room = _room;
		mouseIsOver = false;
		doingAnimation = false;
		typeSetup ();


	}
	public virtual void typeSetup(){}
	
	// Update is called once per frame
	void Update () {

		typeUpdate ();
	}
	public virtual void typeUpdate(){}

	public void resolve(TileType currentType){
		//Debug.Log (tileType + " is trying");
		if (currentType == tileType && !isInactive) {
			typeResolve();
		}
	}
	public virtual void typeResolve(){}


	public void setPos(int _col, int _row){
		col = _col;
		row = _row;
	}
		
	public bool checkCanMove(int xDir, int yDir, bool doMoveIfAble){
		//if this would put us out of range, return false
		if (col + xDir < 0 || col + xDir >= room.gridSize || row + yDir < 0 || row + yDir >= room.gridSize) {
			return false;
		}

		if (positionLocked) {
			return false;
		}

		//return true if the spot we'd be trying to move into is empty
		if (room.Grid [col + xDir, row + yDir] == null) {
			if (doMoveIfAble) {
				move (xDir, yDir);
			}
			return true;
		}
		//if there was somehting there, check if it can move
		else {
			Tile blockingTile = room.Grid[col + xDir, row + yDir];
			bool canMove = blockingTile.checkCanMove(xDir, yDir, doMoveIfAble);
			//if the other block has room to slide, do it
			//but if a non-player block would move into a player block's space, that is OK, and they can be crushed
			if (canMove || (tileType != TileType.Player && blockingTile.tileType == TileType.Player)){
				if(doMoveIfAble){
					move(xDir, yDir);
				}
				return true;
			}
		}
		

		//if we get here somehow, say it's impossible
		return false;
	}

	public void move(int xDir, int yDir){
		//transform.localPosition += new Vector3 (xDir, yDir, 0);

		room.Grid [col, row] = null;
		col += xDir;
		row += yDir;

		//did we crush a player?
		if (room.Grid [col, row] != null) {
			if (room.Grid [col, row].tileType == TileType.Player) {
				Debug.Log("YOU DEAD");
				room.Grid [col, row].killTile();
			}
		}

		//set the grid to store where this tile is now
		room.Grid [col, row] = this;

		if (audioSource != null) {
			audioSource.clip = moveSounds [(int)Random.Range (0, moveSounds.Length)];
			audioSource.Play ();
		}

		StartCoroutine (doMoveAnimation (transform.localPosition + new Vector3 (xDir, yDir, 0)));
	}

	public IEnumerator doMoveAnimation(Vector3 targetPos){
		doingAnimation = true;
		//float timer = 0;
		Vector3 startPos = transform.localPosition;

		int moveCount = 0;

		while (moveCount < 8) {
			moveCount++;
			float prc = Mathf.Clamp( (float)moveCount /8.0f, 0, 1);
			transform.localPosition = Vector3.Lerp(startPos, targetPos, prc);
			yield return new WaitForSeconds(moveTime);
		}

//		while (timer < moveTime) {
//			timer += Time.deltaTime;
//			float prc = Mathf.Clamp( timer /moveTime, 0, 1);
//			Vector3 newPos = getPixelLockedVector(Vector3.Lerp(startPos, targetPos, prc));
//			Debug.Log (newPos.x);
//			transform.localPosition = newPos;
//			yield return null;
//		}

		doingAnimation = false;
		//transform.localPosition = targetPos;
		transform.localPosition = new Vector3(col, row, 0);
	}

	public Vector3 getPixelLockedVector(Vector3 source){
		Vector3 returnVal = new Vector3 (source.x, source.y, source.z);

		float step = 1.0f / 8.0f;	//there are 8 pixels per unit

		float fullSteps = (int)Mathf.Floor (source.x);
		float remainder = source.x - fullSteps;
		int numSmallSteps = (int)(remainder / step);
		Debug.Log (fullSteps + " " +numSmallSteps);

		returnVal.x = fullSteps + (float)numSmallSteps * step;

		return returnVal;
	}

	public void setInnactive(bool newVal){
		isInactive = newVal;
		StartCoroutine( doInnactiveAnimation (newVal));
	}

	IEnumerator doInnactiveAnimation (bool newVal){
		doingAnimation = true;
		float greyVal = 0.3f;
		float startCol 	 = newVal ? 1f : greyVal;
		float endCol	 = newVal ? greyVal : 1f;

		float timer = 0;

		while (timer < innactiveAnimationTime) {
			timer += Time.deltaTime;
			float prc = Mathf.Clamp( timer/innactiveAnimationTime , 0f, 1f);
			float newCol = prc * endCol + (1-prc) * startCol;
			spriteRenderer.color = new Color( newCol, newCol, newCol, 1f);
			yield return null;
		}

		spriteRenderer.color = new Color (endCol, endCol, endCol, 1);
		doingAnimation = false;
	}

	public void killTile(){
		doPreDeathEffects ();
		StartCoroutine ( doKillAnimation() );
		room.Grid [col, row] = null;
	}
	public virtual void doPreDeathEffects(){ }

	IEnumerator doKillAnimation(){
		Debug.Log ("tile starts to die");
		doingAnimation = true;
		room.Manager.DyingAnimationObjects.Add (gameObject);
		float timer = 0;
		Vector3 startScale = transform.localScale;

		while (timer < moveTime) {
			timer += Time.deltaTime;
			float prc = Mathf.Clamp( timer /moveTime, 0, 1);
			transform.localScale = Vector3.Lerp(startScale, new Vector3(0,0,0), prc);
			yield return null;
		}

		doingAnimation = false;
		doPostDeathEffects ();
		Destroy (gameObject);

	}

	public virtual void doPostDeathEffects(){ }



	void OnMouseEnter(){
		mouseIsOver = true;
	}
	void OnMouseExit(){
		mouseIsOver = false;
	}


	//some general things
	public Tile getTileInDir(int xDir, int yDir){
		if (col+xDir < 0 || col+xDir >= room.gridSize || row+yDir < 0 || row+yDir >= room.gridSize){
			return null;
		}

		return room.Grid [col + xDir, row + yDir];
	}


	//setters and getters

	public bool DoingAnimation {
		get {
			return this.doingAnimation;
		}
		set {
			doingAnimation = value;
		}
	}

	public bool IsInactive {
		get {
			return this.isInactive;
		}
	}
}
