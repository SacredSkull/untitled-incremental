using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Incremental.XML {
    public partial class Research : IComparable<Research>, IStartable {

		//public string string_id;
		//public string description;
		//public int cost;
		//public int processingLevel;
		//public Research[] prerequisites;
		//private bool done;
		public int ID;

        private static List<Research> allresearch = new List<Research>();
        private static int instances = 0;

        public static Research getResearchByName(string name) {
            name = name.Replace(" ","_");
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

        public string name {
            get {
                return this.string_id.Replace("-", " ");
            }
        }

        // Though research will never depend on parts, it's just more on par with other classes 
        // to indicate this list is for research (rather than just name the list "dependencies").
        private List<Research> _ResearchDependencies;

        public List<Research> ResearchDependencies {
            get {
                if (_ResearchDependencies == null) {
                    _ResearchDependencies = new List<Research>();
                    foreach (dependency d in this.DependsOn) {
                        if (d.type.Equals("research"))
                            _ResearchDependencies.Add(Research.getResearchByName(d.name));
                    }
                }
                return _ResearchDependencies;
            }
        }

		#if debug
		public Research(int points){
			this.cost = points;
		}

		#endif

        public Research() {
            this.dependsOnField = new List<dependency>();
            allresearch.Add(this);
            this.ID = instances;
            instances++;

            // Default values:
            this.processingLevelField = ((short)(1));
        }

        private bool done;

        public int ID {
            get;
            private set;
        }

        public void complete() {
            done = true;
        }

        public void abandon() {
            //TODO: This is required by interface IStartable
        }

        public void start() {
            //TODO: This is required by interface IStartable
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