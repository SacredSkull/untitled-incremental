using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Incremental.Database;

public class ResearchController : MonoBehaviour {

	/** @brief   If research is set or not */
	public bool researchSet = false;
	/** @brief   All uncomplete research. */
	public Dictionary<int, Research> AllUncompleteResearch = new Dictionary<int, Research>();
	/** @brief   All complete research- key = ResearchID, value = Research*/
	public Dictionary<int, Research> AllCompleteResearch = new Dictionary<int, Research>();
	//May need to create another list ordered by ID to make finding completed
	//research more efficient. For now though, it is not necessary.
	//private List<Research> AllResearch = new List<Research>();
	
	public int unlockedCount {
		get;
		set;
	}
	
	public bool isResearchSet(){
		return researchSet;
	}
	
	public Research currentResearch {
		get;
		private set;
	}
	
	public Research lastCompleted {
		get;
		set;
	}
	
	public int currentID {
		get;
		private set;
	}

    // EVENTS
    public delegate void StartedResearchHandler(Research sender, EventArgs e);
    public delegate void StoppedResearchHandler(Research sender, EventArgs e);
    public delegate void CompletedResearchHandler(Research sender, EventArgs e);

    public event StartedResearchHandler onStartedResearch;
    public event StoppedResearchHandler onStoppedResearch;
    public event CompletedResearchHandler onCompletedResearch;

	public int pointCostByID(int ID){
		if (AllUncompleteResearch.ContainsKey (ID)) {
			return AllUncompleteResearch [ID].cost;
		} else {
			return AllCompleteResearch [ID].cost;
		}
	}

	/**
    * @fn  public bool hasBeenDone()
    *
    * @brief   Query if research (this object) has been done.
    *
    * @author  Conal
    * @author  Peter
    * @date    01/04/2015
    * @updated 27/06/2015
    *
    * @return  true if been done, false if not.
    */
	
	public bool hasBeenDone(Research query) {
		return AllCompleteResearch.ContainsKey(query.ID);
	}
	
	/**
     * @fn  public void startResearch(Research research)
     *
     * @brief   Starts research.
     *
     * @author  Conal
     * @date    05/04/2015
     *
     * @param   research    The research to start.
     */
	
	public void startResearch(int ID, bool player){
		if (player) {
			GameController.instance.justFinished = 0;
			researchSet = true;
			if(!AllCompleteResearch.ContainsKey(ID)){
				currentResearch = AllUncompleteResearch [ID];
			}
			// Check that there are listeners, if so call event
			if (onStartedResearch != null)
				onStartedResearch (currentResearch, EventArgs.Empty);
			GUITools.setGameObjectActive (GameController.instance.picker, false);
			GUITools.setGameObjectActive (GameController.instance.inProgress, true);
			GameObject.Find ("CurrentResearch").GetComponent<Text> ().text = currentResearch.name;
			GameObject.Find ("Description").GetComponent<Text> ().text = currentResearch.description;
		} else {
			researchSet = true;
			if(AllCompleteResearch.ContainsKey(ID)){
				currentResearch = AllUncompleteResearch [ID];
			}
		}
	}
	
	//removes from UncompleteResearch, adds to completedResearch
	
	/**
     * @fn  public void finishResearch()
     *
     * @brief   Finishes research, removing it from uncompleted research and adding it into the completed list.
     *
     * @author  Conal
     * @date    01/04/2015
     */
	
	public void finishResearch(bool player){
		if (player) {
			GameController.instance.justFinished = 8;
			GUITools.setGameObjectActive (GameController.instance.inProgress, false);
			researchSet = false;
			if(!currentResearch == null){
				int index = currentResearch.ID;
				AllUncompleteResearch.Remove (index);
				currentResearch.complete ();
				AllCompleteResearch.Add (currentResearch.ID, currentResearch);
				lastCompleted = currentResearch;
				currentResearch = null;
				AllPossibleResearchByKey = SortResearchByKey (AllPossibleResearch);
			}

			
			// If there are listeners, call the event
			if (onCompletedResearch != null)
				onCompletedResearch (currentResearch, EventArgs.Empty);
			

			PickerController.instance.showPicker ();
		} else {
			researchSet = false;
			if(!currentResearch == null){
				int index = currentResearch.ID;
				AllUncompleteResearch.Remove (index);
				currentResearch.complete ();
				AllCompleteResearch.Add (currentResearch.ID, currentResearch);
				lastCompleted = currentResearch;
				currentResearch = null;
				AllPossibleResearchByKey = SortResearchByKey (AllPossibleResearch);
			}

		}
	}
	
	//currently useless
	/*public void addNewResearch(Research a){
		if (a.isDone ()) {
			AllCompleteResearch.Add (a);
			AllCompleteResearch.Sort ();
		} else {
			AllUncompleteResearch.Add (a);
			AllUncompleteResearch.Sort ();
		}
	}*/
	
	//Finds the Research that are in after but not in before
	private List<Research> findNew(List<Research> before,List<Research> after){
		List<Research> newResearch = new List<Research> ();
		foreach(Research a in after){
			bool found = false;
			foreach(Research b in before){
				if(a.ID == b.ID){
					found = true;
					break;
				}
			}
			if(found == false){
				newResearch.Add(a);
			}
		}
		return newResearch;
	}

	/**
    * @fn  public bool canBeDone(ref List<Research> missingResearch)
    *
    * @brief   Recursive variant of canBeDone() which goes up the entire chain of dependencies, 
    * filling a list that must be met before this can be done. 
    *
    * One particular use in mind for this function (and those like it for Projects, etc.) is that of the hierarchy list, which would high
    * -light the required nodes needed to start this research. 
    * 
    * @author  Peter
    * @date    28/06/2015
    *
    * @param   missingResearch Reference to the List<Research> of requirements not met. As a ref, 
    * this must be instantiated BEFORE passing it into the method.  
    * 
    * @warning This method iterates into each unfilled dependency, and walks out its unfilled dependencies and so on, which obviously
    * results in a longer processing time than the standard canBeDone().
    * 
    * @return  true if there are no dependencies to fullfil, false if requirements are not met. The missingResearch list (empty or not) is always returned.
    */
	
	public bool canBeDone(int ID) {
		Research test = GameController.instance.allResearch.Find (x => x.ID == ID);
		foreach (Research depends in test.Dependencies) {
			if (!hasBeenDone(depends)) {
				return false;
			}
		}
		return true;
	}
	
	/**
     * @property    public List<Research> AllPossibleResearch
     *
     * @brief   Gets all possible research that canBeDone().
     * 
     * @warning Iterator; should not be called every tick!
     * 
     * @return  all possible research.
     */
	
	//The higher the int value the more recently the Research has been possible. In order for this to work
	//an old version of the dictionary must be given.
	public Dictionary<int,Research> PossibleResearch(Dictionary<int,Research> before){
		int i;
		Dictionary<int,Research> finalCanDo = new Dictionary<int,Research > ();
		List<Research> canDo = new List<Research> ();
		foreach (Research r in AllUncompleteResearch.Values.ToList()) {
			if (r.canBeDone ()) {
				canDo.Add (r);
			}
		}
		if (before == null) {
			i = 0;
			foreach (Research r in canDo) {
				finalCanDo.Add (i, r);
				i++;
			}
			storePossibleResearch = finalCanDo;
			return finalCanDo;
		} else {
			int high = -1;
			//finds highest number
			foreach (int k in before.Keys.ToList()) {
				if (k > high) {
					high = k;
				}
			}
			List<Research> additions = findNew (before.Values.ToList (), canDo);
			finalCanDo = before;
			//removes the key of the last completed research. Thanks Stack Overflow! :)
			try{
				var item = finalCanDo.First(x => x.Value.ID == lastCompleted.ID);
				if (!item.Equals(null)){
					finalCanDo.Remove(item.Key);
				}
			}
			catch(NullReferenceException){
				
			}
			catch(InvalidOperationException){
				
			}
			
			
			foreach (Research r in additions) {
				high++;
				finalCanDo.Add (high, r);
			}
			storePossibleResearch = finalCanDo;
			return finalCanDo;
		}
	}
	
	public Dictionary<int,Research> storePossibleResearch{
		get;
		private set;
	}

	//Method to be called whenever the list is needed
	public Dictionary<int,Research> AllPossibleResearch{
		get{
			return PossibleResearch(storePossibleResearch);
		}
	}

	//where the sorted list is to be stored, to prevent all these bothersome
	//methods from being repeatedly called.
	public List<Research> AllPossibleResearchByKey {
		get;
		set;
	}
	
	//from highest to lowest
	public List<Research> SortResearchByKey(Dictionary<int,Research> unsorted){
		List<int> keyOrder = new List<int> ();
		List<int> unorderedKeys = unsorted.Keys.ToList ();
		int count = unorderedKeys.Count;
		for (int i = 0; i<count; i++) {
			keyOrder.Add(unorderedKeys.Max ());
			unorderedKeys.Remove (unorderedKeys.Max ());
		}
		List<Research> ordered = new List<Research> ();
		foreach (int j in keyOrder) {
			ordered.Add(unsorted[j]);
		}
		return ordered;
	}

	void Awake(){
		foreach (Research r in GameController.instance.allResearch) {
			AllUncompleteResearch.Add (r.ID,r);
		}
	}

}
