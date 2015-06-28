using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using System;

using Incremental.Database;

/**
    * @class   Project
    *
    * @brief   Projects require Research and/or Part (s) to complete them. 
    *          Some can be only completed a definite number of times; others indefinitely.
    *
    * @author  Conal
    * @author  Peter
    * @date    16/03/2015
    * @updated 25/06/2015
    */

public abstract class Project : Startable {
    //public string name;
    //public int pointsPerTick;
    //public int pointsPerClick;
    //public int moneyPerTick;
    //public int processIncrease;
    //public int moneyMult;
    //public int pointMult;
    //public int oneTimeFees; (not currrently in use)
    //public int upkeepCost(will not be used);

    //public List<Part> requiredParts = new List<Part>();
    public int ID { get; set; }
    public string stringID { get; set; }
    public string description { get; set; }
    public int upkeep { get; set; }
    public int uses { get; set; }
    public int pointsPerTick { get; set; }
    public int pointsPerClick { get; set; }
    public int pointMult { get; set; }
    public double processIncrease { get; set; }
    public int moneyPerTick { get; set; }
    public int moneyMult { get; set; }
    public ICollection<Research> _Research; 
    public ICollection<Research> Research {
        get {
            if (_Research == null) {
                using (DatabaseConnection con = new DatabaseConnection()) {
                    const string sql = @"SELECT r.* FROM Research as r INNER JOIN( SELECT hwr.ResearchID FROM HardwareProject_Research as hwr WHERE hwr.HardwareProjectID = @PID) as hwr ON hwr.ResearchID = ID;";
                    _Research = con.connection.Query<Research>(sql, new { PID = ID }).ToList();
                }
            }
            return _Research;
        }
    }

    /**
        * @fn  public Project()
        *
        * @brief   Default constructor.
        *          
        * @details Initialises the dependsOnField(?) 
        *          
        * @todo    Static methods getProjectByID & getProjectByName are missing. 
        *          Additionally, so is the static list holding all Projects.
        *          When complete, this constructor will add **this** to the static list of all Projects.
        *          
        * 
        *
        * @author  Peter
        * @date    23/03/2015
        */

    /**
        * @property    public bool canDoMultiple
        *
        * @brief   Gets a bool indicating whether we can do multiple or not.
        *          
        * @deprecated  This is convuluted. A missingRequirements() property or method 
        *              should indicate whether or not it you have used up all uses.
        *
        * @return  true if we can do multiple, false if not.
        */

    public bool canDoMultiple {
        get {
            return this.uses == -1;
        }
    }

    public override void complete() {
        //TODO: This is required by interface Startable
        throw new NotImplementedException();
	}

    public override void abandon() {
        //TODO: This is required by interface Startable
        throw new NotImplementedException();
    }

    public override void start() {
        //TODO: This is required by interface Startable
        throw new NotImplementedException();
    }

    public override string name {
        get {
            return this.stringID;
        }
        set {
            this.stringID = value;
        }
    }

    /**
        * @property    public override int number
        *                     
        * @todo        Is the set parameter correct? Should it not be `this.number = value`?
        */

    public override int number {
        get {
            return 0;
        }
        set {
            this.number = 0;
        }
    }

    /**
        * @fn  public bool possible(out List<Startable> missingRequirements)
        *
        * @brief   Computes if this Project is possible, and spits out the missing Startable requirements.
        *
        * @author  Peter
        * @author  Conal
        * @date    27/03/2015
        *
        * @param [out] missingRequirements The missing requirements, with quantity.
        *
        * @return  True if all requirements are satisfied (and returns an empty missingRequirements).
        *          False if there are missing requirements and spits out a dictionary of the missing Startable requirements, 
        *          including quantity for parts.
        */
    public abstract bool possible(out List<Startable> missingRequirements);
}
