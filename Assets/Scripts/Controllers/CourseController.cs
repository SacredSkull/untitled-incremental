using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CourseController : MonoBehaviour {

	public Dictionary<Field.field,List<SoftwareProject>> AllCompletedCourses = new Dictionary<Field.field,List<SoftwareProject>> ();

	public Dictionary<int,SoftwareProject> AllIncompleteCourses = new Dictionary<int,SoftwareProject> ();

	public SoftwareProject currentCourse;

	public bool isCourseSet = false;

	public bool courseHasBeenDone(int ID){
		if(AllIncompleteCourses.ContainsKey(ID)){
			return false;
		}
		return true;
	}
	
	public bool canDoCourse(int ID, ResearchController r){
		if (courseHasBeenDone (ID)) {
			return false;
		}
		foreach(Research d in GameController.instance.courses[ID].Research){
			if(!r.hasBeenDone(d)){
				return false;
			}
		}
		return true;
	}

	void awake(){
		foreach (SoftwareProject s in GameController.instance.courses) {
			AllIncompleteCourses.Add(s.ID,s);
		}
		
	}

	public void startCourse(SoftwareProject project, bool player){
		if (player) {
			GUITools.setGameObjectActive (GameController.instance.picker, false);
			GUITools.setGameObjectActive (GameController.instance.inProgress, true);
			currentCourse = project;
			
				
			isCourseSet = true;
			GameObject.Find ("CurrentResearch").GetComponent<Text> ().text = currentCourse.name;
			GameObject.Find ("Description").GetComponent<Text> ().text = currentCourse.description;
		} else {
			currentCourse = project;
			isCourseSet = true;
		}
	}
	
	
	
	public void finishCourse(bool player){
		if(player){
			PickerController.instance.showPicker ();
			GameController.instance.justFinished = 8;
		}
		AllIncompleteCourses.Remove (currentCourse.ID);
		AllCompletedCourses.Add (currentCourse.ID, currentCourse);
		currentCourse = null;
		isCourseSet = false;
		
		
	}
}
