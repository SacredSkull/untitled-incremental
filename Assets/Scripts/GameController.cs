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

	private int researchPoints = 150;
	private Text score;

    void Awake() {
        score = GameObject.FindWithTag("GUI_Score").GetComponent<Text>();
    }
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        score.text = researchPoints.ToString();
        //Utility.UnityLog(researchPoints);
	}

	public void addResearchPoints(int points){
		this.researchPoints += points;
        //score.text = this.researchPoints.ToString();
	}
}
