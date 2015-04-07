using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/**
 * @class   Startable
 *
 * @brief   This class is to be inherited by anything that is 'missingRequirements' - Research or projects, for example.
 *
 * @author  Peter
 * @date    20/03/2015
 */

public abstract class Startable {
    public abstract void complete();
    public abstract void abandon();

    // \todo Is this method necessary?
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