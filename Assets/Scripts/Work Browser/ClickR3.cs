using UnityEngine;
using System.Collections;

public class ClickR3 : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnMouseDown(){
		GameController game = GameController.instance;
		int ID = GameObject.Find("r3").GetComponent<WorkID>().ID;
		if (game.chapter == GameController.pickedType.Research) {
			if (game.AllUncompleteResearch.ContainsKey (ID)) {
				game.startResearch (game.AllUncompleteResearch [ID]);
			}
		} else if (game.chapter == GameController.pickedType.Software) {
			if (game.UnstartedSoftware.ContainsKey (ID)) {
				game.startSoftware (game.UnstartedSoftware [ID]);
			}
		} else if (game.chapter == GameController.pickedType.Hardware) {
			if (game.UnstartedHardware.ContainsKey (ID)) {
				game.makeHardware (game.UnstartedHardware [ID]);
			}
		} else if (game.chapter == GameController.pickedType.Parts) {
			if (game.allBuyableParts.ContainsKey (ID)) {
				game.buyPart (game.allBuyableParts [ID],1);
			}
		} else {
			game.setChapterToHardware();
		}
	}
}
