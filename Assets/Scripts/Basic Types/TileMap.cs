using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileMap : MonoBehaviour {

	public int xDimension;

	public int yDimension;

	public Dictionary<int,Dictionary<int,Tile>> floorPlan = new Dictionary<int,Dictionary<int,Tile>>();

	public void canAddPlacable(Placable equip, int x, int y){
		int xLimit = x + equip.xDimension - 1;
		int yLimit = y + equip.yDimension - 1;
		for(int i = y; i<= yLimit; i++){
			for(int j = x; j <= xLimit; j++){
				if(!floorPlan[i][j].filled){
					return false;
				}
			}
		}
		return true;
	}

	public void addPlacable(Placable equip, int key, int x, int y){
		int xLimit = x + equip.xDimension - 1;
		int yLimit = y + equip.yDimension - 1;
		for(int i = y; i<= yLimit; i++){
			for(int j = x; j <= xLimit; j++){
				floorPlan[i][j].filled = true;
				floorPlan[i][j].itemID = key;
			}
		}
	}

	public TileMap(int x,int y){
		xDimension = x;
		yDimension = y;
		Tile temp = new Tile ();
		Dictionary<int,Tile> row = new Dictionary<int, Tile> ();
		for (int i = 1; i<= x; i++) {
			row.Add(i,temp);
		}
		for (int j = 1; j <= y; j++) {
			floorPlan.Add(j,row);
		}
	}

	public int tileNumber{
		get{
			return (xDimension * yDimension);
		}
	}
}
