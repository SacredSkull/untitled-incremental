using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class GameController : MonoBehaviour {

    // See http://unitypatterns.com/singletons/ for more details. Alternatively, google C# singleton.
    private static GameController _instance;
    public static GameController instance {
        get {
            if (_instance == null)
                _instance = GameObject.FindObjectOfType<GameController>();
            return _instance;
        }
    }

    public bool debugResearchPoints;

	private double processingPower = 1.23;
	private Text score;
	private Text tick;
	private Text potentialResearch;
	private bool researchSet;
	public List<Research> AllUncompleteResearch = new List<Research>();
	public List<Research> AllCompleteResearch = new List<Research>();
	public List<Research> AllPossibleResearch;
	//May need to create another list ordered by ID to make finding completed
	//research more efficient. For now though, it is not necessary.
	//private List<Research> AllResearch = new List<Research>();

    // A tick is the baseline time measure but it really depends on the speed of the CPU
    // and any lag that occurs between each frame. Delta time is basically how long the last frame
    // took to render. Thus it accurately tells us how long that tick should be.
    
    // Note: This is the point ticker. See the ticker for updating GUIs/checking etc.
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

    private int ticker = 0;

    void Awake() {
        score = GameObject.Find("PointsText").GetComponent<Text>();
        tick = GameObject.Find("Ticks").GetComponent<Text>();
        potentialResearch = GameObject.Find("Research").GetComponent<Text>();
        researchPoints = 0;
    }
	// Use this for initialization
	void Start () {

        // XML load

        incrementalTickTime = 1;
        incrementalTickIterations = 40;
        //requirement test1duplicate = new requirement("Lasers");

        AllCompleteResearch.Add(new Research("Robotics", "Cool robots", 200, 1, new Research[]{}, true));
        AllUncompleteResearch.Add(new Research("Computer Components", "Wow, you put together your own computer!", 100, 0, new Research[]{}, false));
        AllUncompleteResearch.Add(new Research("Basic Physics", "Learning the basics is a step on the way to discovering the meaning of life", 300, 0, new Research[]{}, false));
        AllUncompleteResearch.Add(new Research("Lasers", "Cool robots", 500, 1, new Research[]{Research.getResearchByName("Robotics")}, false));
        AllUncompleteResearch.Add(new Research("PDMS", "Point defence missile system protects against enemy robots (that you invented anyway...)", 800, 1, new Research[] { Research.getResearchByName("Robotics"), Research.getResearchByName("Lasers") }, false));
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

            AllPossibleResearch = AllUncompleteResearch.Where(x => (x.researchCost <= researchPoints) && !x.prerequisites.Except(AllCompleteResearch).Any() ).ToList();

            string text = "";

            foreach (Research research in AllPossibleResearch) {
                text += research.name + "\n";
            }

            if (this.debugResearchPoints) {
                this.addResearchPoints(10);
            }

            score.text = researchPoints.ToString();
            potentialResearch.text = text;
            tick.text = incrementalTickTime.ToString();
        }

        if (ticker == Priority.LOW) {
            // Ticks every 15 frames

            ticker = 0;
        } else
            ticker++;

	}

	public void addResearchPoints(int points){
		this.researchPoints += points;
	}

	//returns index of a paticular piece of Research in the array
	private int getIndexOfUncompletedResearch(Research a){
		double req = a.processingReq;
		for(int i = 0; i<AllUncompleteResearch.Count; i++){
			if(AllUncompleteResearch[i].ID == a.ID){
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

	public void addNewResearch(Research a){
		if (a.isDone ()) {
			AllCompleteResearch.Add (a);
			AllCompleteResearch.Sort ();
		} else {
			AllUncompleteResearch.Add (a);
			AllUncompleteResearch.Sort ();
		}
	}

	public bool hasBeenDone(int id){
		for (int i = 0; i<AllCompleteResearch.Count; i++) {
			if (AllCompleteResearch [i].ID == id) {
				return true;
			}
		}
		return false;
	}

	public bool canBeDone(Research a){
		if(a.processingReq > processingPower){
			return false;
		}
		for (int i = 0; i<a.prerequisites.Length; i++) {
			if(!hasBeenDone(i)){
				return false;
			}
		}
		return true;
	}

	//Only displays the research that can be done
	public List<Research> researchUnderConsideration(){
		List<Research> underConsideration = new List<Research> ();
		for (int i = 0; i<AllUncompleteResearch.Count; i++) {
			if(canBeDone(AllUncompleteResearch[i])){
				underConsideration.Add(AllUncompleteResearch[i]);
				underConsideration.Sort();
			}
			else if(AllUncompleteResearch[i].processingReq > processingPower){
				break;
			}
		}
		return underConsideration;
	}



}
