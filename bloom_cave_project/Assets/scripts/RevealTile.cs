using UnityEngine;
using System.Collections;

public class RevealTile : MonoBehaviour {

	public SpriteRenderer spriteRenderer;

	public Sprite[] sprites;

	private int curFrame;
	private float timer;
	public float minFrameTime, maxFrameTime;

	public bool isFrozen;
	public bool isTitle;
	private Vector3 startPos;
	public float minTimeForTitleMove, maxTimeForTitleMove;
	private float titleTimer;

	public int titleRange;
	private int numXSteps, numYSteps;

	// Use this for initialization
	void Start () {
		GridManager manager = GameObject.Find("grid").GetComponent<GridManager>();
		if (!isFrozen) {
			manager.DyingAnimationObjects.Add (gameObject);
		}
		curFrame = 0;
		spriteRenderer.sprite = sprites [0];
		timer = Random.Range(minFrameTime,maxFrameTime);
		if (isTitle) {
			titleTimer = maxTimeForTitleMove;
			startPos = transform.position;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (!isFrozen) {
			timer -= Time.deltaTime;
		}
		if (timer <= 0) {
			curFrame++;
			timer = Random.Range (minFrameTime, maxFrameTime);
			if (curFrame >= sprites.Length) {
				Destroy (gameObject);
			} else {
				spriteRenderer.sprite = sprites [curFrame];
			}
		}

		if (isTitle) {
			titleTimer -= Time.deltaTime;
			if (titleTimer < 0) {
				Vector3 newPos = startPos;
				float step = 1.0f/8.0f;

				if (Random.value > 0.5f) {
					if (Random.value > 0.5f) {
						numXSteps ++;
					} else {
						numXSteps --;
					}
				} else {
					if (Random.value > 0.5f) {
						numYSteps++;
					} else {
						numYSteps--;
					}
				}

				if (numXSteps < -titleRange)	numXSteps = -titleRange;
				if (numXSteps >  titleRange)	numXSteps = titleRange;
				if (numYSteps < -titleRange)	numYSteps = -titleRange;
				if (numYSteps >  titleRange)	numYSteps = titleRange;

				newPos.x = startPos.x + numXSteps * step;
				newPos.y = startPos.y + numYSteps * step;

				transform.position = newPos;

				titleTimer = Random.Range (minTimeForTitleMove, maxTimeForTitleMove);
			}

		}
	}
}
