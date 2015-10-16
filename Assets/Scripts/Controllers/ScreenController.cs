using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ScreenController : MonoBehaviour {

    public List<Screen> screens;
    private Screen currentScreen;

    private static ScreenController _instance;
    public static ScreenController instance {
        get {
            return _instance ?? (_instance = FindObjectOfType<ScreenController>());
        }
    }

	// Use this for initialization
	void Start () {
        // Grab all screens in the scene
	    screens = FindObjectsOfType<Screen>().ToList();
        if (screens.Count == 0) {
            Utility.UnityLog("No screens have been set up!", 3);
            return;
        }

        // Screens should be disabled by default.
        screens.ForEach(x => x.enabled = false);

        // Set the main screen to be active
        // but this probably should be done by the GameController right?
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public static void MainScreen() {
        ChangeScreen(instance.screens.First(x => x.ScreenName == "MainScreen"));
    }

    public static void ChangeScreen(Screen channel) {
        // Firstly, run the transition on the current screen
        // then disable it.

        // Ideally this would be handled by the MonoBehaviour OnDisable()
        // function, but there's no easy way to tell that screen what the next
        // will be.

        if (instance.currentScreen != null) {
            instance.currentScreen.transitionOut(channel.ScreenName);
            instance.currentScreen.enabled = false;
        }

        // Now set the new screen and enable it.
        instance.currentScreen = channel;
        instance.currentScreen.enabled = true;
    }

    public static void ChangeScreen(string channel) {
        Screen tmp = instance.screens.FirstOrDefault(x => x.ScreenName == channel);
        if (tmp == null) {
            Utility.UnityLog(string.Format("Could not find screen name '{0}'", channel));
            return;
        }

        if (instance.currentScreen != null) {
            instance.currentScreen.transitionOut(channel);
            instance.currentScreen.enabled = false;
        }

        instance.currentScreen = tmp;
        instance.currentScreen.enabled = true;
    }
}
