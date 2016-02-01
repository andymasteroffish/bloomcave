using UnityEngine;
using System.Collections;

public class ColorChanger : MonoBehaviour {

	public SpriteRenderer spriteRenderer;
	public Color[] animationColors;
	public float animationFrameTime;
	private float timer;
	private int curID;

	// Use this for initialization
	void Start () {
		timer = 0;
		curID = 0;
	}
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;
		if (timer > animationFrameTime) {
			timer = 0;
			curID++;
			if (curID >= animationColors.Length) {
				curID = 0;
			}
			spriteRenderer.color = animationColors [curID];
		}
	}
}
