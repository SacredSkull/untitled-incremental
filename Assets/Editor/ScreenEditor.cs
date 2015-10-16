using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Screen), true)]
public class ScreenEditor : Editor {
    private bool overrideTransform;

    public override void OnInspectorGUI() {
        EditorGUIUtility.LookLikeControls();

        Screen targetScreen = (Screen)target;
        GUIStyle richTextStyle = new GUIStyle { richText = true };

        EditorGUILayout.LabelField(new GUIContent("<b>Screen</b>"), richTextStyle);
        EditorGUI.indentLevel++;
        if (string.IsNullOrEmpty(targetScreen.ScreenName))
            targetScreen.ScreenName = targetScreen.name;
        targetScreen.ScreenName = EditorGUILayout.TextField("Name", targetScreen.ScreenName);
        targetScreen.hideOnStart = EditorGUILayout.Toggle(new GUIContent("Starts Hidden", "If true, this screen should be hidden until it is active."), targetScreen.hideOnStart);
        EditorGUI.indentLevel++;
        targetScreen.hideFadeReveal = EditorGUILayout.Toggle("Fade In", targetScreen.hideFadeReveal);
        EditorGUI.indentLevel--;
        overrideTransform = EditorGUILayout.Toggle(new GUIContent("Set Start Position", "By default, the camera begins at the center of this GameObject's position. This box allows you to select a different Transform to start at."), overrideTransform);
        if (overrideTransform) {
            EditorGUI.indentLevel++;
            targetScreen.StartingTransform = (Transform)EditorGUILayout.ObjectField("Custom Spawn", targetScreen.StartingTransform, typeof(Transform), true);
            EditorGUI.indentLevel--;
        }
        targetScreen.lockCursor = (CursorLockMode)EditorGUILayout.EnumPopup("Cursor Mode", targetScreen.lockCursor);
        EditorGUI.indentLevel--;

        EditorGUILayout.LabelField(new GUIContent("<b>Camera</b>"), richTextStyle);
        EditorGUI.indentLevel++;
        targetScreen.orthographic = EditorGUILayout.Toggle(new GUIContent("2D", "If true, sets the camera to the 2D othographic mode"), targetScreen.orthographic);
        targetScreen.updateMode();
        EditorGUI.indentLevel--;

        EditorGUILayout.LabelField(new GUIContent("<b>Movement</b>"), richTextStyle);
        EditorGUI.indentLevel++;
        targetScreen.dragControls = EditorGUILayout.Toggle("Drag", targetScreen.dragControls);
        if (targetScreen.dragControls) {
            EditorGUI.indentLevel++;
            targetScreen.dragMouseSensitivity = EditorGUILayout.IntSlider("Sensitivity", targetScreen.dragMouseSensitivity, 1, 200);
            targetScreen.dragMouseSmoothFactor = EditorGUILayout.IntSlider("Smoothing", targetScreen.dragMouseSmoothFactor, 1, 200);
            EditorGUI.indentLevel--;
        }
        targetScreen.zoomControls = EditorGUILayout.Toggle("Zoom", targetScreen.zoomControls);
        if (targetScreen.zoomControls) {
            EditorGUI.indentLevel++;
            targetScreen.zoomSensitivity = EditorGUILayout.IntSlider("Sensitivity", targetScreen.zoomSensitivity, 1, 2000);
            targetScreen.zoomDamping = EditorGUILayout.IntSlider(new GUIContent("Dampening", "This value is tied to the sensitivity of the zoom. A dampening factor of 5 results in a fairly smooth (but slow) zoom. For more choppy (but faster) approaches, increase this value."), targetScreen.zoomDamping, 1, 100);
            if (targetScreen.orthographic) {
                targetScreen.zoom2DMin = EditorGUILayout.IntSlider("Minimum\t(Size)", targetScreen.zoom2DMin, 1, 3000);
                targetScreen.zoom2DMax = EditorGUILayout.IntSlider("Maximum\t(Size)", targetScreen.zoom2DMax, 1, 3000);

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Set Min Size"))
                   targetScreen.zoom2DMin = (int)GameObject.Find("Main Camera").GetComponent<Camera>().orthographicSize;
                if (GUILayout.Button("Set Max Size"))
                    targetScreen.zoom2DMax = (int)GameObject.Find("Main Camera").GetComponent<Camera>().orthographicSize;
                EditorGUILayout.EndHorizontal();
            } else {
                targetScreen.zoom3DMin = EditorGUILayout.IntSlider("Minimum\t(FOV)", targetScreen.zoom3DMin, 1, 178);
                targetScreen.zoom3DMax = EditorGUILayout.IntSlider("Maximum\t(FOV)", targetScreen.zoom3DMax, 2, 179);

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Set Min FOV"))
                    targetScreen.zoom3DMin = (int)GameObject.Find("Main Camera").GetComponent<Camera>().fieldOfView;
                if (GUILayout.Button("Set Max FOV"))
                    targetScreen.zoom3DMax = (int)GameObject.Find("Main Camera").GetComponent<Camera>().fieldOfView;
                EditorGUILayout.EndHorizontal();
            }
            EditorGUI.indentLevel--;
        }
        targetScreen.autoLimitBoundaries = EditorGUILayout.Toggle(new GUIContent("Auto Boundaries", "Attempt to generate boundaries for a screen based on its 'renderer.boundary' or 'RectTransform.rect.width/height'. This may not work every time, in which case you must compute it manually."), targetScreen.autoLimitBoundaries);
        if (targetScreen.autoLimitBoundaries) {
            EditorGUI.indentLevel++;
            targetScreen.limitTolerance =
                EditorGUILayout.Slider(
                                       new GUIContent("Tolerance",
                                                      "If the distance between the border is greater than this value, movement will begin. To set a hard limit, directly on the border, set this to 0 and a high reset speed."),
                                       targetScreen.limitTolerance, 0, 100);
            targetScreen.limitResetSpeedMultiplier =
                EditorGUILayout.Slider(
                                       new GUIContent("Reset Speed Mult.",
                                                      "The camera will reset back to a border by this speed multiplier. If you want a slow reset time, set a low speed."),
                                       targetScreen.limitResetSpeedMultiplier, 0, 600);
            targetScreen.limitUseCustomObject =
                EditorGUILayout.Toggle(
                                       new GUIContent("Use Other Bounds",
                                                      "Use another object's boundaries to automatically set the screen size."),
                                       targetScreen.limitUseCustomObject);
            if (targetScreen.limitUseCustomObject)
                targetScreen.limitObjectBounds = (GameObject)EditorGUILayout.ObjectField("Object", targetScreen.limitObjectBounds, typeof(GameObject), true);
            EditorGUI.indentLevel--;
        }
        EditorGUI.indentLevel--;

        EditorGUILayout.LabelField(new GUIContent("<b>Debug</b>"), richTextStyle);
        EditorGUI.indentLevel++;

        targetScreen.debugScreen = EditorGUILayout.Toggle("Show Co-ords", targetScreen.debugScreen);
        EditorGUI.indentLevel--;

        if (targetScreen.zoomControls) {}

        if (targetScreen.autoLimitBoundaries)
            EditorGUILayout.HelpBox("If an object's 'renderer.bounds' returns incorrect size data or a renderer (or meshrenderer) component is not attached, you will have to create a manual targeting function on this screen.", MessageType.Warning);
        if(targetScreen.zoomControls && targetScreen.zoom2DMin >= targetScreen.zoom2DMax)
            EditorGUILayout.HelpBox("The minimum zoom value (as the name suggests) should always be smaller than the maximum", MessageType.Error);
        if(!targetScreen.hideOnStart && targetScreen.hideFadeReveal)
            targetScreen.hideOnStart = true;
        if (targetScreen.hideOnStart || targetScreen.hideFadeReveal) {
            EditorGUILayout.HelpBox("Hiding (and therefore fading) has not been implemented yet, but feel free to tick this box and it eventually work.", MessageType.Error);
        }
        if(targetScreen.autoLimitBoundaries && targetScreen.zoomControls)
            EditorGUILayout.HelpBox("Please test the maximum (furthest out) zoom to make sure you don't appear outside the boundaries, as the zoom function *currently* has no appreciation of them.", MessageType.Warning);
    }
}