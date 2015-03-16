using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

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

	private double processingPower = 1.23;
	private Text score;
	private bool researchSet;
	public List<Research> AllUncompleteResearch = new List<Research>();
	public List<Research> AllCompleteResearch = new List<Research>();
	//May need to creat another list ordered by ID to make finding completed
	//research more effiecient. For now though, it is not necessary.
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

    void Awake() {
        score = GameObject.FindWithTag("GUI_Score").GetComponent<Text>();
        researchPoints = 0;
    }
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        score.text = researchPoints.ToString();
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
