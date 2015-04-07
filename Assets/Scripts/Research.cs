﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;

namespace Incremental.XML {

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

    public partial class Research : Startable, IComparable<Research> {

		//public string string_id;
		//public string description;
		//public int cost;
		//public int processingLevel;
		//public Research[] prerequisites;
		//private bool done;

        [NonSerialized]
        private static List<Research> allresearch = new List<Research>();
        private static ObjectIDGenerator _objectID = new ObjectIDGenerator();

        /**
         * @fn  public static Research getResearchByID(int ID)
         *
         * @brief   Gets research by a unique identifier generated by _objectID.
         *
         * @detail  Uses the static member allresearch to search for research, which is filled in the Research() constructor.
         *          If there is more than one piece of Research with this ID a warning is logged, and the first found Research
         *          is returned.         
         * 
         * @author  Peter
         * @date    26/03/2015
         *
         * @param   ID  The identifier.
         *
         * @return  The research identified or null, if no research by that ID could be found.
         */

        public static Research getResearchByName(string name) {
            name = name.Replace(" ","-");
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

        /**
         * @fn  public static Research getResearchByName(string name)
         *
         * @brief   Gets Research by its name.
         *          
         * @attention   This function converts spaces from the parameter into hyphens (-) - *but nothing else* - and then uses the result as the search string. 
         *              If there is more than one Research with this name, a warning is logged, and the first found Research
         *              is returned.
         *
         * @author  Peter
         * @date    26/03/2015
         *
         * @param   name    The name.
         *
         * @return  The research, or null, if none could be found.
         */

        public static Research getResearchByID(int ID){
            List<Research> temp;
            temp = allresearch.Where(x => x.ID == ID).ToList();
            if (temp.Count == 1) {
                return temp[0];
            } else if (temp.Count == 0) {
                return null;
            } else {
                Utility.UnityLog(temp[0].name + " has been defined more than once! The first parsed has been used.", LogLevels.ERROR);
                return temp[0];
            }
        }

        public override string name {
            get {
                return this.string_id;
            }
            set {
                this.string_id = value.Replace(" ", "-");
            }
        }

        public override int number
        {
            get
            {
                return 0;
            }
            set
            {
                this.number = 0;
            }
        }

        // Research will never contain parts
        [NonSerialized]
        private List<Startable> _Dependencies = null;

        /**
         * @property    public List<Startable> Dependencies
         *
         * @brief   Gets the dependencies of this research.
         *          
         * @note    Research only depends on other research.
         *          
         * @attention   This is a lazy load property, meaning that the first time it is accessed its value is parsed.
         *          
         * @detail  Iterates over every dependency for this object and finds the research by it's string name.
         *
         * @return  The list of <Startable> dependencies.
         */

        public List<Startable> Dependencies {
            get {
                if (_Dependencies == null && this.string_id != null && this.string_id.Length > 0) {
                    _Dependencies = new List<Startable>();
                    foreach (dependency d in this.DependsOn) {
                        if (d.type.Equals("Research"))
                            _Dependencies.Add(Research.getResearchByName(d.string_id));
                    }
                }
                return _Dependencies;
            }
        }


		public Research(int points){
			this.cost = (short)points;
		}

        /**
         * @fn  public Research()
         *
         * @brief   Default constructor.
         *          
         * @detail  Adds itself to the static list of all created research, then generates a unique ID for this object.
         *
         * @author  Peter
         * @date    26/03/2015
         */

        public Research() {
            allresearch.Add(this);

            bool known;
            _objectID.GetId(this, out known);

            // Default values:
            this.processingLevelField = ((short)(1));
        }

        private bool done;
        public long ID {
            get {
                bool known;
                return _objectID.GetId(this, out known);
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
         * @fn  public List<Startable> getDependencies()
         *
         * @brief   Gets the dependencies.
         *          
         * @deprecated  This is nothing but a shell of Dependencies and is quite pointless right now.
         *
         * @author  Conal
         * @date    29/03/2015
         *
         * @return  The dependencies.
         */

        public List<Startable> getDependencies() {
            return this.Dependencies;
        }

        /**
         * @fn  public bool isDone()
         *
         * @brief   Query if this research is done.
         *          
         * @deprecated  I (Peter) would argue that it is pointless have a function for this, 
         *              we already have a property that can be easily returned.
         *              ```
         *              if(r.done == true) ...
         *              ```
         *
         * @author  Conal
         * @date    18/03/2015
         *
         * @return  true if done, false if not.
         */

        public bool isDone() {
            return done;
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
            if (a.processingLevel < b.processingLevel) {
                return -1;
            } else if (a.processingLevel > b.processingLevel) {
                return 1;
            } else
                return 0;
        }

        
    }
}
