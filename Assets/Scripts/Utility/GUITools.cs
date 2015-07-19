using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

//class for general methods, that can be applied to all aspects of the GUI
public static class GUITools  {

	public static void setButtonVisible(String name,bool visible){
		GameObject button = GameObject.Find(name);
		if (!visible) {
			button.GetComponent<CanvasGroup>().alpha = 0;
			button.GetComponent<Button>().interactable = false;
			button.GetComponent<CanvasGroup>().interactable = false;
		} else {
			button.GetComponent<CanvasGroup>().alpha = 1;
			button.GetComponent<Button>().interactable = true;
			button.GetComponent<CanvasGroup>().interactable = true;
		}
	}

	//warning, only use this method on stored gameObjects, if a Gameobject is 
	//deactivated simply by finding it, it cannot be found again to reactivate, store the GameObject in the 
	//GameController class.
	public static void setGameObjectActive(GameObject g, bool active) {
	    g.SetActive(active);
	}
}
