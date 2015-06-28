using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper;
using Incremental.Database;

public class HardwareProject : Project {
    private ICollection<Part> _Parts;
    public ICollection<Part> Parts {
        get {
            if (_Parts == null) {
                using (DatabaseConnection con = new DatabaseConnection()) {
                    const string sql = @"SELECT p.ID ID, p.Name stringID, p.Cost cost, hwr.Quantity quantity FROM Part as p INNER JOIN( SELECT hwr.PartID, hwr.Quantity FROM HardwareProject_Parts as hwr WHERE hwr.HardwareProjectID = @PID) as hwr ON hwr.PartID = ID;";
                    _Parts = con.connection.Query<Part>(sql, new { PID = ID }).ToList();
                }
            }
            return _Parts;
        }
    }

    public override bool possible(out List<Startable> missingRequirements)
    {
        missingRequirements = new List<Startable>();
        GameController game = GameController.instance;
    
        foreach (Research r in this.Research) {
            if (!game.hasBeenDone(r)) {
                missingRequirements.Add(r);
            }
        }
    
        foreach (Part part in this.Parts)
        {
            bool contains = game.partInventory.ContainsKey(part.ID);
            if (contains) {
                // If the inventory of parts contains 
                if(game.partInventory[part.ID] < part.quantity) {
                    part.quantity -= game.partInventory[part.ID];
                    if (part.quantity < 0)
                        part.quantity = 0;
                    missingRequirements.Add(part);
                }
            } else {
                missingRequirements.Add(part);
            }
        }
    
        return missingRequirements.Count == 0;
    }
}
