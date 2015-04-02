#define DEBUG
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using Incremental.XML;
#if DEBUG
using System.Diagnostics;
#endif
public class GameController : MonoBehaviour {

    List<Project> allProjects;
    List<Research> allResearch;

    // See http://unitypatterns.com/singletons/ for more details. Alternatively, google C# singleton.
    private static GameController _instance;
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
	private int BASE_POINTS_PER_CLICK = 10;
	public double money = 0.00;
	private double BASE_PROCESSING_POWER = 1.23;

	//----------Outputs
	private Text score;
	private Text tick;
	private Text potentialResearch;

	//-----------------------------------------------------CORE METHODS

	//adds points if you should be able to add points:
	//1.Research has been set
	//2. Research hasn't been finished.
	public void addResearchPoints(int points){
		if ((isResearchSet () && currentResearch.cost > researchPoints) || debugResearchPoints) {
			this.researchPoints += points;
		} else if (isResearchSet () && !isSoftware) {
			stopResearch ();
			this.researchPoints = 0;
		} else {
			finishSoftware();
			this.researchPoints = 0;
		}
		
	}

	//TODO: Calculate sum of all multipliers, points etc in projects, 
	//TODO: Add this to the update method, then we'll have a game!

	public int pointsPerSecond{
		get{
			int temp = 0;
			List<SoftwareProject> relevantSoftware = CompletedSoftware.Where(x => x.pointsPerSecond> 0).ToList();
			List<HardwareProject> relevantHardware = CompletedHardware.Where(x => x.pointsPerSecond> 0).ToList();
			foreach(SoftwareProject item in relevantSoftware){
				temp+=  item.pointsPerSecond;
			}
			foreach(HardwareProject item in relevantHardware){
				temp+=  item.pointsPerSecond;
			}
			return temp;
		}
	}

	public int pointsPerClick{
		get{
			int temp = BASE_POINTS_PER_CLICK;
			List<SoftwareProject> relevantSoftware = CompletedSoftware.Where(x => x.pointsPerClick> 0).ToList();
			List<HardwareProject> relevantHardware = CompletedHardware.Where(x => x.pointsPerClick> 0).ToList();
			foreach(SoftwareProject item in relevantSoftware){
				temp+=  item.pointsPerClick;
			}
			foreach(HardwareProject item in relevantHardware){
				temp+=  item.pointsPerClick;
			}
			return temp;
		}
	}

	public int pointsMult{
		get{
			int temp = BASE_POINTS_PER_CLICK;
			List<SoftwareProject> relevantSoftware = CompletedSoftware.Where(x => x.pointMult> 0).ToList();
			List<HardwareProject> relevantHardware = CompletedHardware.Where(x => x.pointMult> 0).ToList();
			foreach(SoftwareProject item in relevantSoftware){
				temp+=  item.pointMult;
			}
			foreach(HardwareProject item in relevantHardware){
				temp+=  item.pointMult;
			}
			return temp;
		}
	}

	public int processingPower{
		get{
			int temp = 0;
			List<SoftwareProject> relevantSoftware = CompletedSoftware.Where(x => x.processIncrease> 0).ToList();
			List<HardwareProject> relevantHardware = CompletedHardware.Where(x => x.processIncrease> 0).ToList();
			foreach(SoftwareProject item in relevantSoftware){
				temp+=  item.processIncrease;
			}
			foreach(HardwareProject item in relevantHardware){
				temp+=  item.processIncrease;
			}
			return temp;
		}
	}

	public double moneyPerSecond{
		get{
			int temp = 0;
			List<SoftwareProject> relevantSoftware = CompletedSoftware.Where(x => x.moneyperSecond> 0.00).ToList();
			List<HardwareProject> relevantHardware = CompletedHardware.Where(x => x.moneyperSecond> 0.00).ToList();
			foreach(SoftwareProject item in relevantSoftware){
				temp+=  item.moneyperSecond;
			}
			foreach(HardwareProject item in relevantHardware){
				temp+=  item.moneyperSecond;
			}
			return temp;
		}
	}

	public int moneyMultiplier{
		get{
			int temp = 0;
			List<SoftwareProject> relevantSoftware = CompletedSoftware.Where(x => x.moneyMult> 0).ToList();
			List<HardwareProject> relevantHardware = CompletedHardware.Where(x => x.moneyMult> 0).ToList();
			foreach(SoftwareProject item in relevantSoftware){
				temp+=  item.moneyMult;
			}
			foreach(HardwareProject item in relevantHardware){
				temp+=  item.moneyMult;
			}
			return temp;
		}
	}


	//-----------------------------------------------------RESEARCH METHODS AND DATA
	private bool researchSet;
	public List<Research> AllUncompleteResearch = new List<Research>();
	public List<Research> AllCompleteResearch;
	//May need to create another list ordered by ID to make finding completed
	//research more efficient. For now though, it is not necessary.
	//private List<Research> AllResearch = new List<Research>();

	public bool isResearchSet(){
		return researchSet;
	}

	public Research currentResearch {
		get;
		private set;
	}
	
	public int researchPoints {
		get;
		private set;
	}

	//returns index of a paticular piece of Research in the array
	private int getIndexOfUncompletedResearch(Research a){
		for(int i = 0; i<AllUncompleteResearch.Count; i++){
			if(AllUncompleteResearch[i].string_id.CompareTo(a.string_id) == 0){
				return i;
			}
		}
		return -1;
	}
	
	public void startResearch(Research research){
		researchSet = true;
		currentResearch = research;
	}
	
	//removes from UncompleteResearch, adds to completedResearch
	public void stopResearch(){
		researchSet = false;
		int index = getIndexOfUncompletedResearch (currentResearch);
		AllUncompleteResearch [index] = null;
		AllUncompleteResearch.Sort ();
		currentResearch.complete ();
		AllCompleteResearch.Add (currentResearch);
		AllCompleteResearch.Sort ();
		currentResearch = null;
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
	
	public bool hasBeenDone(string id){
		for (int i = 0; i<AllCompleteResearch.Count; i++) {
			if (AllCompleteResearch [i].string_id.CompareTo(id) == 0) {
				return true;
			}
		}
		return false;
	}
	
	public bool canBeDone(Research a){
		if(a.processingLevel > processingPower){
			return false;
		}
		for (int i = 0; i<a.Dependencies.Count; i++) {
            string temp = a.Dependencies[i].name;
			if(!hasBeenDone(temp)){
				return false;
			}
		}
		return true;
	}
	
	//Only displays the research that can be done
	public List<Research> AllPossibleResearch{
		get{
			List<Research> temp = AllUncompleteResearch.Where(x => canBeDone(x)).ToList();
			temp.Sort();
			return temp;
			
		}
		/*List<Research> underConsideration = new List<Research> ();
		for (int i = 0; i<AllUncompleteResearch.Count; i++) {
			if(canBeDone(AllUncompleteResearch[i])){
				underConsideration.Add(AllUncompleteResearch[i]);
				underConsideration.Sort();
			}
			else if(AllUncompleteResearch[i].processingLevel > processingPower){
				break;
			}
		}
		return underConsideration;*/

	}

	//-----------------------------------------------------SOFTWARE

	//All Projects that have not been done or are repeatable
	public List<SoftwareProject> SoftwareStillDoable = new List<SoftwareProject> ();
	public List<SoftwareProject> CompletedSoftware = new List<SoftwareProject> ();

	public List<SoftwareProject> PossibleSoftware{
		get{
			List<SoftwareProject> temp = SoftwareStillDoable.Where(x => x.canDo()).ToList();
			return temp;
		}
	}

	//used in ComputerOnClick, tells the method 
	//not to treat it as research
	public bool isSoftware {
		get;
		private set;
	}

	public SoftwareProject currentSoftware {
		get;
		private set;
	}


	private int getIndexOfUncompletedSoftware(SoftwareProject a){
		for(int i = 0; i<SoftwareStillDoable.Count; i++){
			if(SoftwareStillDoable[i].string_id.CompareTo(a.string_id)==0){
				return i;
			}
		}
		return -1;
	}

	public void startSoftware(SoftwareProject a){
		Research temp = new Research (a.pointCost);
		currentSoftware = a;
		isSoftware = true;
		currentResearch = temp;
		researchSet = true;

	}

	public void finishSoftware(){
		int index = getIndexOfUncompletedSoftware (currentSoftware);
		if(currentSoftware.canDoMultiple){
			CompletedSoftware.Add(currentSoftware);
		}
		else{
			CompletedSoftware.Add(currentSoftware);
			SoftwareStillDoable.Remove(currentSoftware);
		}
		currentSoftware = null;
		isSoftware = false;
		currentResearch = null;
		researchSet = false;
	}

	//-----------------------------------------------------HARDWARE

	public List<HardwareProject> HardwareStillDoable = new List<HardwareProject> ();
	public List<HardwareProject> CompletedHardware = new List<HardwareProject> ();

	public List<HardwareProject> PossibleHardware{
		get{
			List<HardwareProject> temp = HardwareStillDoable.Where(x => x.canDo()).ToList();
			return temp;
		}
	}

	private int getIndexOfUncompletedHardware(HardwareProject a){
		for(int i = 0; i<HardwareStillDoable.Count; i++){
			if(HardwareStillDoable[i].string_id.CompareTo(a.string_id)==0){
				return i;
			}
		}
		return -1;
	}

	private void useRequiredParts(HardwareProject a){
		foreach (var item in a.requiredParts) {
			int index = indexOfPart(item.Key);
			use(index, item.Value);
		}
	}

	public void makeHardware(HardwareProject a){
		int index = getIndexOfUncompletedHardware(a);
		if(a.canDoMultiple){
			CompletedHardware.Add(a);
			useRequiredParts(a);

		}
		else{
			CompletedHardware.Add(a);
			HardwareStillDoable.Remove(a);
			useRequiredParts(a);
		}

	}


	//-----------------------------------------------------TICK METHODS AND DATA
	// A tick is the baseline time measure but it really depends on the speed of the CPU
	// and any lag that occurs between each frame. Delta time is basically how long the last frame
	// took to render. Thus it accurately tells us how long that tick should be.

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
	public List<Part> allParts = new List<Part> ();
	
	//likely a mthod to be deprecated by xml
	public bool hasParts(string name, int number){
		for (int i = 0; i<allParts.Count; i++) {
			if(allParts[i].string_id.CompareTo(name) == 0){
				if(allParts[i].numberOwned == number){
					return true;
				}
				else return false;
			}
		}
		return false;
	}

	public int indexOfPart (Part a){
		for(int i = 0; i<allParts.Count; i++){
			if(allParts[i].string_id.CompareTo(a.string_id)==0){
				return i;
			}
		}
		return -1;
	}
	
	public void buyPart(int index, int number){
		if (money >= (allParts [index].cost * number)) {
			allParts[index].buy(number);
			money-= (allParts [index].cost * number);
		}
		else{
			//TODO: Exceptions
		}
	}
	
	public void use(int index, int number){
		if(allParts[index].numberOwned >= number){
			allParts[index].use(number);
		}
	}

	//-----------------------------------------BASIC METHODS
    
	void Awake() {
		score = GameObject.Find("PointsText").GetComponent<Text>();
		tick = GameObject.Find("Ticks").GetComponent<Text>();
		potentialResearch = GameObject.Find("Research").GetComponent<Text>();
		researchPoints = 0;
		money = 0;
	}

	// Use this for initialization
	void Start () {
		// XML load
		incrementalTickTime = 1;
		incrementalTickIterations = 40;
		ResearchRoot researchXML = ResearchRoot.LoadFromFile(@"./Assets/Data/Research.xml");
		PartRoot partXML = PartRoot.LoadFromFile(@"./Assets/Data/Part.XML");
		ProjectRoot projectXML = ProjectRoot.LoadFromFile(@"./Assets/Data/Project.XML");
		allResearch = researchXML.Research;
        allProjects = projectXML.Project;
		allParts = partXML.Part;

		//AllCompleteResearch.Add(new Research("Robotics", "Cool robots", 200, 1, new Research[]{}, true));
		//AllUncompleteResearch.Add(new Research("Computer Components", "Wow, you put together your own computer!", 100, 0, new Research[]{}, false));
		//AllUncompleteResearch.Add(new Research("Basic Physics", "Learning the basics is a step on the way to discovering the meaning of life", 300, 0, new Research[]{}, false));
		//AllUncompleteResearch.Add(new Research("Lasers", "Cool robots", 500, 1, new Research[]{Research.getResearchByName("Robotics")}, false));
		//AllUncompleteResearch.Add(new Research("PDMS", "Point defence missile system protects against enemy robots (that you invented anyway...)", 800, 1, new Research[] { Research.getResearchByName("Robotics"), Research.getResearchByName("Lasers") }, false));
	}
	
	// Update is called once per frame
	void Update () {
		if (ticker == Priority.REALTIME) {
			// Ticks every frame
		}
		if (ticker == Priority.HIGH) {
			// Ticks every 5 frames
		}
		if (ticker == Priority.MEDIUM) {
			// Ticks every 10 frames
			//AllPossibleResearch = AllUncompleteResearch.Where(x => (x.cost <= researchPoints) && !x.Dependencies.Except(AllCompleteResearch).Any() ).ToList();
			string text = "";
			foreach (Research research in AllPossibleResearch) {
				text += research.string_id + "\n";
			}
			researchPoints+= pointsPerSecond*pointsMult;
			money+= moneyPerSecond*moneyMultiplier;
			score.text = researchPoints.ToString ();
			potentialResearch.text = text;
			tick.text = incrementalTickTime.ToString ();
		}
		if (ticker == Priority.LOW) {
			// Ticks every 15 frames
			ticker = 0;
		} else
			ticker++;
	}
}
