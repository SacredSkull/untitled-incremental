using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Project  {

	public string name;
	public string description;
	public int[] researchIDsRequired;
	public int pointsPerSecond;
	public double moneyperSecond;
	//cost you have to pay to create the project
	public double oneTimeFees;
	public double processingBoost;
	public int pointCost;
	public bool done;

	public bool canDo(){
		GameController game = GameController.instance;
		for(int i = 0; i<researchIDsRequired.Length; i++){
			if(!game.hasBeenDone(researchIDsRequired[i])){
				return false;
			}
		}
		return true;
	}

	public void complete(){
		this.done = true;
	}
}
