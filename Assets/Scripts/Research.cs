using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class Research : IComparable<Research>{

    private static List<Research> allresearch = new List<Research>();
    private static int instances = 0;

    public static Research getResearchByName(string name) {
        List<Research> temp;
        temp = allresearch.Where(x => x.name == name).ToList();
        if (temp.Count == 1) {
            return temp[0];
        } else if (temp.Count == 0) {
            return null;
        } else {
            Utility.UnityLog(temp[0].name + " has been defined more than once! The first parsed has been used.", LogLevels.ERROR);
            return temp[0];
        }
    }

#warning This code deprecates as soon as the XML code is live.
    #region Deprecated

    public Research(string name, string description, int researchCost, 
	                int processingReq, Research[] prerequisites, bool done){
		this.name = name;
		this.description = description;
		this.researchCost = researchCost;
		this.processingReq = processingReq;
		this.prerequisites = prerequisites;
		this.done = done;
        allresearch.Add(this);
        this.ID = instances;
        instances++;
	}

	public string name;
	public string description;
	public int processingReq;
    public Research[] prerequisites;
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
    #endregion

    public int CompareTo(Research b){
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
/*
public partial class Requirement {

#warning This code deprecates as soon as the XML code is live.
    #region Deprecated
    public string name;
    public int research;
    #endregion

    public Research researchRequirement;

    // These two static members serve as an elegant solution to using a pseudo-unique string identifier (name)
    // It will return one/all requirements with that name, but if it can't find anything it'll return null. If
    // it finds more than one it'll log an error and then use the first parsed value.
    private static List<Requirement> requirements = new List<Requirement>();
    public static Requirement getRequirementByName(string name){
        List<Requirement> temp;
        temp = requirements.Where(x => x.name == name).ToList();
        if (temp.Count == 1) {
            return temp[0];
        } else if (temp.Count == 0) {
            return null;
        } else {
            Utility.UnityLog(temp[0].name+" has been defined more than once! The first parsed has been used.", LogLevels.ERROR);
            return temp[0];
        }
    }

    public Requirement(string name, int researchID) {
        this.name = name;
        requirements.Add(this);
        this.researchRequirement = Research.getResearchById(researchID);
    }
}
*/