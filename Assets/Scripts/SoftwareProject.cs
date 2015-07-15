using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper;
using Incremental.Database;
using UnityEngine;

public class SoftwareProject : Project {
    public int pointCost { get; set; }

    public bool possible(out List<Startable> missingRequirements) {
		missingRequirements = new List<Startable>();
        foreach (Research r in this.Research) {
            if (!r.hasBeenDone()) {
                missingRequirements.Add(r);
            }
        }
        return missingRequirements.Count == 0 && this.uses != 0;
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

	public enum type
	{
		OS,
		Course,
		Software,
		None,
		NoType
	}

	public type _SoftwareType = type.None;
	
	public type SoftwareType{
		get{
			if(_SoftwareType == type.None){
				IEnumerable<String> parseable;
				using (DatabaseConnection conn = new DatabaseConnection()){
					const string sql = @"SELECT t.Name name FROM SoftwareType as t INNER JOIN( SELECT swr.TypeID, swr.SoftwareProjectID FROM SoftwareProject_Type as swr WHERE swr.SoftwareProjectID = @PID)as swr ON swr.TypeID = t.ID;";
					parseable = conn.connection.Query<String>(sql, new { PID = ID });
				}
				if(parseable.Any ()){
					foreach (type item in Enum.GetValues(typeof(type))){
						String finalType = RemoveLineEndings(parseable.First().ToString());
						if(item.ToString().Equals(finalType, StringComparison.OrdinalIgnoreCase)){
							_SoftwareType = item;
							return _SoftwareType;
						}
					}
				}
				foreach (type item in Enum.GetValues(typeof(type))){
					if(item.ToString().Equals(parseable)){
						_SoftwareType = item;
						return _SoftwareType;
					}
				}
				_SoftwareType = type.NoType;
			}
			return _SoftwareType;
		}
	}
}
