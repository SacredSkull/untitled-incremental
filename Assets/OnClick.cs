using UnityEngine;
using System.Collections;

public class OnClick : MonoBehaviour {

	public int researchPerClick = 10;
	private GameController gameController;

	// Use this for initialization
	void Start () {
		GameObject gameControllerObject = GameObject.FindWithTag ("GameController");
		if (gameControllerObject != null) {
			gameController = gameControllerObject.GetComponent<GameController>();
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnMouseDown(){
		gameController.addResearchPoints (researchPerClick);
	}
}
