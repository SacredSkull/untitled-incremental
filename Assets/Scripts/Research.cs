using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;

namespace Incremental.XML {
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
        public List<Startable> Dependencies {
            get {
                Utility.UnityLog("Hi! Dependencies");
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

        public override void complete() {
            done = true;
        }

        public override void abandon() {
            //TODO: This is required by interface Startable
            throw new NotImplementedException();
        }

        public override void start() {
            //TODO: This is required by interface Startable
            throw new NotImplementedException();
        }

        public List<Startable> getDependencies() {
            return this.Dependencies;
        }

        public bool isDone() {
            return done;
        }
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
