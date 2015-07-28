using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Incremental.Database;

public class SoftwareController : MonoBehaviour {

	/**
     * @property    public List<SoftwawreProject> UnstartedSoftware
     *
     * @brief   All SoftwareProjects that have not been completed or are repeatable
     * 
     * @warning Iterator; should not be called every tick!
     *
     * @return  The unstarted software.
     */

	public Dictionary<int,SoftwareProject> UnstartedSoftware
	{
		get {
			Dictionary<int,SoftwareProject> temp = new Dictionary<int, SoftwareProject>();
			foreach (SoftwareProject item in GameController.instance.allSoft.Values.ToList()) {
				if(item.uses > 0 || item.uses == -1){
					temp.Add(item.ID,item);
				}
			}
			return temp;
		}
	}

	public Dictionary<int,SoftwareProject> AllIncompleteCourses = new Dictionary<int,SoftwareProject> ();

	public List<SoftwareProject> AllCompletedGenericProjects = new List<SoftwareProject> ();
	
	public List<SoftwareProject> AllCompletedSoftware = new List<SoftwareProject>();
	
	public List<SoftwareProject> AllCompletedOS = new List<SoftwareProject> ();
	
	public Dictionary<Field.field,List<SoftwareProject>> AllCompletedCourses = new Dictionary<Field.field,List<SoftwareProject>> ();

	public List<SoftwareProject> AllCompletedSoftwareProjects = new List<SoftwareProject>();
	
	/**
     * @property    public bool isSoftware
     *
     * @brief   Used by ComputerOnClick. Gets or privately sets a value indicating whether this object is software.
     *
     * @return  true if this object is software, false if not.
     */
	
	public bool isSoftwareSet {
		get;
		private set;
	}
	
	public SoftwareProject currentSoftware {
		get;
		private set;
	}

    // EVENTS
    public delegate void StartedSoftwareHandler(SoftwareProject sender, EventArgs e);
    public delegate void StoppedSoftwareHandler(SoftwareProject sender, EventArgs e);
    public delegate void CompletedSoftwareHandler(SoftwareProject sender, EventArgs e);

    public event StartedSoftwareHandler onStartedSoftware;
    public event StoppedSoftwareHandler onStoppedSoftware;
    public event CompletedSoftwareHandler onCompletedSoftware;
	
	public void startSoftware(SoftwareProject project, bool player){
		if (player) {
			GUITools.setGameObjectActive (GameController.instance.picker, false);
			GUITools.setGameObjectActive (GameController.instance.inProgress, true);
			currentSoftware = project;
			
			// Check if there are listeners, if so, call event
			if (onStartedSoftware != null)
				onStartedSoftware (currentSoftware, EventArgs.Empty);
			
			isSoftwareSet = true;
			GameObject.Find ("CurrentResearch").GetComponent<Text> ().text = currentSoftware.name;
			GameObject.Find ("Description").GetComponent<Text> ().text = currentSoftware.description;
		} else {
			currentSoftware = project;
			isSoftwareSet = true;
		}
	}

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
	
	public void finishSoftware(bool player){
		if (player) {
			GameController.instance.justFinished = 8;
			switch (currentSoftware.SoftwareType) {
			case SoftwareProject.type.Course:
				AllCompletedCourses.Add (currentSoftware);
				break;
			case SoftwareProject.type.OS:
				AllCompletedOS.Add (currentSoftware);
				break;
			case SoftwareProject.type.Software:
				AllCompletedSoftware.Add (currentSoftware);
				break;
			default:
				AllCompletedGenericProjects.Add (currentSoftware);
				break;
			}
			if (!currentSoftware.canDoMultiple) {
				currentSoftware.uses -= 1;
				GameController.instance.allSoft.Remove (currentSoftware.ID);
				GameController.instance.allSoft.Add (currentSoftware.ID, currentSoftware);
			}
			if(currentSoftware.SoftwareType = SoftwareProject.type.Course){
				AllIncompleteCourses.Remove(currentSoftware.ID);
				AllCompletedCourses.Add (currentSoftware.ID,currentSoftware);
			}
			else{
				if (!currentSoftware.canDoMultiple) {
					currentSoftware.uses -= 1;
					GameController.instance.allSoft.Remove (currentSoftware.ID);
					GameController.instance.allSoft.Add (currentSoftware.ID, currentSoftware);
				}
				AllCompletedSoftwareProjects.Add (currentSoftware);
			}
			// Check if there are listeners, if so, call event
			if (onCompletedSoftware != null)
				onCompletedSoftware (currentSoftware, EventArgs.Empty);
			currentSoftware = null;
			isSoftwareSet = false;
			PickerController.instance.showPicker ();
		} else {
			switch (currentSoftware.SoftwareType) {
			case SoftwareProject.type.Course:
				AllCompletedCourses.Add (currentSoftware);
				break;
			case SoftwareProject.type.OS:
				AllCompletedOS.Add (currentSoftware);
				break;
			case SoftwareProject.type.Software:
				AllCompletedSoftware.Add (currentSoftware);
				break;
			default:
				AllCompletedGenericProjects.Add (currentSoftware);
				break;
			}
			if(currentSoftware.SoftwareType = SoftwareProject.type.Course){
				AllIncompleteCourses.Remove(currentSoftware.ID);
				AllCompletedCourses.Add (currentSoftware.ID,currentSoftware);
			}
			else{
				if (!currentSoftware.canDoMultiple) {
					currentSoftware.uses -= 1;
					GameController.instance.allSoft.Remove (currentSoftware.ID);
					GameController.instance.allSoft.Add (currentSoftware.ID, currentSoftware);
				}
				AllCompletedSoftwareProjects.Add (currentSoftware);
			}
			currentSoftware = null;
			isSoftwareSet = false;
		}
		
	}

	void awake(){
		foreach (SoftwareProject s in GameController.instance.courses) {
			AllIncompleteCourses.Add(s.ID,s);
		}

	}
}
