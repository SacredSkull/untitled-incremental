﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Incremental.XML;

public class HardwareProject :  Part {

	public List<Part> requiredParts = new List<Part>();

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