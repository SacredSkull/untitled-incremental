using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//A part can be bought through the computer to make pc components etc
//For now lets just say all parts are available from the get go, but some will obviously 
//be a little too pricey.

//idea for the future, markets which must be unlocked to get access to parts.
namespace Incremental.XML {
    public partial class Part : IComparable<Part>, IStartable {

        public int numberOwned;

        public void buy(int numberToBuy) {
            numberOwned += numberToBuy;
        }

        public void use(int numberToUse) {
            numberOwned -= numberToUse;
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
