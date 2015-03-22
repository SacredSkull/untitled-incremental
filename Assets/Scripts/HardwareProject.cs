using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Incremental.XML;

public class HardwareProject :  Part {

	//public string name;
	//public string description;
	public int pointsPerSecond;
	public int pointsPerClick;
	public int moneyperSecond;
	public int processIncrease;
	public int moneyMult;
	public int pointMult;
	//public int oneTimeFees;
	//public int upkeepCost(will not be used);
	public int pointCost;
	//public int uses;

	public Dictionary<Part, int> requiredParts = new Dictionary<Part, int>();

	public override bool canDo(){
		GameController game = GameController.instance;
		for(int i = 0; i<researchIDsRequired.Length; i++){
			if(!game.hasBeenDone(researchIDsRequired[i])){
				return false;
			}
		}
		for (int j = 0; j<requiredParts.Count; j++) {
			if(!game.hasParts(requiredParts[j].name, requiredParts.numberOwned)){
				return false;
			}
		}
		return true;
	}
}
