using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Incremental.Database;

public class PartController : ScriptableObject {

	public Dictionary<int,Part> allBuyableParts{
		get {
			Dictionary<int,Part> temp = new Dictionary<int,Part>();
			foreach (Part item in GameController.instance.allParts)
			{	
				if(item.isBuyable()){
					temp.Add(item.ID,item);
				}
			}
			return temp;
		}
	}
	
	// Dictionary containing the part ID and quantity
	public readonly Dictionary<int, int> partInventory = new Dictionary<int, int>();
	
	bool hasParts(Part p, int number) {
		return partInventory[p.ID] >= number;
	}
	
	public void buyPart(Part p, int number){
		if (GameController.instance.money >= (p.cost * number)) {
			GameController.instance.money -= (p.cost * number);
			if(partInventory.ContainsKey(p.ID)){
				partInventory[p.ID] += number;
			}
			else{
				partInventory.Add(p.ID,number);
			}
			
		}
		else{
			// \todo Exceptions
		}
	}
	
	public void use(Part p, int number){
		partInventory[p.ID] -= number;
	}
}
