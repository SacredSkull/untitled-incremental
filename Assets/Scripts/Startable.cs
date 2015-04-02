using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public abstract class Startable {
    public abstract void complete();
    public abstract void abandon();

    //REVIEW: Is this method necessary?
    public abstract void start();

    public abstract string name {
        get;
        set;
    }

    public abstract int number
    {
        get;
        set;
    }
}