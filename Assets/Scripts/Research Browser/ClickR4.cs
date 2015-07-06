using UnityEngine;
using System.Collections;

public class ClickR4 : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnMouseDown(){
		GameController game = GameController.instance;
		int ID = GameObject.Find("r4").GetComponent<ResearchID>().ID;
		if (game.AllUncompleteResearch.ContainsKey (ID)) {
			game.startResearch(game.AllUncompleteResearch[ID]);
		}
	}
}
