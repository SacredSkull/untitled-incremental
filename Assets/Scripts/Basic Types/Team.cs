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

	public void setResearch(int ID){
		researchProject = GameController.instance.allResearch.Find (r => r.ID == researchProject.ID);
	}

	//calculates the cost of doing research for a group of people, with bonuses 
	//for more people doing it, and people who already know it helping. Algorithm 
	//is hidden from user. GUI should be designed so that every time someone is added 
	//to the team we see cost change 
	public int groupResearchCost(){
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

}
