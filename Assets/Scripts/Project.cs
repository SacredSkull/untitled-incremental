using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Incremental.XML {
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

		public bool canDoMultiple{
			get{
				if(this.uses == -1){
					return true;
				}
				else return false;
			}
		}

        public Project() {
            this.dependsOnField = new List<dependency>();
            this.usesField = ((sbyte)(-1));
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

        [NonSerialized]
        private List<Startable> _Dependencies = null;
        private List<Startable> Dependencies {
            get {
                if (_Dependencies == null && this.string_id != null && this.string_id.Length > 0) {
                    _Dependencies = new List<Startable>();
                    Part temp = new Part();
                    foreach (dependency d in this.DependsOn) {
                        if (d.type.Equals("Research"))
                            _Dependencies.Add(Research.getResearchByName(d.string_id));
                        if (d.type.Equals("Part"))
                             temp = new Part(d.string_id, d.amount);
                            _Dependencies.Add(temp);
                    }
                }
                return _Dependencies;
            }
        }

        public Dictionary<Part,int> partDependencies {
            get {
                List<Startable> temp = Dependencies;
                Dictionary<Part,int> parts = new Dictionary<Part,int>();
                foreach (Startable s in temp) {
                    if (s.GetType().Equals("Part")) { 
                        parts.Add(Part.getPartByName(s.name), s.number);
                    }
                }
                return parts;
            }
        }

        public List<Research> researchDependencies {
            get {
                List<Startable> temp = Dependencies;
                List<Research> research = new List<Research>();
                foreach (Startable s in temp) {
                    if (s.GetType().Equals("Research")) { 
                        research.Add(Research.getResearchByName(s.name));
                    }
                }
                return research;
            }
        }

        public bool canDo()
        {
            GameController game = GameController.instance;
            List<Research> research = researchDependencies;
            foreach (var thing in research) {
                if (!game.hasBeenDone(thing.name)) {
                    return false;
                }
            }
            if (this.type.Equals("Part")) {
                Dictionary<Part, int> temp = partDependencies;
                foreach (var item in temp)
                {
                    if (!game.hasParts(item.Key.name, item.Value))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
