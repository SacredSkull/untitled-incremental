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
 */

public class GameController : MonoBehaviour {

    List<Project> allProjects;
    List<Research> allResearch;

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
	private int BASE_POINTS_PER_CLICK = 10;
	/** @brief   Current money of the player. */
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

    /**
     * @fn  public void addResearchPoints(int points)
     *
     * @brief   Adds research points.
     *          
     * @details Adds research points if the research you are currently researching costs more than you currently have. If it costs less and isn't software, your current research is halted and your research points are set to zero. Finally, if any other condition (should only be software research) , your software is finished and points are set to zero.
     *
     * @author  Conal
     * @date    05/04/2015
     *
     * @param   points  The points to add.
     */

	public void addResearchPoints(int points){
		if ((isResearchSet () && currentResearch.cost > researchPoints) || debugResearchPoints) {
			this.researchPoints += points;
		} else if (isResearchSet () && !isSoftware) {
			finishResearch ();
			this.researchPoints = 0;
		} else {
			finishSoftware();
			this.researchPoints = 0;
		}

	}

	// 
	// 

    /**
     * @property    public int pointsPerSecond
     *
     * @brief   Adds up and returns the points per second of all completed software/hardware research.
     *          
     * 
     * @todo    Calculate sum of all multipliers, points etc in projects,
     *          
     * 
     * @todo    Add this to the update method, then we'll have a game!
     *          
     * 
     * @return  The points per second.
     */

    public int pointsPerSecond{
		get{
			int temp = 0;
			List<Project> relevantSoftware = CompletedSoftware.Where(x => x.pointsPerSecond> 0).ToList();
			List<Project> relevantHardware = CompletedHardware.Where(x => x.pointsPerSecond> 0).ToList();
			foreach(Project item in relevantSoftware){
				temp+=  item.pointsPerSecond;
			}
			foreach(Project item in relevantHardware){
				temp+=  item.pointsPerSecond;
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
			List<Project> relevantSoftware = CompletedSoftware.Where(x => x.pointsPerClick> 0).ToList();
			List<Project> relevantHardware = CompletedHardware.Where(x => x.pointsPerClick> 0).ToList();
			foreach(Project item in relevantSoftware){
				temp+=  item.pointsPerClick;
			}
			foreach(Project item in relevantHardware){
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
			int temp = BASE_POINTS_PER_CLICK;
			List<Project> relevantSoftware = CompletedSoftware.Where(x => x.pointMult> 0).ToList();
			List<Project> relevantHardware = CompletedHardware.Where(x => x.pointMult> 0).ToList();
			foreach(Project item in relevantSoftware){
				temp+=  item.pointMult;
			}
			foreach(Project item in relevantHardware){
				temp+=  item.pointMult;
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
			List<Project> relevantSoftware = CompletedSoftware.Where(x => x.processIncrease> 0.00).ToList();
			List<Project> relevantHardware = CompletedHardware.Where(x => x.processIncrease> 0.00).ToList();
			foreach(Project item in relevantSoftware){
				temp+=  item.processIncrease;
			}
			foreach(Project item in relevantHardware){
				temp+=  item.processIncrease;
			}
			return temp;
		}
	}

    /**
     * @property    public double moneyPerSecond
     *
     * @brief   Gets money per second.
     *
     * @return  The money per second.
     */

	public double moneyPerSecond{
		get{
			int temp = 0;
			List<Project> relevantSoftware = CompletedSoftware.Where(x => x.moneyPerSecond> 0.00).ToList();
			List<Project> relevantHardware = CompletedHardware.Where(x => x.moneyPerSecond> 0.00).ToList();
			foreach(Project item in relevantSoftware){
				temp+=  item.moneyPerSecond;
			}
			foreach(Project item in relevantHardware){
				temp+=  item.moneyPerSecond;
			}
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
			int temp = 0;
			List<Project> relevantSoftware = CompletedSoftware.Where(x => x.moneyMult> 0).ToList();
			List<Project> relevantHardware = CompletedHardware.Where(x => x.moneyMult> 0).ToList();
			foreach(Project item in relevantSoftware){
				temp+=  item.moneyMult;
			}
			foreach(Project item in relevantHardware){
				temp+=  item.moneyMult;
			}
			return temp;
		}
	}


	//-----------------------------------------------------RESEARCH METHODS AND DATA
	/** @brief   If research is set or not */
	private bool researchSet;
	/** @brief   All uncomplete research. */
	public List<Research> AllUncompleteResearch = new List<Research>();
	/** @brief   All complete research. */
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

    /**
     * @fn  private int getIndexOfUncompletedResearch(Research a)
     *
     * @brief   Gets the index of uncompleted research, using a passed Research object.
     *
     * @author  Conal
     * @date    05/04/2015
     *
     * @param   r   The Research to process.
     *
     * @return  The index of uncompleted research.
     */

	private int getIndexOfUncompletedResearch(Research r){
		for(int i = 0; i<AllUncompleteResearch.Count; i++){
			if(AllUncompleteResearch[i].string_id.CompareTo(r.string_id) == 0){
				return i;
			}
		}
		return -1;
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

    /**
     * @fn  public bool hasBeenDone(string id)
     *
     * @brief   Query if research of string_id 'id' has been done.
     *
     * @author  Conal
     * @date    01/04/2015
     *
     * @param   id  The string_id identifier of the research.
     *
     * @return  true if been done, false if not.
     */

	public bool hasBeenDone(string id){
        //Utility.UnityLog("  ");
		for (int i = 0; i<AllCompleteResearch.Count; i++) {
			if (AllCompleteResearch [i].string_id.CompareTo(id) == 0) {
				return true;
			}
		}
		return false;
	}

    /**
     * @fn  public bool canBeDone(Research r)
     *
     * @brief   Determine if Research 'r' can be done.
     *
     * @author  Conal
     * @date    01/04/2015
     *
     * @param   r   The Research to process.
     *
     * @return  true if we can be done, false if not.
     */

	public bool canBeDone(Research r){
        if(r.processingLevel > processingPower){
           return false;
		}
       for (int i = 0; i<r.Dependencies.Count; i++) {
            string temp = r.Dependencies[i].name;
			if(!hasBeenDone(temp)){
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
     * @return  all possible research.
     */

	public List<Research> AllPossibleResearch{
		get{
            Utility.UnityLog("Before:" + AllUncompleteResearch.Count);
            List<Research> temp = AllUncompleteResearch.Where(x => canBeDone(x)).ToList();
			temp.Sort();
            Utility.UnityLog("After:"+temp.Count);
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

    /**
     * @property    public List<Project> UnstartedSoftware
     *
     * @brief   All Projects that have not been done or are repeatable
     *
     * @return  The unstarted software.
     */

    public List<Project> UnstartedSoftware
    {
        get {
            List<Project> temp = new List<Project>();
            foreach (var item in allProjects) {
                if(item.type.Equals("Software")&& item.uses > 0){
                    temp.Add(item);
                }
            }
            return temp;
        }
    }
	public List<Project> CompletedSoftware = new List<Project> ();

	public List<Project> PossibleSoftware{
		get{
			List<Project> temp = UnstartedSoftware.Where(x => x.possible()).ToList();
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

	public Project currentSoftware {
		get;
		private set;
	}


	private int getIndexOfUncompletedSoftware(Project a){
		for(int i = 0; i<UnstartedSoftware.Count; i++){
			if(UnstartedSoftware[i].string_id.CompareTo(a.string_id)==0){
				return i;
			}
		}
		return -1;
	}

	public void startSoftware(Project a){
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
            allProjects[index].uses--;
		}
		currentSoftware = null;
		isSoftware = false;
		currentResearch = null;
		researchSet = false;
	}

	//-----------------------------------------------------HARDWARE

    public List<Project> UnstartedHardware {
        get {
            List<Project> temp = new List<Project>();
            foreach (var item in allProjects)
            {
                if (item.type.Equals("Hardware") && item.uses > 0)
                {
                    temp.Add(item);
                }
            }
            return temp;
        }
    }
	public List<Project> CompletedHardware = new List<Project> ();

	public List<Project> PossibleHardware{
		get{
			List<Project> temp = UnstartedHardware.Where(x => x.possible()).ToList();
			return temp;
		}
	}

	private int getIndexOfUncompletedHardware(Project a){
		for(int i = 0; i<UnstartedHardware.Count; i++){
			if(UnstartedHardware[i].string_id.CompareTo(a.string_id)==0){
				return i;
			}
		}
		return -1;
	}

	private void useRequiredParts(Project a){
		foreach (var item in a.partDependencies) {
			int index = indexOfPart(item.Key);
			use(index, item.Value);
		}
	}

	public void makeHardware(Project a){
		int index = getIndexOfUncompletedHardware(a);
		if(a.canDoMultiple){
			CompletedHardware.Add(a);
			useRequiredParts(a);

		}
		else{
			CompletedHardware.Add(a);
            allProjects[index].uses--;
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
	public List<Part> allParts = new List<Part>();

    public Dictionary<long, int> partInventory = new Dictionary<long, int>();

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

	public void buyPart(int id, int number){
        Part p = Part.getPartByID(id);
		if (money >= (p.cost * number)) {
			p.buy(number);
			money-= (p.cost * number);
            partInventory[(long)id] += number;
		}
		else{
			// \todo Exceptions
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
		AllUncompleteResearch = researchXML.Research;
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
            List<Research> temp = AllPossibleResearch;
			foreach (Research research in temp) {
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
