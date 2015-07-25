using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Incremental.Database;

public class ComputerController : ScriptableObject {

	public List<HardwareProject> AllCompletedComputers = new List<HardwareProject>();

	public Computer mannedComputer {
		get;
		set;
	}

	public void setMannedComputer(HardwareProject comp){
		Computer parse = new Computer();
		parse.computerBuild = comp;
		parse.manned = true;
		mannedComputer = parse;
	}

	public List<Computer> activeUnmannedComputers = new List<Computer> ();

	public int getPointsPerTick{
		get{
			int points = 0;
			points+= mannedComputer.pointsPerTick;
			foreach(Computer comp in activeUnmannedComputers){
				points+= comp.pointsPerTick;
			}
			return points;
		}
	}

	public int getPointsPerClick{
		get{
			int points = 0;
			points+= mannedComputer.pointsPerClick;
			foreach(Computer comp in activeUnmannedComputers){
				points+= comp.pointsPerClick;
			}
			return points;
		}
	}

	public int getMoneyPerTick{
		get{
			int points = 0;
			points+= mannedComputer.moneyPerTick;
			foreach(Computer comp in activeUnmannedComputers){
				points+= comp.moneyPerTick;
			}
			return points;
		}
	}
}
