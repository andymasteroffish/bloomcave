using UnityEngine;
using System.Collections;

public class TileRune : Tile {

	private int colorID;
	private int runeID;
	private int stoneID;

	public SpriteRenderer stoneRenderer;

	public Color[] runeColors;
	public Sprite[] runeSprites, stoneSprites;

	private Color[] animationColors;
	public Color[] animationColors0;
	public Color[] animationColors1;
	public Color[] animationColors2;
	public Color[] animationColors3;
	//public Color[] animationColors4;
	public int animationCycles;
	public float animationFrameTime;


	public override void typeSetup(){
		colorID = (int)Random.Range (0, runeColors.Length);
		runeID = (int)Random.Range (0, runeSprites.Length);
		stoneID = (int)Random.Range (0, stoneSprites.Length);

		spriteRenderer.sprite = runeSprites [runeID];
		spriteRenderer.color = runeColors [colorID];
		stoneRenderer.sprite = stoneSprites [stoneID];

		if (colorID == 0)	animationColors = animationColors0;
		if (colorID == 1)	animationColors = animationColors1;
		if (colorID == 2)	animationColors = animationColors2;
		if (colorID == 3)	animationColors = animationColors3;
		//if (colorID == 4)	animationColors = animationColors4;
	}

	public int getSeedVal(){
		return colorID * col + runeID * row;
	}

	public void startRuneAnimation(){
		StartCoroutine (doRuneAnimation ());
	}

	IEnumerator doRuneAnimation(){
		DoingAnimation = true;
		float timer = 0;

		int curFrame = 0;
		int curCycle = 0;

		while (curCycle < animationCycles) {
			spriteRenderer.color = animationColors [curFrame];

			yield return new WaitForSeconds(animationFrameTime);

			curFrame++;
			if (curFrame >= animationColors.Length){
				curFrame = 0;
				curCycle ++;
			}
		}

		spriteRenderer.color = runeColors [colorID];
		DoingAnimation = false;
	}
}
