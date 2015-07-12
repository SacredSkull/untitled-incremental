#define DEBUG
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Incremental.Database;

using UnityEngine.EventSystems;

#if DEBUG
//using System.Diagnostics;
#endif

/**
 * @class   GameController
 *
 * @brief   This is the main class of the game, and thus handles all of the top level logic.
 *          
 * @details This class is a singleton (See http://unitypatterns.com/singletons/) and can only have one instance.
 *
 * @author  Peter
 * @author  Conal
 * @date    20/03/2015
 * @updated 26/06/2015
 */

public class GameController : MonoBehaviour {

	private int BUTTON_COUNT = 5;
	GameObject picker;
	GameObject inProgress;
	public GameObject info;
    List<HardwareProject> allHardwareProjects;
    List<SoftwareProject> allSoftwareProjects;
    List<Research> allResearch;
	public int chapterPage = 0;
	//Have you changed page in the last 15 frames?
	public bool flicked = false;
	public enum pickedType
	{
		None,
		Research,
		Software,
		Hardware,
		Parts

	}
	public pickedType chapter;

    // See http://unitypatterns.com/singletons/ for more details. Alternatively, google C# singleton.
    private static GameController _instance;

    /**
     * @property    public static GameController instance
     *
     * @brief   Gets the singleton instance of the GameController (this) class.
     * @return  The instance.
     */

    public static GameController instance {
        get {
            if (_instance == null)
                _instance = GameObject.FindObjectOfType<GameController>();
            return _instance;
        }
    }

	//-----------DEBUG
	public bool debugResearchPoints;

	//-----------Incremental Values
    private const int BASE_POINTS_PER_CLICK = 1;
	private const double BASE_PROCESSING_POWER = 1.00;
    /** @brief   Current money of the player. */
	public double money = 0.00;

    //----------Outputs
	private Text score;
	private Text moneyScore;
	private Text powerLevel;
	private Text ppc;
	private Text ppt;
	private Text mpt;
	private Text potentialResearch;
	private Text r1;
	private Text r2;
	private Text r3;
	private Text r4;
	private Text r5;
	public List<Text> outProject = new List<Text>();


	//-----------------------------------------------------CORE METHODS

	//adds points if you should be able to add points:
	//1.Research has been set
	//2. Research hasn't been finished.

    /**
     * @fn  public void addResearchPoints(int points)
     *
     * @brief   Adds research points.
     *          
     * @details Adds research points if the research you are currently researching costs 
     * more than you currently have. If it costs less and isn't software, your current research 
     * is halted and your research points are set to zero. Finally, if any other 
     * condition (should only be software research) , your software is finished and points are set to zero.
     *
     * @author  Conal
     * @date    05/04/2015
     *
     * @param   points  The points to add.
     */

	public void addResearchPoints(int points){
		if(isResearchSet()){
			if ((currentResearch.cost > researchPoints+points) || debugResearchPoints) {
				this.researchPoints += points;
			} else if (!isSoftware) {
				finishResearch ();
				this.researchPoints = 0;
			} else {
				finishSoftware();
				this.researchPoints = 0;
			}
		}


	}

    /**
     * @property    public int pointsPerTick
     *
     * @brief   Adds up and returns the points per second of all completed software/hardware research.     
     * 
     * @todo    Calculate sum of all multipliers, points etc in projects,      
     * 
     * @todo    Add this to the update method, then we'll have a game!
     *          
     * 
     * @return  The points per second.
     */

    int pointsPerSecond{
		get{
			int temp = 0;
			List<SoftwareProject> relevantSoftware = AllCompletedSoftware.Where(x => x.pointsPerTick> 0).ToList();
			List<HardwareProject> relevantHardware = AllCompletedHardware.Where(x => x.pointsPerTick> 0).ToList();
			foreach(SoftwareProject project in relevantSoftware){
				temp += project.pointsPerTick;
			}
			foreach(HardwareProject project in relevantHardware){
				temp += project.pointsPerTick;
			}
			return temp;
		}
	}

    /**
     * @property    public int pointsPerClick
     *
     * @brief   Gets the points per click.
     *
     * @return  The points per click.
     */

	 public int pointsPerClick{
		get{
			int temp = BASE_POINTS_PER_CLICK;
			List<SoftwareProject> relevantSoftware = AllCompletedSoftware.Where(x => x.pointsPerClick> 0).ToList();
			List<HardwareProject> relevantHardware = AllCompletedHardware.Where(x => x.pointsPerClick> 0).ToList();
			foreach(SoftwareProject item in relevantSoftware){
				temp+=  item.pointsPerClick;
			}
			foreach(HardwareProject item in relevantHardware){
				temp+=  item.pointsPerClick;
			}
			return temp;
		}
	}

    /**
     * @property    public int pointsMult
     *
     * @brief   Gets the points multiplier.
     *
     * @return  The points multiplier.
     */

    public int pointsMult{
		get{
			int temp = 1;
			List<SoftwareProject> relevantSoftware = AllCompletedSoftware.Where(x => x.pointMult> 0).ToList();
            List<HardwareProject> relevantHardware = AllCompletedHardware.Where(x => x.pointMult > 0).ToList();
			foreach(SoftwareProject item in relevantSoftware){
				temp += item.pointMult;
			}
			foreach(HardwareProject item in relevantHardware){
				temp += item.pointMult;
			}
			return temp;
		}
	}

    /**
     * @property    public double processingPower
     *
     * @brief   Gets the processing power.
     *
     * @return  The processing power.
     */

    public double processingPower{
		get{
			double temp = BASE_PROCESSING_POWER;
			List<SoftwareProject> relevantSoftware = AllCompletedSoftware.Where(x => x.processIncrease> 0.00).ToList();
			List<HardwareProject> relevantHardware = AllCompletedHardware.Where(x => x.processIncrease> 0.00).ToList();
			foreach(SoftwareProject item in relevantSoftware){
				temp += item.processIncrease;
			}
			foreach(HardwareProject item in relevantHardware){
				temp += item.processIncrease;
			}
			return temp;
		}
	}

    /**
     * @property    public double moneyPerTick
     *
     * @brief   Gets money per second.
     *
     * @return  The money per second.
     */

	public double moneyPerSecond{
		get{
			int temp = 0;
			List<SoftwareProject> relevantSoftware = AllCompletedSoftware.Where(x => x.moneyPerTick> 0).ToList();
            List<HardwareProject> relevantHardware = AllCompletedHardware.Where(x => x.moneyPerTick > 0).ToList();
			foreach(SoftwareProject item in relevantSoftware){
				temp+=  item.moneyPerTick;
			}
			foreach(HardwareProject item in relevantHardware){
				temp+=  item.moneyPerTick;
			}
			Debug.Log (temp.ToString());
			return temp;
		}
	}

    /**
     * @property    public int moneyMultiplier
     *
     * @brief   Gets the money multiplier.
     *
     * @return  The money multiplier.
     */

	public int moneyMultiplier{
		get{
			int temp = 1;
			List<SoftwareProject> relevantSoftware = AllCompletedSoftware.Where(x => x.moneyMult> 0).ToList();
            List<HardwareProject> relevantHardware = AllCompletedHardware.Where(x => x.moneyMult > 0).ToList();
			foreach(SoftwareProject item in relevantSoftware){
				temp+=  item.moneyMult;
			}
			foreach(HardwareProject item in relevantHardware){
				temp+=  item.moneyMult;
			}
			return temp;
		}
	}

	public void addMoneyPerSecond(){
		money += moneyPerSecond * moneyMultiplier;
	}

	public void addPointsPerSecond(){
		int toAdd = pointsPerSecond * pointsMult;
		if (researchSet) {
			addResearchPoints(toAdd);
		}
	}


	//-----------------------------------------------------RESEARCH METHODS AND DATA
	/** @brief   If research is set or not */
	private bool researchSet = false;
	/** @brief   All uncomplete research. */
    public Dictionary<int, Research> AllUncompleteResearch = new Dictionary<int, Research>();
	/** @brief   All complete research- key = ResearchID, value = Research*/
    public Dictionary<int, Research> AllCompleteResearch = new Dictionary<int, Research>();
	//May need to create another list ordered by ID to make finding completed
	//research more efficient. For now though, it is not necessary.
	//private List<Research> AllResearch = new List<Research>();
	
    // EVENTS
    public delegate void StartedResearchHandler(Research sender, EventArgs e);
    public delegate void StoppedResearchHandler(Research sender, EventArgs e);
    public delegate void CompletedResearchHandler(Research sender, EventArgs e);

    public event StartedResearchHandler StartedResearch;
    public event StoppedResearchHandler StoppedResearch;
    public event CompletedResearchHandler aCompletedResearch;

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

    public int researchPoints {
		get;
		private set;
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

	public void startResearch(Research research){
		researchSet = true;
		currentResearch = research;
		picker.active = false;
		inProgress.active = true;
		//451,151,0,455,300
		GameObject.Find ("CurrentResearch").GetComponent<Text>().text = currentResearch.name;
		GameObject.Find ("Description").GetComponent<Text> ().text = currentResearch.description;
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

	public void finishResearch(){
		researchSet = false;
		inProgress.active = false;
		picker.active = true;
		setButtonVisible ("Index", true);
	    int index = currentResearch.ID;
		AllUncompleteResearch.Remove (index);
		currentResearch.complete();
		AllCompleteResearch.Add(currentResearch.ID, currentResearch);
		lastCompleted = currentResearch;
		currentResearch = null;
		AllPossibleResearchByKey = SortResearchByKey (AllPossibleResearch);
		List<Research> temp = AllPossibleResearchByKey;
		int i = 0;
		chapterPage = 0;
		foreach(Text field in outProject){
			int position = 0+(chapterPage*BUTTON_COUNT)+i;
			try{
				field.text = temp[position].name + ": " +temp[position].cost;
				field.GetComponent<WorkID>().ID = temp[position].ID;
			}
			catch(ArgumentOutOfRangeException){
				field.text = "???";
			}
			i++;
		}
		if(chapterPage == 0){
			GameObject button; 
			button = GameObject.Find("Previous");
			button.GetComponent<CanvasGroup>().alpha = 0;
			button.GetComponent<Button>().interactable = false;
			button.GetComponent<CanvasGroup>().interactable = false;
			if(temp.Count <= BUTTON_COUNT+(chapterPage*BUTTON_COUNT)){
				button = GameObject.Find("Next");
				button.GetComponent<CanvasGroup>().alpha = 0;
				button.GetComponent<Button>().interactable = false;
				button.GetComponent<CanvasGroup>().interactable = false;
			}
			else{
				button = GameObject.Find("Next");
				button.GetComponent<CanvasGroup>().alpha = 1;
				button.GetComponent<Button>().interactable = true;
				button.GetComponent<CanvasGroup>().interactable = true;
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
			var item = finalCanDo.First(x => x.Value.ID == lastCompleted.ID);
			finalCanDo.Remove(item.Key);
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
			Debug.Log (unorderedKeys.Count.ToString());
			unorderedKeys.Remove (unorderedKeys.Max ());
		}
		List<Research> ordered = new List<Research> ();
		foreach (int j in keyOrder) {
			ordered.Add(unsorted[j]);
		}
		return ordered;
	}

	//-----------------------------------------------------SOFTWARE

    // EVENTS
    public delegate void StartedSoftwareHandler(SoftwareProject sender, EventArgs e);
    public delegate void StoppedSoftwareHandler(SoftwareProject sender, EventArgs e);
    public delegate void CompletedSoftwareHandler(SoftwareProject sender, EventArgs e);

    public event StartedSoftwareHandler StartedSoftware;
    public event StoppedSoftwareHandler StoppedSoftware;
    public event CompletedSoftwareHandler CompletedSoftware;

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
            foreach (SoftwareProject item in allSoftwareProjects) {
                if(item.uses > 0 || item.uses == -1){
					temp.Add(item.ID,item);
				}
			}
            return temp;
        }
    }
    public List<SoftwareProject> AllCompletedSoftware = new List<SoftwareProject>();

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

	public bool isSoftware {
		get;
		private set;
	}

	public SoftwareProject currentSoftware {
		get;
		private set;
	}

	public void startSoftware(SoftwareProject project){
		currentSoftware = project;
		isSoftware = true;
	}

	public void finishSoftware(){
		if(currentSoftware.canDoMultiple){
			AllCompletedSoftware.Add(currentSoftware);
		}
		else{
			AllCompletedSoftware.Add(currentSoftware);
            allSoftwareProjects[currentResearch.ID].uses--;
		}
		currentSoftware = null;
		isSoftware = false;
		currentResearch = null;
		researchSet = false;
	}

	//-----------------------------------------------------HARDWARE

    // HardwareProject
    public delegate void StartedHardwareHandler(HardwareProject sender, EventArgs e);
    public delegate void StoppedHardwareHandler(HardwareProject sender, EventArgs e);
    public delegate void CompletedHardwareHandler(HardwareProject sender, EventArgs e);

    public event StartedHardwareHandler StartedHardware;
    public event StoppedHardwareHandler StoppedHardware;
    public event CompletedHardwareHandler CompletedHardware;

    public Dictionary<int,HardwareProject> UnstartedHardware {
        get {
            Dictionary<int,HardwareProject> temp = new Dictionary<int,HardwareProject>();
            foreach (var item in allHardwareProjects)
            {
                if (item.uses > 0 || item.uses == -1)
                {
                    temp.Add(item.ID,item);
                }
            }
            return temp;
        }
    }
    public List<HardwareProject> AllCompletedHardware = new List<HardwareProject>();

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
			use(item, item.quantity);
		}
	}

	public void makeHardware(HardwareProject project){
        if (project.canDoMultiple) {
            AllCompletedHardware.Add(project);
            useRequiredParts(project);
		}
		else{
            if (allHardwareProjects[project.ID].uses > 0) {
                AllCompletedHardware.Add(project);
                allHardwareProjects[project.ID].uses--;
                useRequiredParts(project);
            }
        }

	}


	//-----------------------------------------------------TICK METHODS AND DATA
	// A tick is the baseline time measure but it really depends on the speed of the CPU
	// and any lag that occurs between each frame. Delta time is basically how long the last frame
	// took to render; it accurately tells us how long that tick should be.

	// Note: This is the point ticker. See the ticker for updating GUIs/checking etc.
	private int ticker = 0;
	private float _incrementalTickTime;
	public float incrementalTickTime {
		get {
			return _incrementalTickTime * Time.deltaTime;
		}
		set{
			_incrementalTickTime = value;
		}
	}

	// This is that time upgrade you can buy in incrementals. By reducing the amount of iterations, you
	// reduce the amount of time a tick takes, thus increasing your effectivity.
	public int incrementalTickIterations;


	//-----------------------------------------------------PARTS METHODS AND DATA

	//List containing all parts
	public List<Part> allParts = new List<Part>();

	public Dictionary<int,Part> allBuyableParts{
		get {
			Dictionary<int,Part> temp = new Dictionary<int,Part>();
			foreach (var item in allParts)
			{
				temp.Add(item.ID,item);

			}
			return temp;
		}
	}
	
	// Dictionary containing the part ID and quantity
	public readonly Dictionary<int, int> partInventory = new Dictionary<int, int>();

	bool hasParts(Part p, int number) {
	    return partInventory[p.ID] >= number;
	}

	public void buyPart(Part p, int number){
		if (money >= (p.cost * number)) {
			money-= (p.cost * number);
			if(partInventory.ContainsKey(p.ID)){
				partInventory[p.ID] += number;
			}
			else{
				partInventory.Add(p.ID,number);
			}
            
		}
		else{
			// \todo Exceptions
		}
	}

	public void use(Part p, int number){
	    partInventory[p.ID] -= number;
	}
	//-----------------------------------------MISC

	//if bool is true, button made visible, if false button made invisble
	public void setButtonVisible(String name,bool visible){
		GameObject button = GameObject.Find(name);
		if (!visible) {
			button.GetComponent<CanvasGroup> ().alpha = 0;
			button.GetComponent<Button> ().interactable = false;
			button.GetComponent<CanvasGroup> ().interactable = false;
		} else {
			button.GetComponent<CanvasGroup>().alpha = 1;
			button.GetComponent<Button>().interactable = true;
			button.GetComponent<CanvasGroup>().interactable = true;
		}
	}

	public void setChapterToNone(){
		chapter = pickedType.None;
		chapterPage = 0;
		setButtonVisible ("Index", false);
		outProject[0].text = "Research";
		outProject[1].text = "Software";
		outProject[2].text = "Hardware";
		outProject[3].text = "Parts";
		if (!PossibleSoftware.Any()) {
			setButtonVisible ("R2", false);
		} else {
			setButtonVisible ("R2", true);
		}
		if (!PossibleHardware.Any ()) {
			setButtonVisible ("R3", false);
			setButtonVisible ("R4", false);
		} else {
			setButtonVisible ("R3", true);
			setButtonVisible ("R4", true);
		}
		setButtonVisible ("R5", false);
		setButtonVisible ("Next", false);
		setButtonVisible ("Previous", false);
	}

	public void setChapterToResearch(){
		chapter = pickedType.Research;
		setButtonVisible ("R2", true);
		setButtonVisible ("R3", true);
		setButtonVisible ("R4", true);
		setButtonVisible ("R5", true);
		AllPossibleResearchByKey = SortResearchByKey (AllPossibleResearch);
		List<Research> temp = AllPossibleResearchByKey;
		int i = 0;
		foreach(Text field in outProject){
			int position = 0+(chapterPage*BUTTON_COUNT)+i;
			try{
				field.text = temp[position].name + ":  " +temp[position].cost;
				field.GetComponent<WorkID>().ID = temp[position].ID;
			}
			catch(ArgumentOutOfRangeException){
				field.text = "???";
			}
			i++;
		}
		setButtonVisible ("Index", true);
		if(chapterPage == 0){
			GameObject button;
			setButtonVisible ("Previous", false);
			if(temp.Count <= BUTTON_COUNT+(chapterPage*BUTTON_COUNT)){
				setButtonVisible ("Next", false);
			}
			else{
				setButtonVisible ("Next", true);
			}
		}
	}

	public void setChapterToSoftware(){
		chapter = pickedType.Software;
		setButtonVisible ("R2", true);
		setButtonVisible ("R3", true);
		setButtonVisible ("R4", true);
		setButtonVisible ("R5", true);
		List<SoftwareProject> temp = PossibleSoftware;
		int i = 0;
		foreach(Text field in outProject){
			int position = 0+(chapterPage*BUTTON_COUNT)+i;
			try{
				field.text = temp[position].name + ":  " +temp[position].pointCost;
				field.GetComponent<WorkID>().ID = temp[position].ID;
			}
			catch(ArgumentOutOfRangeException){
				field.text = "???";
			}
			i++;
		}
		setButtonVisible ("Index", true);
		if(chapterPage == 0){
			GameObject button;
			setButtonVisible ("Previous", false);
			if(temp.Count <= BUTTON_COUNT+(chapterPage*BUTTON_COUNT)){
				setButtonVisible ("Next", false);
			}
		}
	}

	public void setChapterToHardware(){
		chapter = pickedType.Hardware;
		setButtonVisible ("R2", true);
		setButtonVisible ("R3", true);
		setButtonVisible ("R4", true);
		setButtonVisible ("R5", true);
		List<HardwareProject> temp = PossibleHardware;
		int i = 0;
		foreach(Text field in outProject){
			int position = 0+(chapterPage*BUTTON_COUNT)+i;
			try{
				field.text = temp[position].name;
				field.GetComponent<WorkID>().ID = temp[position].ID;
			}
			catch(ArgumentOutOfRangeException){
				field.text = "???";
			}
			i++;
		}
		setButtonVisible ("Index", true);
		if(chapterPage == 0){
			GameObject button;
			setButtonVisible ("Previous", false);
			if(temp.Count <= BUTTON_COUNT+(chapterPage*BUTTON_COUNT)){
				setButtonVisible ("Next", false);
			}
			else{
				setButtonVisible ("Next", true);
			}
		}
	}

	public void setChapterToParts(){
		chapter = pickedType.Parts;
		setButtonVisible ("R2", true);
		setButtonVisible ("R3", true);
		setButtonVisible ("R4", true);
		setButtonVisible ("R5", true);
		List<Part> temp = allParts;
		int i = 0;
		foreach(Text field in outProject){
			int position = 0+(chapterPage*BUTTON_COUNT)+i;
			try{
				field.text = temp[position].name;
				field.GetComponent<WorkID>().ID = temp[position].ID;
			}
			catch(ArgumentOutOfRangeException){
				field.text = "???";
			}
			i++;
		}
		setButtonVisible ("Index", true);
		if(chapterPage == 0){
			GameObject button;
			setButtonVisible ("Previous", false);
			if(temp.Count <= BUTTON_COUNT+(chapterPage*BUTTON_COUNT)){
				setButtonVisible ("Next", false);
			}
			else{
				setButtonVisible ("Next", true);
			}
		}
	}


	public int pageNumber(){
		return chapterPage;
	}

	public void next(){
		chapterPage+=1;
		if(chapter == pickedType.Research){
			List<Research> temp = AllPossibleResearchByKey;
			int i = 0;
			foreach(Text field in outProject){
				int position = 0+(chapterPage*BUTTON_COUNT)+i;
				try{
					field.text = temp[position].name + ": " +temp[position].cost;
					field.GetComponent<WorkID>().ID = temp[position].ID;
				}
				catch(ArgumentOutOfRangeException){
					field.text = "???";
				}
				i++;
			}
			setButtonVisible ("Previous", true);
			if(temp.Count <= BUTTON_COUNT+(chapterPage*BUTTON_COUNT)){
				setButtonVisible("Next",false);
			}
			flicked = true;
		}
		else if(chapter == pickedType.Software){
			List<SoftwareProject> temp = PossibleSoftware;
			int i = 0;
			foreach(Text field in outProject){
				int position = 0+(chapterPage*BUTTON_COUNT)+i;
				try{
					field.text = temp[position].name + ": " +temp[position].pointCost;
					field.GetComponent<WorkID>().ID = temp[position].ID;
				}
				catch(ArgumentOutOfRangeException){
					field.text = "???";
				}
				i++;
			}
			setButtonVisible ("Previous", true);
			if(temp.Count <= BUTTON_COUNT+(chapterPage*BUTTON_COUNT)){
				setButtonVisible("Next",false);
			}
			flicked = true;
		}
		else if(chapter == pickedType.Hardware){
			List<HardwareProject> temp = PossibleHardware;
			int i = 0;
			foreach(Text field in outProject){
				int position = 0+(chapterPage*BUTTON_COUNT)+i;
				try{
					field.text = temp[position].name;
					field.GetComponent<WorkID>().ID = temp[position].ID;
				}
				catch(ArgumentOutOfRangeException){
					field.text = "???";
				}
				i++;
			}
			setButtonVisible ("Previous", true);
			if(temp.Count <= BUTTON_COUNT+(chapterPage*BUTTON_COUNT)){
				setButtonVisible("Next",false);
			}
			flicked = true;
		}
		else if(chapter == pickedType.Parts){
			List<Part> temp = allParts;
			int i = 0;
			foreach(Text field in outProject){
				int position = 0+(chapterPage*BUTTON_COUNT)+i;
				try{
					field.text = temp[position].name + ": £" +temp[position].cost;
					field.GetComponent<WorkID>().ID = temp[position].ID;
				}
				catch(ArgumentOutOfRangeException){
					field.text = "???";
				}
				i++;
			}
			setButtonVisible ("Previous", true);
			if(temp.Count <= BUTTON_COUNT+(chapterPage*BUTTON_COUNT)){
				setButtonVisible("Next",false);
			}
			flicked = true;
		}

		
	}
	
	public void previous(){
		chapterPage -= 1;
		if (chapter == pickedType.Research) {
			List<Research> temp = AllPossibleResearchByKey;
			int i = 0;
			foreach (Text field in outProject) {
				int position = 0 + (chapterPage * BUTTON_COUNT) + i;
				try {
					field.text = temp [position].name + ": " + temp [position].cost;
					field.GetComponent<WorkID> ().ID = temp [position].ID;
				} catch (ArgumentOutOfRangeException) {
					field.text = "???";
				}
				i++;
			}
			if (chapterPage == 0) {
				setButtonVisible ("Previous", false);
				if (temp.Count <= BUTTON_COUNT + (chapterPage * BUTTON_COUNT)) {
					setButtonVisible ("Next", false);
				} else {
					setButtonVisible ("Next", true);
				}
			} else if (chapterPage != 0) {
				setButtonVisible ("Previous", true);
				if (temp.Count <= BUTTON_COUNT + (chapterPage * BUTTON_COUNT)) {
					setButtonVisible ("Next", false);
				} else {
					setButtonVisible ("Next", true);
				}
			}
			flicked = true;
		} else if (chapter == pickedType.Software) {
			List<SoftwareProject> temp = PossibleSoftware;
			int i = 0;
			foreach (Text field in outProject) {
				int position = 0 + (chapterPage * BUTTON_COUNT) + i;
				try {
					field.text = temp [position].name + ": " + temp [position].pointCost;
					field.GetComponent<WorkID> ().ID = temp [position].ID;
				} catch (ArgumentOutOfRangeException) {
					field.text = "???";
				}
				i++;
			}
			if (chapterPage == 0) {
				setButtonVisible ("Previous", false);
				if (temp.Count <= BUTTON_COUNT + (chapterPage * BUTTON_COUNT)) {
					setButtonVisible ("Next", false);
				} else {
					setButtonVisible ("Next", true);
				}
			} else if (chapterPage != 0) {
				setButtonVisible ("Previous", true);
				if (temp.Count <= BUTTON_COUNT + (chapterPage * BUTTON_COUNT)) {
					setButtonVisible ("Next", false);
				} else {
					setButtonVisible ("Next", true);
				}
			}
			flicked = true;
		} else if (chapter == pickedType.Hardware) {
			List<HardwareProject> temp = PossibleHardware;
			int i = 0;
			foreach (Text field in outProject) {
				int position = 0 + (chapterPage * BUTTON_COUNT) + i;
				try {
					field.text = temp [position].name;
					field.GetComponent<WorkID> ().ID = temp [position].ID;
				} catch (ArgumentOutOfRangeException) {
					field.text = "???";
				}
				i++;
			}
			if (chapterPage == 0) {
				setButtonVisible ("Previous", false);
				if (temp.Count <= BUTTON_COUNT + (chapterPage * BUTTON_COUNT)) {
					setButtonVisible ("Next", false);
				} else {
					setButtonVisible ("Next", true);
				}
			} else if (chapterPage != 0) {
				setButtonVisible ("Previous", true);
				if (temp.Count <= BUTTON_COUNT + (chapterPage * BUTTON_COUNT)) {
					setButtonVisible ("Next", false);
				} else {
					setButtonVisible ("Next", true);
				}
			}
			flicked = true;
		} else if (chapter == pickedType.Parts) {
			List<Part> temp = allParts;
			int i = 0;
			foreach (Text field in outProject) {
				int position = 0 + (chapterPage * BUTTON_COUNT) + i;
				try {
					field.text = temp [position].name + ": " + temp [position].cost;
					field.GetComponent<WorkID> ().ID = temp [position].ID;
				} catch (ArgumentOutOfRangeException) {
					field.text = "???";
				}
				i++;
			}
			if (chapterPage == 0) {
				setButtonVisible ("Previous", false);
				if (temp.Count <= BUTTON_COUNT + (chapterPage * BUTTON_COUNT)) {
					setButtonVisible ("Next", false);
				} else {
					setButtonVisible ("Next", true);
				}
			} else if (chapterPage != 0) {
				setButtonVisible ("Previous", true);
				if (temp.Count <= BUTTON_COUNT + (chapterPage * BUTTON_COUNT)) {
					setButtonVisible ("Next", false);
				} else {
					setButtonVisible ("Next", true);
				}
			}
			flicked = true;
		}
	}

	//-----------------------------------------UNITY METHODS

	void Awake() {
		score = GameObject.Find("PointsText").GetComponent<Text>();
		moneyScore = GameObject.Find ("MoneyText").GetComponent<Text> ();
		powerLevel = GameObject.Find ("PowerText").GetComponent<Text> ();
		ppc = GameObject.Find ("PointsPerClick").GetComponent<Text> ();
		ppt = GameObject.Find ("PointsPerTick").GetComponent<Text> ();
		mpt = GameObject.Find ("MoneyPerTick").GetComponent<Text> ();
		potentialResearch = GameObject.Find("Picker").GetComponent<Text>();
		r1 = GameObject.Find ("r1").GetComponent<Text> ();
		r2 = GameObject.Find ("r2").GetComponent<Text> ();
		r3 = GameObject.Find ("r3").GetComponent<Text> ();
		r4 = GameObject.Find ("r4").GetComponent<Text> ();
		r5 = GameObject.Find ("r5").GetComponent<Text> ();
		picker = GameObject.Find("Picker");
		inProgress = GameObject.Find ("WorkInProgress");
		inProgress.active = false;
		info = GameObject.Find("MoreInfo");
		info.active = false;
		researchPoints = 0;
		money = 0;
	}

    // Use this for initialization
	void Start () {
		incrementalTickTime = 1;
		incrementalTickIterations = 40;
		InvokeRepeating ("addMoneyPerSecond", 0f, 1.0f);
		InvokeRepeating ("addPointsPerSecond", 0f, 1.0f);
		outProject.Add (r1);
		outProject.Add (r2);
		outProject.Add (r3);
		outProject.Add (r4);
		outProject.Add (r5);
		//ResearchRoot researchXML = ResearchRoot.LoadFromFile(@"./Assets/Data/Research.xml");
//		PartRoot partXML = PartRoot.LoadFromFile(@"./Assets/Data/Part.XML");
//		ProjectRoot projectXML = ProjectRoot.LoadFromFile(@"./Assets/Data/Project.XML");
//		AllUncompleteResearch = researchXML.Research;
//        allProjects = projectXML.Project;
//		allParts = partXML.Part;
	    using (DatabaseConnection connection = new DatabaseConnection()) {
	        allHardwareProjects = connection.GetAllHardwareProjects().ToList();
	        allSoftwareProjects = connection.GetAllSoftwareProjects().ToList();
	        allResearch = connection.GetAllResearch().ToList();
	        allParts = connection.GetAllParts().ToList();

	    }
		foreach (Research r in allResearch) {
			AllUncompleteResearch.Add(r.ID,r);
		} 
		setChapterToNone ();


	    //AllCompleteResearch.Add(new Research("Robotics", "Cool robots", 200, 1, new Research[]{}, true));
	    //AllUncompleteResearch.Add(new Research("Computer Components", "Wow, you put together your own computer!", 100, 0, new Research[]{}, false));
	    //AllUncompleteResearch.Add(new Research("Basic Physics", "Learning the basics is a step on the way to discovering the meaning of life", 300, 0, new Research[]{}, false));
	    //AllUncompleteResearch.Add(new Research("Lasers", "Cool robots", 500, 1, new Research[]{Research.getResearchByName("Robotics")}, false));
	    //AllUncompleteResearch.Add(new Research("PDMS", "Point defence missile system protects against enemy robots (that you invented anyway...)", 800, 1, new Research[] { Research.getResearchByName("Robotics"), Research.getResearchByName("Lasers") }, false));
		    
	}

    // Update is called once per frame
	void Update () {
		//List<Research> temp = AllPossibleResearch;
		if (ticker == Priority.REALTIME) {
			// Ticks every frame
		}
		if (ticker == Priority.HIGH) {
			// Ticks every 5 frames
		}
		if (ticker == Priority.MEDIUM) {
			// Ticks every 10 frames
			score.text = researchPoints.ToString ()+"RP";
			moneyScore.text = "$"+ money.ToString();
			powerLevel.text = processingPower.ToString()+"MHz";
			ppc.text = (pointsPerClick).ToString()+" Points/Click";
			ppt.text = (pointsPerSecond * pointsMult).ToString()+" Points/Second";
			mpt.text = (moneyPerSecond * moneyMultiplier).ToString()+" Money/Second";

		}
		if (ticker == Priority.LOW) {
			// Ticks every 15 frames
			flicked = false;
			ticker = 0;
		} else
			ticker++;
	}
}
