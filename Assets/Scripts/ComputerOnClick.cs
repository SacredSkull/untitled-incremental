using UnityEngine;
using System.Collections;

public class ComputerOnClick : MonoBehaviour {
	public int researchPerClick = 10;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnMouseDown(){
        GameController.instance.addResearchPoints(researchPerClick);
	}
}
