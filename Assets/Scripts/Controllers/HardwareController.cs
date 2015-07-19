using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Incremental.Database;

public class HardwareController : MonoBehaviour {

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

	public List<HardwareProject> AllCompletedHardwareProjects = new List<HardwareProject> ();
	
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

    // EVENTS

    public delegate void StartedHardwareHandler(HardwareProject sender, EventArgs e);
    public delegate void StoppedHardwareHandler(HardwareProject sender, EventArgs e);
    public delegate void CompletedHardwareHandler(HardwareProject sender, EventArgs e);

    public event StartedHardwareHandler onStartedHardware;
    public event StoppedHardwareHandler onStoppedHardware;
    public event CompletedHardwareHandler onCompletedHardware;

	
	private void useRequiredParts(HardwareProject project){
		foreach (Part item in project.Parts) {
			GameController.instance.pControl.use(item, item.quantity);
		}
	}
	
	public void makeHardware(HardwareProject project){
		if (project.canDoMultiple) {
			if(project.HardwareType == HardwareProject.type.Computer){
				GameController.instance.compControl.AllCompletedComputers.Add(project);
			}
			else{
				AllCompletedGenericHardware.Add(project);
				AllCompletedHardwareProjects.Add(project);
			}
            // Check if there are listeners, if so, call event
            if (onCompletedHardware != null)
                onCompletedHardware(project, EventArgs.Empty);
			useRequiredParts(project);
		}
		else{
			if (GameController.instance.allHardwareProjects[project.ID].uses > 0) {
				if(project.HardwareType == HardwareProject.type.Computer){
					GameController.instance.compControl.AllCompletedComputers.Add(project);
				}
				else{
					AllCompletedGenericHardware.Add(project);
				}
				AllCompletedHardwareProjects.Add(project);
                
                // Check if there are listeners, if so, call event
                if (onCompletedHardware != null)
                    onCompletedHardware(project, EventArgs.Empty);

				GameController.instance.allHardwareProjects[project.ID].uses--;
				useRequiredParts(project);
			}
		}
		
	}

}
