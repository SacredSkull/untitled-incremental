using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Incremental.Database;
using UnityEngine.EventSystems;

// a Computer is hardware with a computer tag, it contains a list of
//software installed, as well as information on its operating system.
public class Computer : MonoBehaviour {

	public bool manned {
		get;
		set;
	}

	public Employee user {
		get;
		set;
	}

	public HardwareProject computerBuild {
		get;
		set;
	}

	public double processingPower {
		get{
			return computerBuild.processIncrease;
		}
	}

	public SoftwareProject primaryOS {
		get;
		set;
	}

	public SoftwareProject secondaryOS{
		get;
		set;
	}

	public List<SoftwareProject> InstalledPrograms = new List<SoftwareProject> ();

	//the total power, being used by currently installed programs
	public double usedPower{
		get{
			double power = 0.00;
			power+= primaryOS.ProcessReq;
			if(secondaryOS!= null){
				power+= primaryOS.ProcessReq;
			}
			foreach(SoftwareProject program in InstalledPrograms){
				power += program.ProcessReq;  
			}
			return power;
		}
	}

	public bool installing {
		get;
		private set;
	}

	//install software to run on your computer, there is a short waiting time between 
	//each install, this will be visualised in the GUI.
	public void installSoftware(SoftwareProject tobeInstalled, int number){
		if (tobeInstalled.canDoMultiple) {
			for (int i = 0; i<number; i++) {
				if(usedPower+tobeInstalled.ProcessReq < processingPower){
					System.Threading.Thread.Sleep ((int)(tobeInstalled.ProcessReq * (10000 / processingPower)));
					InstalledPrograms.Add (tobeInstalled);
				}
				else{
					break;
				}
				
			}
		} else {
			if(usedPower+tobeInstalled.ProcessReq < processingPower){
				System.Threading.Thread.Sleep ((int)(tobeInstalled.ProcessReq * (10000 / processingPower)));
				InstalledPrograms.Add (tobeInstalled);
			}
		}
	}

	public int pointsPerTick{
		get{
			int points = 0;
			int multiplier = 0;
			foreach(SoftwareProject s in InstalledPrograms){
				if(s.pointsPerTick>0){
					points+=s.pointsPerTick;
				}
				if(s.pointMult>0){
					multiplier+=s.pointMult;
				}
			}
			if (secondaryOS == null) {
				points += primaryOS.pointsPerTick;
				multiplier += primaryOS.pointMult;
			} else {
				points += ((primaryOS.pointsPerTick/2) + (secondaryOS.pointsPerTick/2));
				multiplier += ((primaryOS.pointMult/2) + (secondaryOS.pointsPerTick/2));
			}
			if(manned){
				return (points * multiplier);
			}
			else{
				return ((points * multiplier)/2);
			}
		}
	}

	public int moneyPerTick{
		get{
			int money = 0;
			int multiplier = 0;
			foreach(SoftwareProject s in InstalledPrograms){
				if(s.moneyPerTick>0){
					money += s.moneyPerTick;
				}
				if(s.moneyMult>0){
					multiplier += s.moneyMult;
				}
			}
			if (secondaryOS == null) {
				money += primaryOS.moneyPerTick;
				multiplier += primaryOS.moneyMult;
			} else {
				money += ((primaryOS.moneyPerTick/2) + (secondaryOS.moneyPerTick/2));
				multiplier += ((primaryOS.moneyMult/2) + (secondaryOS.moneyPerTick/2));
			}
			if(manned){
				return (money * multiplier);
			}
			else{
				return ((money * multiplier)/2);
			}
		}
	}

	public int pointsPerClick{
		get{
			int points = 0;
			int multiplier = 0;
			foreach(SoftwareProject s in InstalledPrograms){
				if(s.pointsPerClick > 0){
					points+=s.pointsPerClick;
				}
				if(s.pointMultPerClick>0){
					multiplier+=s.pointMultPerClick;
				}
			}
			if (secondaryOS == null) {
				points += primaryOS.pointsPerClick;
				multiplier += primaryOS.pointMultPerClick;
			} else {
				points += ((primaryOS.pointsPerClick/2) + (secondaryOS.pointsPerClick/2));
				multiplier += ((primaryOS.pointMultPerClick/2) + (secondaryOS.pointMultPerClick/2));
			}
			if(manned){
				return (points * multiplier);
			}
			else{
				return 0;
			}
		}

	}
}

