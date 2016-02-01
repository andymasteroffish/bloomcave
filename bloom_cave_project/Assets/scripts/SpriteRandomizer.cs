using UnityEngine;
using System.Collections;

public class SpriteRandomizer : MonoBehaviour {

	public SpriteRenderer renderer;
	public Sprite[] sprites;

	// Use this for initialization
	void Start () {
		renderer.sprite = sprites [(int)Random.Range (0, sprites.Length)];
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
