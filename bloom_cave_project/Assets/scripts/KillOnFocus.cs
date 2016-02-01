using UnityEngine;
using System.Collections;

public class KillOnFocus : MonoBehaviour {

	public SpriteRenderer sprite;

	// Use this for initialization
	void Start () {
	
	}
	
	void OnApplicationFocus(bool focusStatus) {
		sprite.enabled = !focusStatus;
	}

}
