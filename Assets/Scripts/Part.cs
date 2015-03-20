using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//A part can be bought through the computer to make pc components etc
//For now lets just say all parts are available from the get go, but some will obviously 
//be a little too pricey.

//idea for the future, markets which must be unlocked to get access to parts.
public class Part: IComparable<Part>  {

	public string name;
	public string description;
	public double price;
	public int numberOwned;

	public void buy(int numberToBuy){
		numberOwned += numberToBuy;
	}

	public void use(int numberToUse){
		numberOwned -= numberToUse;
	}

	public int CompareTo(Part b){
		Part a = this;
		if (a.price == b.price) {
			return 0;
		} else if (a.price > b.price) {
			return 1;
		} else
			return -1;
	}

}
