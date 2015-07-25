using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EmployeeController : MonoBehaviour {

	//stores all information on the users progress
	public Employee Player = new Employee ();

	//All Employees that have been randomly created this game
	public List<Employee> AllEmployees = new List<Employee> ();

	//All Employees that have been hired
	public List<Employee> AllHiredEmployees = new List<Employee>();

	//All employees that have not been hired by anyone
	public List<Employee> AllAvailableEmployees = new List<Employee>();

	public Dictionary<int,Team> AllActiveTeams = new Dictionary<int,Team> ();

	public List<int> LeaderIDs = new List<Int> ();

	public int totalWages{
		get{
			int cost = 0;
			foreach(Employee e in AllHiredEmployees){
				cost += e.actualWages;
			}
		}
	}

	public void createTeam(List<int> team, bool bossHelp){
		List<Employee> members;
		foreach (int id in team) {
			members.Add(AllEmployees.Find (x => x.ID == id));
		} 
		if(bossHelp){
			members.Add (Player);
		}
		Team temp = new Team (members);
		AllActiveTeams.Add (temp.ID, temp);
		LeaderIDs.Add (temp.ID);
		
	}



	public void awake(){
		Player.me = true;
	}


}
