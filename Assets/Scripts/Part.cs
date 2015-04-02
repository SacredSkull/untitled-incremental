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
namespace Incremental.XML {
    public partial class Part : Startable, IComparable<Part> {

        private static List<Part> allparts = new List<Part>();
        private static ObjectIDGenerator _objectID = new ObjectIDGenerator();

        public long ID {
            get {
                bool known;
                return _objectID.GetId(this, out known);
            }
        }

        public static Part getPartByID(int ID) {
            List<Part> temp;
            temp = allparts.Where(x => x.ID == ID).ToList();
            if (temp.Count == 1) {
                return temp[0];
            } else if (temp.Count == 0) {
                return null;
            } else {
                Utility.UnityLog(temp[0].name + " has been defined more than once! The first parsed has been used.", LogLevels.ERROR);
                return temp[0];
            }
        }

        public static Part getPartByName(string name) {
            name = name.Replace(" ", "-");
            List<Part> temp;
            temp = allparts.Where(x => x.name == name).ToList();
            if (temp.Count == 1) {
                return temp[0];
            } else if (temp.Count == 0) {
                return null;
            } else {
                Utility.UnityLog(temp[0].name + " has been defined more than once! The first parsed has been used.", LogLevels.ERROR);
                return temp[0];
            }
        }

        public Part(string name, int amount) {
            this.name = name;
            this.number = amount;
        }

        public Part() {
            allparts.Add(this);

            bool known;
            _objectID.GetId(this, out known);
        }

        //public int numberOwned;

        public void buy(int numberToBuy) {
            numberOwned += (short)numberToBuy;
        }

        public void use(int numberToUse) {
            numberOwned -= (short)numberToUse;
        }

        public int CompareTo(Part b) {
            Part a = this;
            if (a.cost == b.cost) {
                return 0;
            } else if (a.cost > b.cost) {
                return 1;
            } else
                return -1;
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

        public override int number {
            get {
                return this.amount;            
            }
            set {
                this.amount = (short)value;
            }
        }

    }
}
