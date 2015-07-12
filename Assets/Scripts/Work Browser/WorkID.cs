using UnityEngine;
using System.Collections;

public class WorkID : MonoBehaviour {

	public int? ID {
		get;
		set;
	}

	public BrowserListItemClick.ListItemType storedType {
		get;
		set;
	}
}
