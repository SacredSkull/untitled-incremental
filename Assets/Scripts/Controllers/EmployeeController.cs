using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EmployeeController : MonoBehaviour {

	public OfficeController offControl = new OfficeController();

	//stores all information on the users progress
	public Employee Player = new Employee ();

	//All Employees that have been randomly created this game
	public List<Employee> AllEmployees = new List<Employee> ();

	//All Employees that have been hired and can be given work
	public List<Employee> AllHiredEmployees = new List<Employee>();

	//All employees that have not been hired by anyone
	public List<Employee> AllAvailableEmployees = new List<Employee>();

	// teams that are working
	public Dictionary<int,Team> AllActiveTeams = new Dictionary<int,Team> ();

	public List<int> LeaderIDs = new List<int> ();

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
			AllHiredEmployees.Remove (AllEmployees.Find (x=>x.ID == id));
		} 
		if(bossHelp){
			members.Add (Player);
			Player.working = true;
		}
		Team temp = new Team (members);
		AllActiveTeams.Add (temp.ID, temp);
		LeaderIDs.Add (temp.ID);
	}

	public void teamStartResearch(int teamID, int researchID){
		AllActiveTeams [teamID].startResearch (researchID);
	}

	public void teamFinishResearch(int teamID){
		AllActiveTeams [teamID].finishResearch();
		disbandTeam (AllActiveTeams [teamID]);
	}

	public void teamStartSoftware(int teamID, int softID){
		AllActiveTeams [teamID].startSoftware (softID);
	}
	
	public void teamFinishSoftware(int teamID){
		AllActiveTeams [teamID].finishSoftware();
		disbandTeam (AllActiveTeams [teamID]);
	}

	public void teamStartCourse(int teamID, int courseID){
		AllActiveTeams [teamID].startCourse(courseID);
	}
	
	public void teamFinishCourse(int teamID){
		AllActiveTeams [teamID].finishCourse();
		disbandTeam (AllActiveTeams [teamID]);
	}

	void disbandTeam(Team done){
		foreach(Employee e in done.members){
			if(e.me){
				Player.working = false;
			}
			else{
				AllHiredEmployees.Add(e);
			}
		}
		AllActiveTeams.Remove (done.ID);
	}

	public void addPoints(bool click){
		if(click){
			foreach(Team t in AllActiveTeams.Values){
				if(t.containsPlayer){
					if(t.canAddPoints){
						t.addPoints(Player.pointsPerClick);
					}
				}
			}
		}
		foreach(Team t in AllActiveTeams.Values){
			int points = t.pointsPerSecond;
			if(t.canAddPoints(points)){
				t.addPoints(points);
			}
			else{
				if(t.goal = Team.goalType.Research){
					teamFinishResearch(t.ID);
				}
				else if(t.goal = Team.goalType.Course){
					teamFinishCourse(t.ID);
				}
				else{
					teamFinishSoftware(t.ID);
				}
			}
		}
	}


	public void awake(){
		Player.me = true;
	}


}
