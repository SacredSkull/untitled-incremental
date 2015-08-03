using System;
using System.Collections;
using System.Collections.Generic;

public class Office : Asset {
	public int x;
	public int y;
	public TileMap floorPlan;
	public int itemID = 0;
	public Dictionary<int,Startable> placedEquipment = new Dictionary<int, Startable> ();

	private int _ID;
	public int ID{
		get{
			if(_ID == null){
				_ID = GameController.instance.offID;
				GameController.instance.offID++;
			}
			return _ID;
		}
		set{
			_ID = value;
		}
	}

	public void placeEquipment(Placable p, int xPos, int yPos){
		if(floorPlan.canAddPlacable(p,xPos,yPos)){
			floorPlan.addPlacable(p, itemID, xPos, yPos);
			placedEquipment.Add(itemID,p);
			itemID++;
		}
	}

	public int _cost;

	public int cost{
		get {
			if (_cost == null) {
				Random rnd = new Random();
				int rand = rnd.Next(1,10);
				_cost = floorPlan.tileNumber * 100 * rand;
			}
			return _cost;
		}
	}

	void awake(){
		floorPlan = new TileMap (x, y);
	}
}
