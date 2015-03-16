using UnityEngine;
using System.Collections;
using UnityEngine.UI;

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

	public void startResearch(Research research){
		//TODO: Change this once Data structure has been implemented
		researchSet = true;
		currentResearch = research;
	}

	public void stopResearch(){
		researchSet = false;
		//TODO: This will not work, and the data structure for 
		// research will need to be created before it can work.
		//The fact that research has been completed will not be stored.
		currentResearch.complete ();
		currentResearch = null;
	}
}
