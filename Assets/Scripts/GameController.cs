#define DEBUG
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using Incremental.Database;

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
 * @updated 26/06/2015
 */

public sealed class GameController : MonoBehaviour {

	//called whenever research is finished, set to a high number, prevents click spamming opening the browser
	public int justFinished = 0;
	public int empID = 0;
	public GameObject picker;
	public GameObject inProgress;
	public GameObject browser;
	public GameObject info;
	public List<Research> allResearch;
	public Dictionary<int,HardwareProject> allHardwareProjects = new Dictionary<int, HardwareProject>();
    Dictionary<int,SoftwareProject> allSoftwareProjects = new Dictionary<int,SoftwareProject >();
	public Dictionary<int,SoftwareProject> courses = new Dictionary<int,SoftwareProject> ();

	public ResearchController userRControl{
		get{
			return empControl.Player.employeeResearch;
		}
	}

	public SoftwareController userSControl; 
	public HardwareController userHControl;
	public PartController pControl;
	public ComputerController compControl;
	public EmployeeController empControl;

    // Prevents initialisation; the static instance MUST be used
    private GameController() {
        
    }

    private static GameController _instance;

    /**
     * @property    public static GameController instance
     *
     * @brief   Gets the singleton instance of the GameController (this) class.
     * @return  The instance.
     */
    public static GameController instance {
        get {
			if (_instance == null){
				_instance = GameObject.FindObjectOfType<GameController>();
			    if (_instance == null) {
			        Utility.UnityLog("Could not find singular GameController instance! Are you sure the GameController gameobject exists?", LogLevels.ERROR);
			    }
			}
           
            return _instance;
        }
    }



	//-----------DEBUG
	public bool debugResearchPoints;

	//-----------Incremental Values
    private const int BASE_POINTS_PER_CLICK = 1;
	private const double BASE_PROCESSING_POWER = 1.00;
	public int researchPoints {
		get;
		private set;
	}
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

	public void addPointsPerSecond(){
		empControl.addPoints (false);
	}

	public void addPointsPerClick(){
		empControl.addPoints (true);
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


	//-----------------------------------------UNITY METHODS

	void Awake() {
		userSControl = new SoftwareController ();
		userHControl = new HardwareController ();
		pControl = new PartController ();
		compControl = new ComputerController ();
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
		browser = GameObject.Find ("FileBrowser");
		browser.active = false;
		info = GameObject.Find("MoreInfo");
		info.active = false;
		researchPoints = 0;
		money = 0;
	}

    // Use this for initialization
	void Start () {
        incrementalTickTime = 1;
		incrementalTickIterations = 40;
		InvokeRepeating ("addPointsPerSecond", 0f, 1.0f);
		outProject.Add (r1);
		outProject.Add (r2);
		outProject.Add (r3);
		outProject.Add (r4);
		outProject.Add (r5);

	    using (DatabaseConnection connection = new DatabaseConnection()) {
	        List<HardwareProject> parseableHardware = connection.GetAllHardwareProjects().ToList();
			List<SoftwareProject> parseableSoftware = connection.GetAllSoftwareProjects().ToList();
			foreach(HardwareProject h in parseableHardware){
				if(h.ID !=3){
					allHardwareProjects.Add(h.ID,h);
				}
				else{
					compControl.AllCompletedComputers.Add (h);
					compControl.setMannedComputer(h);

				}
				
			}
			foreach(SoftwareProject p in parseableSoftware){
				if(p.ID !=42){
					allSoftwareProjects.Add(p.ID,p);
				}
				else{
					userSControl.AllCompletedOS.Add (p);
					compControl.mannedComputer.primaryOS = p;
					
				}
				
			}
	        allResearch = connection.GetAllResearch().ToList();
	        allParts = connection.GetAllParts().ToList();
			foreach(Part p in allParts){
				pControl.partInventory.Add(p.ID,0);
			}

	    }
		PickerController.instance.setChapterToNone ();
		foreach(SoftwareProject s in allSoftwareProjects.Values.ToList()){
			if(s.SoftwareType = SoftwareProject.type.Course){
				courses.Add(s.ID,s);
			}
			else{
				allSoftwareProjects.Add (s.ID,s);
			}
		}

	    //AllCompleteResearch.Add(new Research("Robotics", "Cool robots", 200, 1, new Research[]{}, true));
	    //AllUncompleteResearch.Add(new Research("Computer Components", "Wow, you put together your own computer!", 100, 0, new Research[]{}, false));
	    //AllUncompleteResearch.Add(new Research("Basic Physics", "Learning the basics is a step on the way to discovering the meaning of life", 300, 0, new Research[]{}, false));
	    //AllUncompleteResearch.Add(new Research("Lasers", "Cool robots", 500, 1, new Research[]{Research.getResearchByName("Robotics")}, false));
	    //AllUncompleteResearch.Add(new Research("PDMS", "Point defence missile system protects against enemy robots (that you invented anyway...)", 800, 1, new Research[] { Research.getResearchByName("Robotics"), Research.getResearchByName("Lasers") }, false));
		    
	}

    // Update is called once per frame
	void Update () {
		//List<Research> temp = AllPossibleResearch;
		if (ticker == Priority.HIGH) {
			// Ticks every 5 frames

		}
		if (ticker == Priority.MEDIUM) {
			// Ticks every 10 frames
			score.text = researchPoints.ToString ()+"RP";
			moneyScore.text = "$"+ money.ToString();


		}
		if (ticker == Priority.LOW) {
			// Ticks every 15 frames
			if(justFinished>0){
				justFinished--;
			}
			PickerController.instance.flicked = false;
			ticker = 0;
		} else
			ticker++;
	}
}
