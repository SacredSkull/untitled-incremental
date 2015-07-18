using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using Incremental.Database;
// This line is needed for the Research GUI StopGap class.
using UnityEngine;

/**
    * @class   Research
    *
    * @brief   Research units are dependencies for projects (and other things).
    *          Research follows a tech tree, and therefore research depends on its parent (or more) research.
    *
    * @author  Conal
    * @author  Peter
    * @date    23/03/2015
    */

public class Research : Startable, IComparable<Research> {
    public int ID { get; set; }
	public string description;
	public int cost;
	public double processReq;
	public bool done;

    /**
        * @property    public List<Startable> Dependencies
        *
        * @brief   Gets the dependencies of this research.
        *          
        * @note    Research only depends on other research.
        *          
        * @detail  Iterates over every dependency for this object and finds the research by it's string name.
        *
        * @return  The list of <Startable> dependencies.
        */

    private List<Research> _Dependencies;

    public List<Research> Dependencies {
        get {
            if (_Dependencies == null) {
                using (DatabaseConnection con = new DatabaseConnection()) {
                    _Dependencies = new List<Research>();
                    const string sql = @"SELECT r.* FROM Research as r 
                                INNER JOIN(
	                                SELECT rJunction.ResearchDependencyID 
	                                FROM Research_Dependencies as rJunction 
	                                WHERE rJunction.ResearchID = @RID
                                ) as rJunction ON rJunction.ResearchDependencyID = r.ID;";
                    _Dependencies = con.connection.Query<Research>(sql, new { RID = ID }).ToList();
                }
            }
            return _Dependencies;
        }
    }

    /**
        * @fn  public override void complete()
        *
        * @brief   Sets this research as complete.
        *
        * @author  Conal
        * @date    15/03/2015
        */

    public override void complete() {
        done = true;
    }

    /**
        * @fn  public override void abandon()
        *
        * @brief   Abandons this research.
        *
        * @author  Peter
        * @date    29/03/2015
        */

    public override void abandon() {
        //TODO: This is required by interface Startable
        throw new NotImplementedException();
    }

    /**
        * @fn  public override void start()
        *
        * @brief   Starts this research.
        *
        * @author  Peter
        * @date    29/03/2015
        */

    public override void start() {
        //TODO: This is required by interface Startable
        throw new NotImplementedException();
    }

    /**
    * @fn  public bool hasBeenDone()
    *
    * @brief   Query if research (this object) has been done.
    *
    * @author  Conal
    * @author  Peter
    * @date    01/04/2015
    * @updated 27/06/2015
    *
    * @return  true if been done, false if not.
    */

    public bool hasBeenDone() {
        return ResearchController.instance.AllCompleteResearch.ContainsKey(this.ID);
    }

    /**
 * @fn  public bool canBeDone()
 *
 * @brief   Determine if Research (this object) can be done.
 *
 * @author  Conal
 * @date    01/04/2015
 *
 * @return  true if it can be done, false if not.
 */

    public bool canBeDone() {
		if (this.processReq > GameController.instance.processingPower) {
			Utility.UnityLog(GameController.instance.processingPower.ToString(),1);
            return false;
        }
        foreach (Research depends in this.Dependencies) {
            if (!depends.hasBeenDone()) {
				return false;
            }
        }
        return true;
    }

    /**
    * @fn  public bool canBeDone(ref List<Research> missingResearch)
    *
    * @brief   Recursive variant of canBeDone() which goes up the entire chain of dependencies, 
    * filling a list that must be met before this can be done. 
    *
    * One particular use in mind for this function (and those like it for Projects, etc.) is that of the hierarchy list, which would high
    * -light the required nodes needed to start this research. 
    * 
    * @author  Peter
    * @date    28/06/2015
    *
    * @param   missingResearch Reference to the List<Research> of requirements not met. As a ref, 
    * this must be instantiated BEFORE passing it into the method.  
    * 
    * @warning This method iterates into each unfilled dependency, and walks out its unfilled dependencies and so on, which obviously
    * results in a longer processing time than the standard canBeDone().
    * 
    * @return  true if there are no dependencies to fullfil, false if requirements are not met. The missingResearch list (empty or not) is always returned.
    */

    public bool canBeDone(ref List<Research> missingResearch) {
        foreach (Research depends in this.Dependencies) {
            if (!depends.hasBeenDone()) {
                missingResearch.Add(depends);
                depends.canBeDone(ref missingResearch);
            }
        }
        return missingResearch.Count == 0;
    }

    /**
        * @fn  public int CompareTo(Research b)
        *
        * @brief   Compares this Research object to another to sort which has the higher processing level.
        *
        * @author  Conal
        * @date    20/03/2015
        *
        * @param   b   Research to compare to this.
        *
        * @return  Assuming that **a is this**, and **b is the other** research object:
        *          Negative if this research is smaller (a < b) , 0 if they are equal (a == b), or positive if
        *          this is greater (a > b).
        */

    public int CompareTo(Research b) {
        Research a = this;
        if (a.processReq < b.processReq) {
            return -1;
        } else if (a.processReq > b.processReq) {
            return 1;
        } else
            return 0;
    }
}
