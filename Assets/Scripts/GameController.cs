using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

	private int researchPoints;
    private Text score;
    public int researchPerClick = 10;


	// Use this for initialization
	void Start () {
        GameObject scoreObject = GameObject.Find("Score");
		researchPoints = 0;

        score = scoreObject.GetComponent<Text>();

        score.text = researchPoints.ToString();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonUp(0)) {
            Utility.UnityLog(researchPerClick + " points added");
            addResearchPoints(researchPerClick);
        }
		score.text = researchPoints.ToString();
	}

    void OnMouseUp() {
        
    }

	public void addResearchPoints(int points){
		researchPoints += points;
	}
}
