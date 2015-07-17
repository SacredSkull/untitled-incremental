using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Incremental.Database;

using UnityEngine.EventSystems;

public class PickerController : MonoBehaviour {

	private static PickerController _instance;

	public static PickerController instance {
		get {
			if (_instance == null)
				_instance = GameObject.FindObjectOfType<PickerController>();
			return _instance;
		}
	}


	private int BUTTON_COUNT = 5;
	public int chapterPage = 0;
	//Have you changed page in the last 15 frames?
	public bool flicked = false;

	public enum pickedType
	{
		None,
		Research,
		Software,
		Hardware,
		Parts
		
	}
	public pickedType chapter;



	public void setChapterToNone(){
		foreach(Text field in GameController.instance.outProject){
			field.text = null;
			field.GetComponent<WorkID>().ID = null;
		}
		chapter = pickedType.None;
		chapterPage = 0;
		GUITools.setButtonVisible ("Index", false);
		GameController.instance.outProject[0].text = "Research";
		GameController.instance.outProject [0].GetComponent<WorkID> ().storedType = BrowserListItem.ListItemType.ResearchCategory;
		GameController.instance.outProject[1].text = "Software";
		GameController.instance.outProject [1].GetComponent<WorkID> ().storedType = BrowserListItem.ListItemType.SoftwareCategory;
		GameController.instance.outProject[2].text = "Hardware";
		GameController.instance.outProject [2].GetComponent<WorkID> ().storedType = BrowserListItem.ListItemType.HardwareCategory;
		GameController.instance.outProject[3].text = "Parts";
		GameController.instance.outProject [3].GetComponent<WorkID> ().storedType = BrowserListItem.ListItemType.PartsCategory;
		if (!SoftwareController.instance.PossibleSoftware.Any()) {
			GUITools.setButtonVisible ("R2", false);
		} else {
			GUITools.setButtonVisible ("R2", true);
		}
		if (!HardwareController.instance.PossibleHardware.Any ()) {
			GUITools.setButtonVisible ("R3", false);
			GUITools.setButtonVisible ("R4", false);
		} else {
			GUITools.setButtonVisible ("R3", true);
			GUITools.setButtonVisible ("R4", true);
		}
		GUITools.setButtonVisible ("R5", false);
		GUITools.setButtonVisible ("Next", false);
		GUITools.setButtonVisible ("Previous", false);
		
	}
	
	public void setChapterToResearch(){
		chapter = pickedType.Research;
		GUITools.setButtonVisible ("R2", true);
		GUITools.setButtonVisible ("R3", true);
		GUITools.setButtonVisible ("R4", true);
		GUITools.setButtonVisible ("R5", true);
		List<Research> temp = new List<Research> ();
		if (ResearchController.instance.AllPossibleResearchByKey != null) {
			temp = ResearchController.instance.AllPossibleResearchByKey;
		} else {
			ResearchController.instance.AllPossibleResearchByKey = ResearchController.instance.SortResearchByKey (ResearchController.instance.AllPossibleResearch);
			temp = ResearchController.instance.AllPossibleResearchByKey;
		}
		int i = 0;
		foreach(Text field in GameController.instance.outProject){
			int position = 0+(chapterPage*BUTTON_COUNT)+i;
			try{
				field.text = temp[position].name + ":  " +temp[position].cost;
				field.GetComponent<WorkID>().ID = temp[position].ID;
			}
			catch(ArgumentOutOfRangeException){
				field.text = "???";
				field.GetComponent<WorkID>().ID = null;
			}
			i++;
		}
		GUITools.setButtonVisible ("Index", true);
		if(chapterPage == 0){
			GameObject button;
			GUITools.setButtonVisible ("Previous", false);
			if(temp.Count <= BUTTON_COUNT+(chapterPage*BUTTON_COUNT)){
				GUITools.setButtonVisible ("Next", false);
			}
			else{
				GUITools.setButtonVisible ("Next", true);
			}
		}
	}
	
	public void setChapterToSoftware(){
		if(!SoftwareController.instance.PossibleSoftware.Any ()){
			setChapterToNone();
			return;
		}
		chapter = pickedType.Software;
		GUITools.setButtonVisible ("R2", true);
		GUITools.setButtonVisible ("R3", true);
		GUITools.setButtonVisible ("R4", true);
		GUITools.setButtonVisible ("R5", true);
		List<SoftwareProject> temp = SoftwareController.instance.PossibleSoftware;
		int i = 0;
		foreach(Text field in GameController.instance.outProject){
			int position = 0+(chapterPage*BUTTON_COUNT)+i;
			try{
				field.text = temp[position].name + ":  " +temp[position].pointCost;
				field.GetComponent<WorkID>().ID = temp[position].ID;
			}
			catch(ArgumentOutOfRangeException){
				field.text = "???";
				field.GetComponent<WorkID>().ID = null;
			}
			i++;
		}
		GUITools.setButtonVisible ("Index", true);
		if(chapterPage == 0){
			GameObject button;
			GUITools.setButtonVisible ("Previous", false);
			if(temp.Count <= BUTTON_COUNT+(chapterPage*BUTTON_COUNT)){
				GUITools.setButtonVisible ("Next", false);
			}
		}
	}
	
	public void setChapterToHardware(){
		if(!HardwareController.instance.PossibleHardware.Any ()){
			setChapterToNone();
			return;
		}
		chapter = pickedType.Hardware;
		GUITools.setButtonVisible ("R2", true);
		GUITools.setButtonVisible ("R3", true);
		GUITools.setButtonVisible ("R4", true);
		GUITools.setButtonVisible ("R5", true);
		List<HardwareProject> temp = HardwareController.instance.PossibleHardware;
		int i = 0;
		foreach(Text field in GameController.instance.outProject){
			int position = 0+(chapterPage*BUTTON_COUNT)+i;
			try{
				field.text = temp[position].name;
				field.GetComponent<WorkID>().ID = temp[position].ID;
			}
			catch(ArgumentOutOfRangeException){
				field.text = "???";
				field.GetComponent<WorkID>().ID = null;
			}
			i++;
		}
		GUITools.setButtonVisible ("Index", true);
		if(chapterPage == 0){
			GameObject button;
			GUITools.setButtonVisible ("Previous", false);
			if(temp.Count <= BUTTON_COUNT+(chapterPage*BUTTON_COUNT)){
				GUITools.setButtonVisible ("Next", false);
			}
			else{
				GUITools.setButtonVisible ("Next", true);
			}
		}
	}
	
	public void setChapterToParts(){
		chapter = pickedType.Parts;
		GUITools.setButtonVisible ("R2", true);
		GUITools.setButtonVisible ("R3", true);
		GUITools.setButtonVisible ("R4", true);
		GUITools.setButtonVisible ("R5", true);
		List<Part> temp = PartController.instance.allBuyableParts.Values.ToList();
		int i = 0;
		foreach(Text field in GameController.instance.outProject){
			int position = 0+(chapterPage*BUTTON_COUNT)+i;
			try{
				field.text = temp[position].name;
				field.GetComponent<WorkID>().ID = temp[position].ID;
			}
			catch(ArgumentOutOfRangeException){
				field.text = "???";
				field.GetComponent<WorkID>().ID = null;
			}
			i++;
		}
		GUITools.setButtonVisible ("Index", true);
		if(chapterPage == 0){
			GameObject button;
			GUITools.setButtonVisible ("Previous", false);
			if(temp.Count <= BUTTON_COUNT+(chapterPage*BUTTON_COUNT)){
				GUITools.setButtonVisible ("Next", false);
			}
			else{
				GUITools.setButtonVisible ("Next", true);
			}
		}
	}
	
	
	public int pageNumber(){
		return chapterPage;
	}
	
	public void next(){
		chapterPage+=1;
		if(chapter == pickedType.Research){
			List<Research> temp = ResearchController.instance.AllPossibleResearchByKey;
			int i = 0;
			foreach(Text field in GameController.instance.outProject){
				int position = 0+(chapterPage*BUTTON_COUNT)+i;
				try{
					field.text = temp[position].name + ": " +temp[position].cost;
					field.GetComponent<WorkID>().ID = temp[position].ID;
				}
				catch(ArgumentOutOfRangeException){
					field.text = "???";
					field.GetComponent<WorkID>().ID = null;
				}
				i++;
			}
			GUITools.setButtonVisible ("Previous", true);
			if(temp.Count <= BUTTON_COUNT+(chapterPage*BUTTON_COUNT)){
				GUITools.setButtonVisible("Next",false);
			}
			flicked = true;
		}
		else if(chapter == pickedType.Software){
			List<SoftwareProject> temp = SoftwareController.instance.PossibleSoftware;
			int i = 0;
			foreach(Text field in GameController.instance.outProject){
				int position = 0+(chapterPage*BUTTON_COUNT)+i;
				try{
					field.text = temp[position].name + ": " +temp[position].pointCost;
					field.GetComponent<WorkID>().ID = temp[position].ID;
				}
				catch(ArgumentOutOfRangeException){
					field.text = "???";
					field.GetComponent<WorkID>().ID = null;
				}
				i++;
			}
			GUITools.setButtonVisible ("Previous", true);
			if(temp.Count <= BUTTON_COUNT+(chapterPage*BUTTON_COUNT)){
				GUITools.setButtonVisible("Next",false);
			}
			flicked = true;
		}
		else if(chapter == pickedType.Hardware){
			List<HardwareProject> temp = HardwareController.instance.PossibleHardware;
			int i = 0;
			foreach(Text field in GameController.instance.outProject){
				int position = 0+(chapterPage*BUTTON_COUNT)+i;
				try{
					field.text = temp[position].name;
					field.GetComponent<WorkID>().ID = temp[position].ID;
				}
				catch(ArgumentOutOfRangeException){
					field.text = "???";
					field.GetComponent<WorkID>().ID = null;
				}
				i++;
			}
			GUITools.setButtonVisible ("Previous", true);
			if(temp.Count <= BUTTON_COUNT+(chapterPage*BUTTON_COUNT)){
				GUITools.setButtonVisible("Next",false);
			}
			flicked = true;
		}
		else if(chapter == pickedType.Parts){
			List<Part> temp = GameController.instance.allParts;
			int i = 0;
			foreach(Text field in GameController.instance.outProject){
				int position = 0+(chapterPage*BUTTON_COUNT)+i;
				try{
					field.text = temp[position].name + ": £" +temp[position].cost;
					field.GetComponent<WorkID>().ID = temp[position].ID;
				}
				catch(ArgumentOutOfRangeException){
					field.text = "???";
					field.GetComponent<WorkID>().ID = null;
				}
				i++;
			}
			GUITools.setButtonVisible ("Previous", true);
			if(temp.Count <= BUTTON_COUNT+(chapterPage*BUTTON_COUNT)){
				GUITools.setButtonVisible("Next",false);
			}
			flicked = true;
		}
		
		
	}
	
	public void previous(){
		chapterPage -= 1;
		if (chapter == pickedType.Research) {
			List<Research> temp = ResearchController.instance.AllPossibleResearchByKey;
			int i = 0;
			foreach (Text field in GameController.instance.outProject) {
				int position = 0 + (chapterPage * BUTTON_COUNT) + i;
				try {
					field.text = temp [position].name + ": " + temp [position].cost;
					field.GetComponent<WorkID> ().ID = temp [position].ID;
				} catch (ArgumentOutOfRangeException) {
					field.text = "???";
					field.GetComponent<WorkID>().ID = null;
				}
				i++;
			}
			if (chapterPage == 0) {
				GUITools.setButtonVisible ("Previous", false);
				if (temp.Count <= BUTTON_COUNT + (chapterPage * BUTTON_COUNT)) {
					GUITools.setButtonVisible ("Next", false);
				} else {
					GUITools.setButtonVisible ("Next", true);
				}
			} else if (chapterPage != 0) {
				GUITools.setButtonVisible ("Previous", true);
				if (temp.Count <= BUTTON_COUNT + (chapterPage * BUTTON_COUNT)) {
					GUITools.setButtonVisible ("Next", false);
				} else {
					GUITools.setButtonVisible ("Next", true);
				}
			}
			flicked = true;
		} else if (chapter == pickedType.Software) {
			List<SoftwareProject> temp = SoftwareController.instance.PossibleSoftware;
			int i = 0;
			foreach (Text field in GameController.instance.outProject) {
				int position = 0 + (chapterPage * BUTTON_COUNT) + i;
				try {
					field.text = temp [position].name + ": " + temp [position].pointCost;
					field.GetComponent<WorkID> ().ID = temp [position].ID;
				} catch (ArgumentOutOfRangeException) {
					field.text = "???";
					field.GetComponent<WorkID>().ID = null;
				}
				i++;
			}
			if (chapterPage == 0) {
				GUITools.setButtonVisible ("Previous", false);
				if (temp.Count <= BUTTON_COUNT + (chapterPage * BUTTON_COUNT)) {
					GUITools.setButtonVisible ("Next", false);
				} else {
					GUITools.setButtonVisible ("Next", true);
				}
			} else if (chapterPage != 0) {
				GUITools.setButtonVisible ("Previous", true);
				if (temp.Count <= BUTTON_COUNT + (chapterPage * BUTTON_COUNT)) {
					GUITools.setButtonVisible ("Next", false);
				} else {
					GUITools.setButtonVisible ("Next", true);
				}
			}
			flicked = true;
		} else if (chapter == pickedType.Hardware) {
			List<HardwareProject> temp = HardwareController.instance.PossibleHardware;
			int i = 0;
			foreach (Text field in GameController.instance.outProject) {
				int position = 0 + (chapterPage * BUTTON_COUNT) + i;
				try {
					field.text = temp [position].name;
					field.GetComponent<WorkID> ().ID = temp [position].ID;
				} catch (ArgumentOutOfRangeException) {
					field.text = "???";
					field.GetComponent<WorkID>().ID = null;
					
				}
				i++;
			}
			if (chapterPage == 0) {
				GUITools.setButtonVisible ("Previous", false);
				if (temp.Count <= BUTTON_COUNT + (chapterPage * BUTTON_COUNT)) {
					GUITools.setButtonVisible ("Next", false);
				} else {
					GUITools.setButtonVisible ("Next", true);
				}
			} else if (chapterPage != 0) {
				GUITools.setButtonVisible ("Previous", true);
				if (temp.Count <= BUTTON_COUNT + (chapterPage * BUTTON_COUNT)) {
					GUITools.setButtonVisible ("Next", false);
				} else {
					GUITools.setButtonVisible ("Next", true);
				}
			}
			flicked = true;
		} else if (chapter == pickedType.Parts) {
			List<Part> temp = GameController.instance.allParts;
			int i = 0;
			foreach (Text field in GameController.instance.outProject) {
				int position = 0 + (chapterPage * BUTTON_COUNT) + i;
				try {
					field.text = temp [position].name + ": " + temp [position].cost;
					field.GetComponent<WorkID> ().ID = temp [position].ID;
				} catch (ArgumentOutOfRangeException) {
					field.text = "???";
					field.GetComponent<WorkID>().ID = null;
				}
				i++;
			}
			if (chapterPage == 0) {
				GUITools.setButtonVisible ("Previous", false);
				if (temp.Count <= BUTTON_COUNT + (chapterPage * BUTTON_COUNT)) {
					GUITools.setButtonVisible ("Next", false);
				} else {
					GUITools.setButtonVisible ("Next", true);
				}
			} else if (chapterPage != 0) {
				GUITools.setButtonVisible ("Previous", true);
				if (temp.Count <= BUTTON_COUNT + (chapterPage * BUTTON_COUNT)) {
					GUITools.setButtonVisible ("Next", false);
				} else {
					GUITools.setButtonVisible ("Next", true);
				}
			}
			flicked = true;
		}
	}
	
	public void showPicker(){
		GameController.instance.picker.active = true;
		if(chapter == pickedType.Research ){
			setChapterToResearch();
		}
		else if(chapter == pickedType.Software){
			setChapterToSoftware();
		}
		else if(chapter == pickedType.Hardware){
			setChapterToHardware();
		}
		else if(chapter == pickedType.Parts){
			setChapterToParts();
		}
		else{
			setChapterToNone();
		}
		
	}
}
