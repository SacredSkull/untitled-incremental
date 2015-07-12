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
        Utility.UnityLog("giggle" + this.name);
    }

    public void OnMouseExit() {
        
    }

	public void OnMouseDown(){
		GameController game = GameController.instance;
		int? ID = this.transform.parent.GetComponentInChildren<WorkID>().ID;
		switch (game.chapter) {
		    case GameController.pickedType.Research:
		        if (ID!=null&& game.AllUncompleteResearch.ContainsKey ((int)ID)) {
		            game.startResearch (game.AllUncompleteResearch [(int)ID]);
		        }
		        break;
		    case GameController.pickedType.Software:
		        if (ID!=null&& game.UnstartedSoftware.ContainsKey ((int)ID)) {
		            game.startSoftware (game.UnstartedSoftware [(int)ID]);
		        }
		        break;
		    case GameController.pickedType.Hardware:
		        if (ID!=null&& game.UnstartedHardware.ContainsKey ((int)ID)) {
		            game.makeHardware (game.UnstartedHardware [(int)ID]);
		        }
		        break;
		    case GameController.pickedType.Parts:
		        if (ID!=null&& game.allBuyableParts.ContainsKey ((int)ID)) {
		            game.buyPart (game.allBuyableParts [(int)ID],1);
		        }
		        break;
		    default:
		        switch (buttonType) {
		            case ListItemType.ResearchCategory:
                        game.setChapterToResearch();
		                break;
                    case ListItemType.HardwareCategory:
                        game.setChapterToHardware();
		                break;
                    case ListItemType.SoftwareCategory:
                        game.setChapterToSoftware();
		                break;
                    case ListItemType.PartsCategory:
                        game.setChapterToParts();
		                break;
		        }
		        break;
		}
	}
}
