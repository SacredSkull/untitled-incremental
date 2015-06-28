using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class SoftwareProject : Project {
    public int pointCost { get; set; }

    public override bool possible(out List<Startable> missingRequirements) {
        missingRequirements = new List<Startable>();
        GameController game = GameController.instance;

        foreach (Research r in this.Research) {
            if (!game.hasBeenDone(r)) {
                missingRequirements.Add(r);
            }
        }
        return missingRequirements.Count == 0;
    }
}
