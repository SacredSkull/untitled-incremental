using UnityEngine;
using System.Collections;

public class CloseOnClick : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void onMouseDown(){
		GameController game = GameController.instance;
		game.info.active = false;
	}
}
