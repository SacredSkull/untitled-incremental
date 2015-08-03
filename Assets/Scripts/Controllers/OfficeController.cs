using System;
using System.Collections;
using System.Collections.Generic;

public class OfficeController : Asset {

	public enum tier
	{
		Small,
		Medium,
		Large
	}
	public Dictionary<int,Office> allOwnedOffices = new Dictionary<int,Office>();

	public Dictionary<int,Office> rolledOffices = new Dictionary<int,Office>();

	public void generateRolledOffices(tier size){
		int maxX;
		int minX;
		int maxY;
		int minY;
		int X;
		int Y;
		Random rnd = new Random ();
		if (size == tier.Small) {
			maxX = 21;
			minX = 11;
			maxY = 31;
			minY = 16;
		} else if (size == tier.Medium) {
			maxX = 51;
			minX = 26;
			maxY = 81;
			minY = 36;
		} else {
			maxX = 101;
			minX = 51;
			maxY = 141;
			minY = 66;
		}
		for(int i = 1; i<=5; i++){
			X = rnd.Next (minX, maxX);
			Y = rnd.Next (minY, maxY);
			Office temp = new Office(X,Y);
			rolledOffices.Add(i,temp);
		}
	}

	public void rentOffice(int position){
		allOwnedOffices.Add (rolledOffices[position].ID, rolledOffices [position]);
		rolledOffices.Remove(position);
	}

	public int costsPerSecond(){
		int cost = 0;
		foreach(Office o in allOwnedOffices.Values){
			cost+= o.cost;
		}
	}

}
