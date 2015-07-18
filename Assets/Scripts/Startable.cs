using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/**
 * @class   Startable
 *
 * @brief   This is the base class for anything that is startable, completeable or abandonable.
 *
 * @author  Peter
 * @date    20/03/2015
 * @updated 19/07/2015
 */

public abstract class Startable : Asset {
    public abstract void complete();
    public abstract void abandon();

    // \todo Is this method necessary?
    public abstract void start();
}