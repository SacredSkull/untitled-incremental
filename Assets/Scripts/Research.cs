using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Research : MonoBehaviour, IComparable<Research>{

	public string name;
	public string description;
	public int processingReq;
	public int[] prerequisites;
	private bool done;

	public int ID {
		get;
		private set;
	}

    public int researchCost {
        get;
        private set;
    }

	public void complete(){
		done = true;
	}

	public bool isDone(){
		return done;
	}

	public int compareTo(Research b){
		Research a = this;
		if (a.processingReq < b.processingReq) {
			return -1;
		} else if (a.processingReq  > b.processingReq) {
			return 1;
		} else
			return 0;
	}



	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
