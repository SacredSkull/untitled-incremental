using UnityEngine;
using System.Collections;

public class ExitFileBrowser : MonoBehaviour {

	public void onMouseDown(){
		PickerController picker = PickerController.instance;
		picker.showPicker ();
		GUITools.setGameObjectActive (GameController.instance.browser, false);
	}
}
