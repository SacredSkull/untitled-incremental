using UnityEngine;
using System.Collections;

using UnityEngine.EventSystems;

public class BrowserListItem : MonoBehaviour {

    public enum ListItemType {
        ResearchCategory,
        SoftwareCategory,
        HardwareCategory,
        PartsCategory,
        ItemPicker // This button isn't currently a selector
    }

    // Represents the initial button type
    public ListItemType buttonType = ListItemType.ItemPicker;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnMouseOver(BaseEventData e) {
        
    }

    public void OnMouseExit() {
        
    }

	public void OnMouseDown(){
		PickerController projectPicker = PickerController.instance;
		GameController game = GameController.instance;
		int? ID = this.transform.parent.GetComponentInChildren<WorkID>().ID;
		BrowserListItem.ListItemType storedType = this.transform.parent.GetComponentInChildren<WorkID> ().storedType;
		switch (projectPicker.chapter) {
		    case PickerController.pickedType.Research:
		        if (ID!=null&& GameController.instance.rControl.AllUncompleteResearch.ContainsKey ((int)ID)) {
					GameController.instance.rControl.startResearch (GameController.instance.rControl.AllUncompleteResearch [(int)ID]);
		        }
		        break;
			case PickerController.pickedType.Software:
				if (ID!=null&& GameController.instance.sControl.UnstartedSoftware.ContainsKey ((int)ID)) {
					GameController.instance.sControl.startSoftware (GameController.instance.sControl.UnstartedSoftware [(int)ID]);
		        }
		        break;
			case PickerController.pickedType.Hardware:
				if (ID!=null&& GameController.instance.hControl.UnstartedHardware.ContainsKey ((int)ID)) {
					GameController.instance.hControl.makeHardware (GameController.instance.hControl.UnstartedHardware [(int)ID]);
		        }
		        break;
		    case PickerController.pickedType.Parts:
				if (ID!=null&& GameController.instance.pControl.allBuyableParts.ContainsKey ((int)ID)) {
					GameController.instance.pControl.buyPart (GameController.instance.pControl.allBuyableParts [(int)ID],1);
		        }
		        break;
		    default:
		        switch (storedType) {
					case ListItemType.ResearchCategory:
						projectPicker.setChapterToResearch();
		                break;
					case ListItemType.HardwareCategory:
						projectPicker.setChapterToHardware();
		                break;
					case ListItemType.SoftwareCategory:
						projectPicker.setChapterToSoftware();
		                break;
					case ListItemType.PartsCategory:
						projectPicker.setChapterToParts();
		                break;
		        }
		        break;
		}
	}
}
