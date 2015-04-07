using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Incremental.XML {

    /**
     * @class   Project
     *
     * @brief   Projects require Research and/or Part(s) to complete them. Some can be only created once or twice; others indefinitely.
     *
     * @author  Conal
     * @date    16/03/2015
     */

    public partial class Project : Startable {
        //public string name;
        //public string description;
        //public int pointsPerSecond;
        //public int pointsPerClick;
        //public int moneyPerSecond;
        //public int processIncrease;
        //public int moneyMult;
        //public int pointMult;
        //public int oneTimeFees; (not currrently in use)
        //public int upkeepCost(will not be used);
        //public int pointCost;
        //public int uses;
        //public List<Part> requiredParts = new List<Part>();

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

		public bool canDoMultiple{
			get{
				if(this.uses == -1){
					return true;
				}
				else return false;
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

        public Project() {
            this.dependsOnField = new List<dependency>();
            Dependencies.All(x => x.number != null);
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
                return this.string_id;
            }
            set {
                this.string_id = value.Replace(" ", "-");
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
         * @property    public List<Startable> Dependencies
         *
         * @brief   Gets the dependencies of this project.
         *          
         * @note    A software Project only requires Research.
         *          A hardware Project requires Part(s) and optionally, Research.
         *          
         * @attention   This is a lazy load property, meaning that the first time it is accessed its value is parsed.
         *              However, the constructor of this class iterates (in the most minimal/performant way possible) over it,
         *              thus forcing the loading of it.
         *          
         * @detail  Iterates over every dependency for this object and finds the research by it's string name.
         *
         * @return  The list of <Startable> dependencies.
         */

        [NonSerialized]
        private List<Startable> _Dependencies = null;
        public List<Startable> Dependencies {
            get {
                if (_Dependencies == null && this.string_id != null && this.string_id.Length > 0) {
                    _Dependencies = new List<Startable>();
                    foreach (dependency d in this.DependsOn) {
                        if (d.type.Equals("Research")){
                            Research r = Research.getResearchByName(d.string_id);
                            _Dependencies.Add(r);
                            researchDependencies.Add(r);
                        }
                        if (d.type.Equals("Part")) {
                            Part p = Part.getPartByName(d.string_id);
                            partDependencies.Add(p.ID, d.amount);
                            _Dependencies.Add(p);
                        }
                    }
                }
                return _Dependencies;
            }
        }

        /**
         * @property    public Dictionary<int, int> partDependencies
         *
         * @brief   Gets or sets the part requirements.
         *
         * @return  The part requirements.
         */

        private Dictionary<long, int> partDependencies {
            get;
        }

        /**
         * @property    public List<Research> researchDependencies
         *
         * @brief   Selects all Research types from the Depencies list.
         *
         * @return  The research dependencies.
         */

        public List<Research> researchDependencies {
            get;
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
         * @param [out] missingRequirements The missing requirements.
         *
         * @return  True if all requirements are satisfied (and returns an empty missingRequirements).
         *          False if there are missing requirements and spits out a list of the missing Startable requirements.
         */

        public bool possible(out List<Startable> missingRequirements)
        {
            GameController game = GameController.instance;
            missingRequirements = new List<Startable>();
            List<Research> research = researchDependencies;

            foreach (Research r in research) {
                if (!game.hasBeenDone(r.name)) {
                    missingRequirements.Add(r);
                }
            }

            foreach (KeyValuePair<long, int>pID in this.partDependencies)
            {
                bool contains = game.partInventory.ContainsKey(pID.Key);
                if (contains) {
                    if(game.partInventory[pID.Key] < pID.Value){
                        missingRequirements.Add(Part.getPartByID((int)pID.Key));
                    }
                } else {
                    missingRequirements.Add(Part.getPartByID((int)pID.Key));
                }
            }

            if (missingRequirements.Count != 0) {
                return false;
            }

            return true;
        }
    }
}
