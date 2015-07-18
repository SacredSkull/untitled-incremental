using UnityEngine;
using System.Collections;

public class ComputerOnClick : MonoBehaviour {


	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//Basic idea is, that the player gains no benefit from clicking unless
	//research has been set, then they produce only the points necessary to finish it.
	public void OnMouseDown(){
		GameController game = GameController.instance;
		ResearchController rControl = ResearchController.instance;
		SoftwareController sControl = SoftwareController.instance;
		if (rControl.isResearchSet()||sControl.isSoftwareSet) {
			game.addResearchPoints (game.pointsPerClick); 
		} else {
			if(game.justFinished == 0){
				game.picker.active=false;
				game.browser.active=true;
			}

		}

	}
}
