using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Incremental.Database;

public class SoftwareController : MonoBehaviour {

	GameController game;
	PickerController picker;
	ResearchController rControl;

	void Start(){
		game = GameController.instance;
		picker = PickerController.instance;
		rControl = ResearchController.instance;
	}

	private static SoftwareController _instance;
	
	public static SoftwareController instance {
		get {
			if (_instance == null)
				_instance = GameObject.FindObjectOfType<SoftwareController>();
			return _instance;
		}
	}

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
			foreach (SoftwareProject item in game.allSoftwareProjects.Values.ToList()) {
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
	
	public List<SoftwareProject> AllCompletedCourses = new List<SoftwareProject> ();
	
	/**
    * @property    public List<SoftwareProject> PossibleSoftware
    *
    * @brief   All Software that has its dependencies filled
    *          
    * @warning Iterator; should not be called every tick!
    *
    * @return  The potential software.
    */
	
	public List<SoftwareProject> PossibleSoftware{
		get{
			List<Startable> requirements = new List<Startable>();
			List<SoftwareProject> temp = UnstartedSoftware.Values.ToList ().Where(x => x.possible(out requirements)).ToList();
			return temp;
		}
	}
	
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
	
	public void startSoftware(SoftwareProject project){
		GUITools.setGameObjectActive (game.picker, false);
		GUITools.setGameObjectActive (game.inProgress, true);
		currentSoftware = project;
		isSoftwareSet = true;
		GameObject.Find ("CurrentResearch").GetComponent<Text>().text = currentSoftware.name;
		GameObject.Find ("Description").GetComponent<Text> ().text = currentSoftware.description;
	}
	
	public void finishSoftware(){
		game.justFinished = 8;
		if(currentSoftware.canDoMultiple){
			if(currentSoftware.SoftwareType == SoftwareProject.type.Course){
				AllCompletedCourses.Add (currentSoftware);
			}
			else if(currentSoftware.SoftwareType == SoftwareProject.type.OS){
				AllCompletedOS.Add(currentSoftware);
			}
			else if(currentSoftware.SoftwareType == SoftwareProject.type.Software){
				AllCompletedSoftware.Add(currentSoftware);
			}
			else{
				AllCompletedGenericProjects.Add(currentSoftware);
			}
		}
		else{
			if(currentSoftware.SoftwareType == SoftwareProject.type.Course){
				AllCompletedCourses.Add (currentSoftware);
			}
			else if(currentSoftware.SoftwareType == SoftwareProject.type.OS){
				AllCompletedOS.Add(currentSoftware);
			}
			else if(currentSoftware.SoftwareType == SoftwareProject.type.Software){
				AllCompletedSoftware.Add(currentSoftware);
			}
			else{
				AllCompletedGenericProjects.Add(currentSoftware);
			}
			currentSoftware.uses -=1;
			game.allSoftwareProjects.Remove(currentSoftware.ID);
			game.allSoftwareProjects.Add (currentSoftware.ID,currentSoftware);
		}
		currentSoftware = null;
		isSoftwareSet = false;
		picker.showPicker ();
	}
}
