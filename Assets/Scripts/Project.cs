using UnityEngine;
using System.Collections;

public class Project : MonoBehaviour {

	public string name;
	public string description;
	public int[] researchIDsRequired;
	public int pointsPerSecond;
	public int moneyperSecond;
	public int oneTimeFees;
	public int upkeepCost;
	public int pointCost;

	public bool canDo(){
		GameController game = GameController.instance;
		for(int i = 0; i<researchIDsRequired.Length; i++){
			if(!game.hasBeenDone(researchIDsRequired[i])){
				return false;
			}
		}
		return true;
	}

}
