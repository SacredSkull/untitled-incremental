using UnityEngine;
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
		int? ID = GameObject.Find("r1").GetComponent<WorkID>().ID;
		if (game.chapter == GameController.pickedType.Research) {
			if (ID!=null&& game.AllUncompleteResearch.ContainsKey ((int)ID)) {
				game.startResearch (game.AllUncompleteResearch [(int)ID]);
			}
		} else if (game.chapter == GameController.pickedType.Software) {
			if (ID!=null&& game.UnstartedSoftware.ContainsKey ((int)ID)) {
				game.startSoftware (game.UnstartedSoftware [(int)ID]);
			}
		} else if (game.chapter == GameController.pickedType.Hardware) {
			if (ID!=null&& game.UnstartedHardware.ContainsKey ((int)ID)) {
				game.makeHardware (game.UnstartedHardware [(int)ID]);
			}
		} else if (game.chapter == GameController.pickedType.Parts) {
			if (ID!=null&& game.allBuyableParts.ContainsKey ((int)ID)) {
				game.buyPart (game.allBuyableParts [(int)ID],1);
			}
		} else {
			game.setChapterToResearch();
		}

	}
}
