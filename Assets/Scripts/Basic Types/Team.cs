using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Team : MonoBehaviour {
	public enum goalType
	{
		None,
		Research,
		Software
	}

	public goalType goal;
	public Research researchProject;
	public SoftwareProject softProject;
	public int pointCost;

	public List<Employee> members = new List<Employee> ();

	public Employee leader;

	public Team(List<Employee> group){
		leader = group [0];
		group.Remove (leader);
		foreach (Employee e in group) {
			members.Add (e);
		}
	}

	public int ID{
		get{
			return leader.ID;
		}
	}

	public void setProject(int ID, goalType go){
		if(go = goalType.Research){
			researchProject = GameController.instance.allResearch.Find (r => r.ID == researchProject.ID);
			pointCost = groupResearchCost();
			goal = go;
		}

	}

	//calculates the cost of doing research for a group of people, with bonuses 
	//for more people doing it, and people who already know it helping. Algorithm 
	//is hidden from user. GUI should be designed so that every time someone is added 
	//to the team we see cost change 
	int groupResearchCost(){
		int points = 0;
		int teacher = 0;
		int students = 0;
		int cost = GameController.instance.allResearch.Find(r => r.ID == researchProject.ID).cost;
		foreach (Employee e in members) {
			if(e.me){
				if(GameController.instance.rControl.hasBeenDone(researchProject.ID)){
					points += cost;
					students ++;
				}
				else{
					teacher ++;
				} 
			}
			else if(e.employeeResearch.hasBeenDone(researchProject.ID)){
				teacher ++;
				students ++;
			}
			else{
				points += cost;
			}
		}
		return (points / 2) + ((points / 2) / ((teacher * 2) + (students)));
		
	}

	public List<Research> possibleTeamResearch{
		get{
			List<Research> possible = new List<Research>();
			foreach(Research r in GameController.instance.allResearch){
				if(canTeamDoResearch(r.ID)){
					possible.Add(r);
				}
			}
			return possible;
		}
	}

	//atLeastOne person must not have already done the research
	public bool canTeamDoResearch(int ID){
		bool atLeastOne = false;
		foreach(Employee e in members){
			if(e.employeeResearch.canBeDone(ID)){
				if(!atLeastOne || !e.employeeResearch.hasBeenDone){
					atLeastOne = true;
				}
			}
			else{
				return false;
			}
		}
		if(atLeastOne){
			return true;
		}
		return false;
	}

	public bool canTeamDoSoftware(int ID){
		SoftwareProject test = GameController.instance.allSoftwareProjects[ID];
		foreach(Research r in test.Research){
			bool met = false;
			foreach(Employee e in members){
				if(e.employeeResearch.hasBeenDone(r.ID)){
					met = true;
				}
			}
			if(met == false){
				return false;
			}
		}
		return true;
	}

	public List<SoftwareProject> possibleTeamSoftware{
		get{
			List<SoftwareProject> possible = new List<SoftwareProject>();
			foreach(SoftwareProject r in GameController.instance.allSoftwareProjects.Values){
				if(canTeamDoSoftware(r.ID)){
					possible.Add(r);
				}
			}
			return possible;
		}
	}

	public bool canTeamDoHardware(int ID){
		HardwareProject test = GameController.instance.allHardwareProjects[ID];
		foreach(Research r in test.Research){
			bool met = false;
			foreach(Employee e in members){
				if(e.employeeResearch.hasBeenDone(r.ID)){
					met = true;
				}
			}
			if(met == false){
				return false;
			}
		}
		return GameController.instance.pControl.hasParts(test.Parts);
	}
	
	public List<HardwareProject> possibleTeamHardware{
		get{
			List<HardwareProject> possible = new List<HardwareProject>();
			foreach(HardwareProject r in GameController.instance.allHardwareProjects.Values){
				if(canTeamDoHardware(r.ID)){
					possible.Add(r);
				}
			}
			return possible;
		}
	}

	public int pointsPerSecond(){
		int point = 0;
		foreach(Employee e in members){
			if(goal == goalType.Research){
				point += e.pointsPerTick(researchProject.ResearchField);
			}
			else if(goal == goalType.Software){
				point += e.pointsPerTick(softProject.SoftwareField);
			}
		}
		if(goal == goalType.Research){
			point += leader.pointsPerTick(researchProject.ResearchField);
		}
		else if(goal == goalType.Software){
			point += leader.pointsPerTick(softProject.SoftwareField);
		}
		return point;
	}
}
