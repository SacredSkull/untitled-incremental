﻿using UnityEngine;
using System.Collections;

public class ClickR1 : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnMouseDown(){
		GameController game = GameController.instance;
		int ID = int.Parse(GameObject.Find("r1").tag);
		if (game.AllUncompleteResearch.ContainsKey (ID)) {
			game.startResearch(game.AllUncompleteResearch[ID]);
		}
	}
}
