using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

	private int researchPoints;
	public Text score;


	// Use this for initialization
	void Start () {
		researchPoints = 0;
		score.text = researchPoints.ToString();
	}
	
	// Update is called once per frame
	void Update () {
		score.text = researchPoints.ToString();
	}

	public void addResearchPoints(int points){
		researchPoints += points;
	}
}
