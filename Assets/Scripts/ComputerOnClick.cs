using UnityEngine;
using System.Collections;

public class ComputerOnClick : MonoBehaviour {
	public int researchPerClick;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//Basic idea is, that the player gains no benefit from clicking unless
	//research has been set, then they produce only the points necessary to finish it.
	void OnMouseDown(){
		GameController game = GameController.instance;
		if((game.isResearchSet() && game.currentResearch.researchCost > game.researchPoints) || game.debugResearchPoints){
			game.addResearchPoints(researchPerClick);
		}
		else if(game.isResearchSet()){
			game.stopResearch();
		}
        
	}
}
