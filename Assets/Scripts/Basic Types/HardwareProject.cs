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
                    _Parts = con.GetHardwareParts(this);
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
				using (DatabaseConnection con = new DatabaseConnection()) {
				    parseable = con.GetTypes(this);
				}
				if(parseable.Any ()){
					foreach (type item in Enum.GetValues(typeof(type))){
						String finalType = RemoveLineEndings(parseable.First().ToString());
						if(item.ToString().Equals(finalType, StringComparison.OrdinalIgnoreCase)){
                            _HardwareType = item;
							return _HardwareType;
						}
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
            if (!GameController.instance.rControl.hasBeenDone(r.ID)) {
				return false;
            }
        }
        GameController game = GameController.instance;
        foreach (Part part in this.Parts)
        {
            bool contains = GameController.instance.pControl.partInventory.ContainsKey(part.ID);
            if (contains||ID != 3) {
				if(GameController.instance.pControl.partInventory[part.ID] < part.quantity) {
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
