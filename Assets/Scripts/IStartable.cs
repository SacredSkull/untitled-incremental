using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

interface IStartable {
    void complete();
    void abandon();

    //REVIEW: Is this necessary?
    void start();
}

