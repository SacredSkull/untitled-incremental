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
			foreach (SoftwareProject item in GameController.instance.allSoftwareProjects.Values.ToList()) {
				if(item.uses > 0 || item.uses == -1){
					temp.Add(item.ID,item);
				}
			}
			return temp;
		}
	}

	public List<SoftwareProject> AllCompletedGenericProjects = new List<SoftwareProject> ();
	
	public List<SoftwareProject> AllCompletedSoftware = new List<SoftwareProject>();
	
	public List<SoftwareProject> AllCompletedOS = new List<SoftwareProject> ();
	
	public Dictionary<SoftwareProject.type,SoftwareProject> AllCompletedCourses = new Dictionary<SoftwareProject.type,SoftwareProject> ();

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
	
	public void startSoftware(SoftwareProject project){
		GUITools.setGameObjectActive (GameController.instance.picker, false);
		GUITools.setGameObjectActive (GameController.instance.inProgress, true);
		currentSoftware = project;

        // Check if there are listeners, if so, call event
        if(onStartedSoftware != null)
            onStartedSoftware(currentSoftware, EventArgs.Empty);

		isSoftwareSet = true;
		GameObject.Find ("CurrentResearch").GetComponent<Text>().text = currentSoftware.name;
		GameObject.Find ("Description").GetComponent<Text> ().text = currentSoftware.description;
	}
	
	public void finishSoftware(){
		GameController.instance.justFinished = 8;
		if(currentSoftware.canDoMultiple){
			switch (currentSoftware.SoftwareType) {
			    case SoftwareProject.type.Course:
			        AllCompletedCourses.Add (currentSoftware);
			        break;
			    case SoftwareProject.type.OS:
			        AllCompletedOS.Add(currentSoftware);
			        break;
			    case SoftwareProject.type.Software:
			        AllCompletedSoftware.Add(currentSoftware);
			        break;
			    default:
			        AllCompletedGenericProjects.Add(currentSoftware);
			        break;
			}
		}
		else{
			switch (currentSoftware.SoftwareType) {
			    case SoftwareProject.type.Course:
			        AllCompletedCourses.Add (currentSoftware);
			        break;
			    case SoftwareProject.type.OS:
			        AllCompletedOS.Add(currentSoftware);
			        break;
			    case SoftwareProject.type.Software:
			        AllCompletedSoftware.Add(currentSoftware);
			        break;
			    default:
			        AllCompletedGenericProjects.Add(currentSoftware);
			        break;
			}
			currentSoftware.uses -=1;
			GameController.instance.allSoftwareProjects.Remove(currentSoftware.ID);
			GameController.instance.allSoftwareProjects.Add (currentSoftware.ID,currentSoftware);
		}
        AllCompletedSoftwareProjects.Add (currentSoftware);
        // Check if there are listeners, if so, call event
        if (onCompletedSoftware != null)
            onCompletedSoftware(currentSoftware, EventArgs.Empty);
		currentSoftware = null;
		isSoftwareSet = false;
		PickerController.instance.showPicker ();
	}
}
