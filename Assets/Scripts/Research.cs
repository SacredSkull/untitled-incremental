using UnityEngine;
using System.Collections;

public class Research : MonoBehaviour {

	public string name;
	public string description;
	public int processingReq;
	public int ID;
	public int[] prerequisites;
	private bool done;

    public int researchCost {
        get;
        private set;
    }

	public void complete(){
		done = true;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
