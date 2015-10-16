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
	public GameObject picker;
	public GameObject inProgress;
	public GameObject browser;
	public GameObject info;
	public Dictionary<int,HardwareProject> allHardwareProjects = new Dictionary<int, HardwareProject>();
    public Dictionary<int,SoftwareProject> allSoftwareProjects = new Dictionary<int,SoftwareProject >();
	public ResearchController rControl;
	public SoftwareController sControl;
	public HardwareController hControl;
	public PartController pControl;
	public ComputerController compControl;
	public List<Research> allResearch;

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
		if(rControl.isResearchSet()){
			if ((rControl.currentResearch.cost > researchPoints+points) || debugResearchPoints) {
				researchPoints += points;
			} 
			 else {
				rControl.finishResearch();
				researchPoints = 0;
			}
		}
		else if(sControl.isSoftwareSet){
			if ((sControl.currentSoftware.pointCost > researchPoints+points) || debugResearchPoints) {
				researchPoints += points;
			}
			else {
				sControl.finishSoftware();
				researchPoints = 0;
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
			List<SoftwareProject> relevantSoftware = sControl.AllCompletedSoftware.Where(x => x.pointsPerTick> 0).ToList();
			List<HardwareProject> relevantHardware = hControl.AllCompletedGenericHardware.Where(x => x.pointsPerTick> 0).ToList();
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
			List<SoftwareProject> relevantSoftware = sControl.AllCompletedSoftware.Where(x => x.pointsPerClick> 0).ToList();
			List<HardwareProject> relevantHardware = hControl.AllCompletedGenericHardware.Where(x => x.pointsPerClick> 0).ToList();
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
			List<SoftwareProject> relevantSoftware = sControl.AllCompletedSoftware.Where(x => x.pointMult> 0).ToList();
            List<HardwareProject> relevantHardware = hControl.AllCompletedGenericHardware.Where(x => x.pointMult > 0).ToList();
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
			List<SoftwareProject> relevantSoftware = sControl.AllCompletedSoftware.Where(x => x.processIncrease> 0.00).ToList();
			List<HardwareProject> relevantHardware = hControl.AllCompletedGenericHardware.Where(x => x.processIncrease> 0.00).ToList();
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
			List<SoftwareProject> relevantSoftware = sControl.AllCompletedCourses.Where(x => x.moneyPerTick> 0).ToList();
            List<HardwareProject> relevantHardware = hControl.AllCompletedGenericHardware.Where(x => x.moneyPerTick > 0).ToList();
			foreach(SoftwareProject item in relevantSoftware){
				temp+=  item.moneyPerTick;
			}
			foreach(HardwareProject item in relevantHardware){
				temp+=  item.moneyPerTick;
			}
			temp += compControl.getMoneyPerTick;
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
			List<SoftwareProject> relevantSoftware = sControl.AllCompletedSoftware.Where(x => x.moneyMult> 0).ToList();
            List<HardwareProject> relevantHardware = hControl.AllCompletedGenericHardware.Where(x => x.moneyMult > 0).ToList();
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
		if (rControl.researchSet) {
			addResearchPoints (toAdd);
		} else if (sControl.isSoftwareSet) {
			addResearchPoints(toAdd);
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


	//-----------------------------------------UNITY METHODS

	void Awake() {
		rControl = new ResearchController ();
		sControl = new SoftwareController ();
		hControl = new HardwareController ();
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

        using (DatabaseConnection connection = new DatabaseConnection()) {
            List<HardwareProject> parseableHardware = connection.GetAllHardwareProjects().ToList();
            List<SoftwareProject> parseableSoftware = connection.GetAllSoftwareProjects().ToList();
            foreach (HardwareProject h in parseableHardware) {
                if (h.ID != 3) {
                    allHardwareProjects.Add(h.ID, h);
                } else {
                    compControl.AllCompletedComputers.Add(h);
                    compControl.setMannedComputer(h);

                }

            }
            foreach (SoftwareProject p in parseableSoftware) {
                if (p.ID != 42) {
                    allSoftwareProjects.Add(p.ID, p);
                } else {
                    sControl.AllCompletedOS.Add(p);
                    compControl.mannedComputer.primaryOS = p;

                }

            }
            allResearch = connection.GetAllResearch().ToList();
            allParts = connection.GetAllParts().ToList();

        }
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

		PickerController.instance.setChapterToNone ();

        ScreenController.MainScreen();
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
			powerLevel.text = processingPower.ToString()+"MHz";
			ppc.text = (pointsPerClick).ToString()+" Points/Click";
			ppt.text = (pointsPerSecond * pointsMult).ToString()+" Points/Second";
			mpt.text = (moneyPerSecond * moneyMultiplier).ToString()+" Money/Second";

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
