using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EmployeeController : MonoBehaviour {

	//All Employees that have been randomly created this game
	public List<Employee> AllEmployees = new List<Employee> ();

	//All Employees that have been hired
	public List<Employee> AllHiredEmployees = new List<Employee>();

	//All employees that have not been hired by anyone
	public List<Employee> AllAvailableEmployees = new List<Employee>();

	public int totalWages{
		get{
			int cost = 0;
			foreach(Employee e in AllHiredEmployees){
				cost += e.actualWages;
			}
		}
	}

	//calculates the cost of doing research for a group of people, with bonuses 
	//for more people doing it, and people who already know it helping. Algorithm 
	//is hidden from user. GUI should be designed so that every time someone is added 
	//to the team we see cost change 
	public int groupCost(List<Employee> team, bool bossHelp, int ID){
		int points = 0;
		int teacher = 0;
		int students = 0;
		int cost = GameController.instance.rControl.hasBeenDone(ID);
		foreach (Employee e in team) {
			if(e.employeeResearch.hasBeenDone(ID)){
				teacher ++;
				students ++;
			}
			else{
				points += cost;
			}
		}
		if(bossHelp){
			if(GameController.instance.rControl.hasBeenDone(ID)){
				points += cost;
				students ++;
			}
			else{
				teacher ++;
			} 
		}
		return (points / 2) + ((points / 2) / ((teacher * 2) + (students)));

	}
}
