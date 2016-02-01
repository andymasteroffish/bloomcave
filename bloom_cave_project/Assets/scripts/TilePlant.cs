using UnityEngine;
using System.Collections;

public class TilePlant : Tile {

	public Sprite[] sprites;

	public float noiseRange;
	//public float noiseCurve;

	public override void typeSetup(){
		

		//sometiems flip the sprite
		if (Random.value > 0.5f) {
			transform.localScale = new Vector3 (-1, 1, 1);
		}

	}

	public void setSprite(int middleSprite){
		//int middleSprite = (int)(roomPrc * (float)sprites.Length);

		float noiseVal = Mathf.PerlinNoise (Random.value * 100, 0);
		float noisePrc = noiseVal * 2.0f - 1.0f;
		//noisePrc = Mathf.Pow (noisePrc, noiseCurve);

		Debug.Log ("middle sprite: " + middleSprite);
		Debug.Log ("noise prc: " + noisePrc);

		int offset = (int)(noisePrc * noiseRange);
		Debug.Log ("offset: " + offset);
		int newSprite = middleSprite + offset;
		Debug.Log ("new sprite: " + newSprite);
		if (newSprite < 0) {
			newSprite = 0;
		}
		if (newSprite >= sprites.Length) {
			newSprite = sprites.Length - 1;
		}
		//newSprite = (int) Mathf.Clamp (newSprite, 0, sprites.Length);
		//Debug.Log ("new sprite: " + newSprite);

		//spriteRenderer.sprite = sprites[ (int)Random.Range(0,sprites.Length) ];
		spriteRenderer.sprite = sprites[ newSprite ];
	}
}
