using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;

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

public class Part : Startable, IComparable<Part> {
    public int ID { get; set; }
    public string stringID;
    public int cost;
    public int quantity;

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

    /**
        * @fn  public override void complete()
        *
        * @brief   Completes this part(?). Should never have a body!
        *
        * @todo    Evaluate the need for parts to implement Startable- or if Startable should derive a base class that doesn't include these methods.
        * @author  Peter
        * @date    29/03/2015
        */

    public override void complete() {
        throw new NotSupportedException();
    }

    /**
        * @fn  public override void abandon()
        *
        * @brief   Abandons this part(?). Should never have a body!
        *          
        * @todo    Evaluate the need for parts to implement Startable- or if Startable should derive a base class that doesn't include these methods.
        *
        * @author  Peter
        * @date    29/03/2015
        */

    public override void abandon() {
        //TODO: This is required by interface Startable
        throw new NotSupportedException();
    }

    /**
        * @fn  public override void start()
        *
        * @brief   Starts this part(?). Should never have a body!
        *          
        * @todo    Evaluate the need for parts to implement Startable- or if Startable should derive a base class that doesn't include these methods.
        *
        * @author  Peter
        * @date    29/03/2015
        */

    public override void start() {
        throw new NotSupportedException();
    }

    /**
        * @property    public override string name
        *
        * @brief   Gets or sets the name.
        *          
        * @detail  The set automatic property converts spaces to hyphens (-).
        *
        * @return  The name.
        */

    public override string name { get; set; }
}
