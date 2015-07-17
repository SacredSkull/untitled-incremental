using UnityEngine;
using System.Collections;

using UnityEngine.EventSystems;

public class BrowserListItemClick : MonoBehaviour {

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
		ResearchController rControl = ResearchController.instance;
		SoftwareController sControl = SoftwareController.instance;
		HardwareController hControl = HardwareController.instance;
		PartController pControl = PartController.instance;
		int? ID = this.transform.parent.GetComponentInChildren<WorkID>().ID;
		BrowserListItemClick.ListItemType storedType = this.transform.parent.GetComponentInChildren<WorkID> ().storedType;
		switch (projectPicker.chapter) {
		    case PickerController.pickedType.Research:
		        if (ID!=null&& rControl.AllUncompleteResearch.ContainsKey ((int)ID)) {
		            rControl.startResearch (rControl.AllUncompleteResearch [(int)ID]);
		        }
		        break;
			case PickerController.pickedType.Software:
		        if (ID!=null&& sControl.UnstartedSoftware.ContainsKey ((int)ID)) {
		            sControl.startSoftware (sControl.UnstartedSoftware [(int)ID]);
		        }
		        break;
			case PickerController.pickedType.Hardware:
		        if (ID!=null&& hControl.UnstartedHardware.ContainsKey ((int)ID)) {
		            hControl.makeHardware (hControl.UnstartedHardware [(int)ID]);
		        }
		        break;
		    case PickerController.pickedType.Parts:
		        if (ID!=null&& pControl.allBuyableParts.ContainsKey ((int)ID)) {
		            pControl.buyPart (pControl.allBuyableParts [(int)ID],1);
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
