using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Incremental.Database;

public class HardwareController : MonoBehaviour {

	private static HardwareController _instance;
	
	public static HardwareController instance {
		get {
			if (_instance == null)
				_instance = GameObject.FindObjectOfType<HardwareController>();
			return _instance;
		}
	}

	public Dictionary<int,HardwareProject> UnstartedHardware {
		get {
			Dictionary<int,HardwareProject> temp = new Dictionary<int,HardwareProject>();
			foreach (var item in GameController.instance.allHardwareProjects.Values.ToList())
			{
				if (item.uses > 0 || item.uses == -1)
				{
					temp.Add(item.ID,item);
				}
			}
			return temp;
		}
	}

	
	public HardwareProject primaryComputer {
		get;
		private set;
	}
	
	public List<HardwareProject> AllCompletedGenericHardware = new List<HardwareProject> ();
	
	public List<HardwareProject> PossibleHardware{
		get{
			// Note: out parameters are not optional by design, so we actually need to create this.
			// The alternative is overloading possible(), with a method that does not compute missing requirements.
			List<Startable> requirements = new List<Startable>();
			List<HardwareProject> temp = UnstartedHardware.Values.ToList ().Where(x => x.possible()).ToList();
			return temp;
		}
	}
	
	private void useRequiredParts(HardwareProject project){
		foreach (Part item in project.Parts) {
			PartController.instance.use(item, item.quantity);
		}
	}
	
	public void makeHardware(HardwareProject project){
		if (project.canDoMultiple) {
			if(project.HardwareType == HardwareProject.type.Computer){
				ComputerController.instance.AllCompletedComputers.Add(project);
			}
			else{
				AllCompletedGenericHardware.Add(project);
			}
			useRequiredParts(project);
		}
		else{
			if (GameController.instance.allHardwareProjects[project.ID].uses > 0) {
				if(project.HardwareType == HardwareProject.type.Computer){
					ComputerController.instance.AllCompletedComputers.Add(project);
				}
				else{
					AllCompletedGenericHardware.Add(project);
				}
				GameController.instance.allHardwareProjects[project.ID].uses--;
				useRequiredParts(project);
			}
		}
		
	}

}
