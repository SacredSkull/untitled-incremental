using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper;
using Incremental.Database;
// Required for GUI stopgap class
using UnityEngine;

public class HardwareProject : Project {
	public enum type
	{
		Computer,
		None,
		NoType
	}

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
    
	public string RemoveLineEndings( string value)
	{
		if(String.IsNullOrEmpty(value))
		{
			return value;
		}
		string lineSeparator = ((char) 0x2028).ToString();
		string paragraphSeparator = ((char)0x2029).ToString();
		
		return value.Replace("\r\n", string.Empty).Replace("\n", string.Empty).Replace("\r", string.Empty).Replace(lineSeparator, string.Empty).Replace(paragraphSeparator, string.Empty);
	}

	private type _HardwareType = type.None;
	public type HardwareType{
		get{
			if(_HardwareType == type.None){
				IEnumerable<String> parseable;
				using (DatabaseConnection conn = new DatabaseConnection()){
					const string sql = @"SELECT t.Name name FROM HardwareType as t INNER JOIN( SELECT hwr.TypeID, hwr.HardwareProjectID FROM HardwareProject_Type as hwr WHERE hwr.HardwareProjectID = @PID)as hwr ON hwr.TypeID = t.ID;";
					parseable = conn.connection.Query<String>(sql, new { PID = ID });
				}
				if(parseable.Any ()){
					foreach (type item in Enum.GetValues(typeof(type))){
						String finalType = RemoveLineEndings(parseable.First().ToString());
						Utility.UnityLog(finalType);
                        Utility.UnityLog((item.ToString() == finalType).ToString());
						if(item.ToString().Equals(finalType, StringComparison.OrdinalIgnoreCase)){
                            Utility.UnityLog("pass");
							_HardwareType = item;
							return _HardwareType;
						}
					}
				}
				foreach (type item in Enum.GetValues(typeof(type))){
					if(item.ToString().Equals(parseable)){
						_HardwareType = item;
						return _HardwareType;
					}
				}
				_HardwareType = type.NoType;
			}
			return _HardwareType;
		}
	}

	//This used to give a list of Parts and research required, no idea why
	//Changing it for now, as it bogs the method down significantly, while
	//the info will come in handy I don't thin it is necessary for this method.
    public bool possible()
    {
        Utility.UnityLog(this.HardwareType.ToString() + "  " + name);
        foreach (Research r in this.Research) {
            if (!r.hasBeenDone()) {
				return false;
            }
        }
        GameController game = GameController.instance;
        foreach (Part part in this.Parts)
        {
            bool contains = game.partInventory.ContainsKey(part.ID);
            if (contains||ID != 3) {
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

	public bool isActive {
		get;
		private set;
	}
}
