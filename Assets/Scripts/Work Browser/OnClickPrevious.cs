﻿using UnityEngine;
using System.Collections;

public class OnClickPrevious : MonoBehaviour {



	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void OnMouseDown(){
		GameController game = GameController.instance;
		game.previous ();
	}
}