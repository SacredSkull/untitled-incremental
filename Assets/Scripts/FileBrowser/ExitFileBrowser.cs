using UnityEngine;
using System.Collections;

public class ExitFileBrowser : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void onMouseDown(){
		GameController game = GameController.instance;
		game.showPicker ();
		game.browser.active=false;
	}
}
