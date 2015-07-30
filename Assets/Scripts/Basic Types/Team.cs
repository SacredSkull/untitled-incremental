using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Team : MonoBehaviour {
	public enum goalType
	{
		None,
		Research,
		Software, 
		Course
	}

	public goalType goal;
	public Research researchProject;
	public SoftwareProject softProject;
	public SoftwareProject courseProject;
	public int pointCost;
	public int currentPoints = 0;

	public List<Employee> members = new List<Employee> ();

	public Employee leader;

	public Team(List<Employee> group){
		leader = group [0];
		foreach (Employee e in group) {
			members.Add (e);
		}
	}

	public int ID{
		get{
			return leader.ID;
		}
	}

	public bool containsPlayer(){
		foreach(Employee e in members){
			if(e.me){
				return true;
			}
		}
		return false;
	}

	public void startResearch(int ID){
		researchProject = GameController.instance.allResearch.Find (x => x.ID = ID);
		foreach(Employee e in members){
			e.employeeResearch.startResearch(researchProject,e.me);
		}
		goal = goalType.Research;
		pointCost = groupResearchCost ();
	}

	public void finishResearch(){
		foreach(Employee e in members){
			e.employeeResearch.finishResearch(e.me);
		}
		researchProject = null;
		goal = goalType.None;
	}

	public void startSoftware(int ID){
		softProject = GameController.instance.userSControl.allSoft [ID];
		GameController.instance.userSControl.startSoftware (softProject, containsPlayer());
		goal = goalType.Software;
		pointCost = groupSoftwareCost ();
	}

	public void finishSoftware(){
		GameController.instance.userSControl.finishSoftware (containsPlayer ());
		softProject = null;
		goal = goalType.None;
	}

	public void startCourse(int ID){
		courseProject = GameController.instance.courses [ID];
		foreach(Employee e in members){
			e.employeeCourses.startCourse(courseProject, e.me);
		}
		goal = goalType.Course;
		pointCost = groupCourseCost ();
	}
	
	public void finishCourse(){
		foreach(Employee e in members){
			e.employeeCourses.finishCourse(e.me);
		}
		softProject = null;
		goal = goalType.None;
	}

	public bool canAddPoints(int points){
		if((currentPoints + points) >= pointCost){
			return false;
		}
		return true;
	}

	// will only be called after first checking canAddPoints
	public void addPoints(int points){
		currentPoints += points;

	}

	//calculates the cost of doing research for a group of people, with bonuses 
	//for more people doing it, and people who already know it helping. Algorithm 
	//is hidden from user. GUI should be designed so that every time someone is added 
	//to the team we see cost change 
	int groupResearchCost(){
		int points = 0;
		int teacher = 0;
		int students = 0;
		int cost = researchProject.cost;
		foreach (Employee e in members) {
			if(!e.employeeResearch.hasBeenDone(researchProject.ID)){
				points += cost;
				students ++;
			}
			else{
				teacher ++;
			}
		}
		return (points / 2) + ((points / 2) / ((teacher * 2) + (students)));
		
	}

	int groupCourseCost(){
		int points = 0;
		int teacher = 0;
		int students = 0;
		int cost = courseProject.pointCost;
		foreach (Employee e in members) {
			if(e.employeeCourses.courseHasBeenDone(softProject.ID)){
				teacher ++;

			}
			else{
				students ++;
				points += cost;
			}
		}
		return (points / 2) + ((points / 2) / ((teacher * 2) + (students)));
	}

	int groupSoftwareCost(){
		int knowledge = 0;
		int totalResearch = 0;
		int cost = softProject.pointCost;
		foreach (Employee e in members) {
			foreach(Research d in softProject.Research){
				if(e.employeeResearch.hasBeenDone(d)){
					knowledge++;
				}
				totalResearch++;
			}
		}
		return (cost / 2) + ((cost / totalResearch)*knowledge) ;
		
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
		SoftwareProject test = GameController.instance.allSoft[ID];
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

	public bool canTeamDoCourse(int ID){
		bool atLeastOne = false;
		foreach(Employee e in members){
			if(e.employeeCourses.canDoCourse(ID,e.employeeResearch)){
				atLeastOne = true;
			}
			else if(e.employeeCourses.courseHasBeenDone(ID)){
				return false;
			}
		}
		return atLeastOne;
	}

	public List<SoftwareProject> possibleTeamSoftware{
		get{
			List<SoftwareProject> possible = new List<SoftwareProject>();
			foreach(SoftwareProject r in GameController.instance.allSoft.Values){
				if(canTeamDoSoftware(r.ID)){
					possible.Add(r);
				}
			}
			return possible;
		}
	}

	public List<SoftwareProject> possibleTeamCourses{
		get{
			List<SoftwareProject> possible = new List<SoftwareProject>();
			foreach(SoftwareProject r in GameController.instance.courses.Values){
				if(canTeamDoCourse(r.ID)){
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
		return point;
	}
}
