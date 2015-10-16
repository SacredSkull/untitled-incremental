using UnityEngine;
using System.Collections;

public class WhiteboardEvents : MonoBehaviour {
    public void OnMouseDown() {
        ScreenController.ChangeScreen("ResearchHierarchy");
    }

    void OnMouseOver() {
        // do something cool, such as highlight the borders or change text etc
        Utility.UnityLog("ASD");
    }
}
