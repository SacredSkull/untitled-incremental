using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using Dapper;
using Incremental.Database;

//A part can be bought through the computer to make pc components etc
//For now lets just say all parts are available from the get go, but some will obviously 
//be a little too pricey.

//idea for the future, markets which must be unlocked to get access to parts.

/**
    * @class   Part
    *
    * @brief   Contains definitions for Parts.
    *          
    * @detail  Parts are requirements for hardware projects. Most will be available to begin with, 
    *          but some may require research or large prices.
    *          
    * @attention      Attainable market tiers will probably be added in place of requiring research to build parts, most likely Market levels being
    *                 research anyway.
    *
    * @author  Conal
    * @author  Peter         
    * 
    * @date    23/03/2015
    */

public class Part : Asset, IComparable<Part> {
    public int cost { get; set; }
    public int quantity { get; set; }
    public override string name { get; set; }

	private ICollection<Research> _Research;
	public ICollection<Research> Research {
		get {
			if (_Research == null) {
				using (DatabaseConnection con = new DatabaseConnection()) {
				    _Research = con.GetPartResearch(this);
				}
			}
			return _Research;
		}
	}

	/**
        * @fn  public Part(string name, int amount)
        *
        * @brief   Direct creation/debugging constructor.
        *
        * @author  Peter
        * @date    25/03/2015
        *
        * @param   name    The name of the part.
        * @param   amount  The amount.
        */

    public Part(string name, int amount, int cost) {
        this.name = name;
        this.cost = cost;
        this.quantity = amount;
    }

    /**
        * @fn  public Part()
        *
        * @brief   Default constructor.
        *          
        * @details A new part will add its self to the static parts list and then create a unique ID that can be used to access it with.
        *
        * @author  Peter
        * @date    25/03/2015
        */

    public Part() {
    }

    /**
        * @fn  public int CompareTo(Part b)
        *
        * @brief   Compares this Part (a) object to another (b) to determine their difference.
        *
        * @author  Conal
        * @date    27/03/2015
        *
        * @param   b   Part to compare to this.
        *
        * @return  Negative if this part's cost is less than the other (a < b), 0 if they are equal, or positive if
        *          this part is more expensive than the other (a > b).
        */

    public int CompareTo(Part b) {
        Part a = this;
        if (a.cost == b.cost) {
            return 0;
        } else if (a.cost > b.cost) {
            return 1;
        } else
            return -1;
    }

	public bool isBuyable()
	{
		foreach (Research r in this.Research) {
			if (!r.hasBeenDone()) {
				return false;
			}
		}
		return true;
	}
}
