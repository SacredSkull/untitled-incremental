using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Incremental.XML {
    public abstract partial class Project : IStartable {
		//public string name;
		//public string description;
		//public int pointsPerSecond;
		//public int moneyPerSecond;
		//public int pointCost;
        public Research[] researchIDsRequired;


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
            for (int i = 0; i < researchIDsRequired.Length; i++) {
                if (!game.hasBeenDone(researchIDsRequired[i].string_id)) {
                    return false;
                }
            }
            return true;
        }

        public Project() {
            this.dependsOnField = new List<dependency>();
            this.usesField = ((sbyte)(-1));
        }

        public void complete() {
            //TODO: This is required by interface IStartable
	    }

        public void abandon() {
            //TODO: This is required by interface IStartable
        }

        public void start() {
            //TODO: This is required by interface IStartable
        }
    }
}
