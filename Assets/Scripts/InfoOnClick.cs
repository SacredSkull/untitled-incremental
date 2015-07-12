using UnityEngine;
using System.Collections;

public class InfoOnClick : MonoBehaviour {



	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void onMouseDown(){
		GameController game = GameController.instance;
		if(game.info.active == false){
			game.info.active = true;
		}

	}
}
