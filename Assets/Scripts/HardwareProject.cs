using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper;
using Incremental.Database;
// Required for GUI stopgap class
using UnityEngine;

public class HardwareProject : Project {
    private ICollection<Part> _Parts;
    public ICollection<Part> Parts {
        get {
            if (_Parts == null) {
                using (DatabaseConnection con = new DatabaseConnection()) {
                    const string sql = @"SELECT p.ID ID, p.Name name, p.Cost cost, hwr.Quantity quantity FROM Part as p INNER JOIN( SELECT hwr.PartID, hwr.Quantity FROM HardwareProject_Parts as hwr WHERE hwr.HardwareProjectID = @PID) as hwr ON hwr.PartID = ID;";
                    _Parts = con.connection.Query<Part>(sql, new { PID = ID }).ToList();
                }
            }
            return _Parts;
        }
    }

	//This used to give a list of Parts and research required, no idea why
	//Changing it for now, as it bogs the method down significantly, while
	//the info will come in handy I don't thin it is necessary for this method.
    public bool possible()
    {
        foreach (Research r in this.Research) {
            if (!r.hasBeenDone()) {
				return false;
            }
        }
        GameController game = GameController.instance;
        foreach (Part part in this.Parts)
        {
            bool contains = game.partInventory.ContainsKey(part.ID);
            if (contains) {
                if(game.partInventory[part.ID] < part.quantity) {
					return false;
                }
            }
			else {
				return false;
            }
        }
		return true;
    }
}
