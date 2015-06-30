using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class SoftwareProject : Project {
    public int pointCost { get; set; }

    public override bool possible(out List<Startable> missingRequirements) {
        missingRequirements = new List<Startable>();
        foreach (Research r in this.Research) {
            if (!r.hasBeenDone()) {
                missingRequirements.Add(r);
            }
        }
        return missingRequirements.Count == 0;
    }
}
