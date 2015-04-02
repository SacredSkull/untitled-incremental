using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Incremental.XML {
    public partial class Project : Startable {
		//public string name;
		//public string description;
		//public int pointsPerSecond;
		public int moneyperSecond;
		//public int pointCost;
        //public Research[] researchIDsRequired;

		public bool canDoMultiple{
			get{
				if(this.uses == -1){
					return true;
				}
				else return false;
			}
		}

        public virtual bool canDo() {
            GameController game = GameController.instance;
            //for (int i = 0; i < researchIDsRequired.Length; i++) {
                //if (!game.hasBeenDone(researchIDsRequired[i].string_id)) {
                //    return false;
                //}
            //}
            return true;
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
        public List<Startable> Dependencies {
            get {
                if (_Dependencies == null && this.string_id != null && this.string_id.Length > 0) {
                    _Dependencies = new List<Startable>();
                    foreach (dependency d in this.DependsOn) {
                        if (d.type.Equals("Research"))
                            _Dependencies.Add(Research.getResearchByName(d.string_id));
                        if (d.type.Equals("Part"))
                            _Dependencies.Add(Part.getPartByName(d.string_id));
                    }
                }
                return _Dependencies;
            }
        }
    }
}
