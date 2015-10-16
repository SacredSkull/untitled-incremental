using JetBrains.Annotations;

using UnityEditor;

using UnityEngine;

public abstract class Screen : MonoBehaviour {
    /**
 * @class   GameController
 *
 * @brief   This abstract class is the base for all screens that we use in the game.
 *          
 * @details Each derivitive should probably only exist once (exceptions include common dialog screens). This class includes helper functions, such as `handleDragMovement()` which, as the name suggests, does everything necessary to manage a camera's movement by dragging. The idea is to use the basic functions where this will work. For more specific/advanced functionality, override it and do with it what you will. An example is the ResearchHierarchyScreen which has to have its own `autoLimitBoundary()` because unity's there was no renderer on the main object.
 *          
 * @warning You will need to ensure that Start & Update exist in derivitive classes (override or otherwise) and then call
 *          base.Start() or base.Update() if you want the default camera to be found. 
 *
 * @author  Peter
 * @date    07/08/2015
 */

    // General toggles
    public bool orthographic = true;
    public bool dragControls = false;
    public bool zoomControls = false;
    public bool autoLimitBoundaries = false;
    public bool limitUseCustomObject = false;
    public bool hideOnStart = false;
    public bool hideFadeReveal = false;
    public bool debugScreen = false;
    public bool dragForceCameraZ = false;
    public CursorLockMode lockCursor = CursorLockMode.None;

    // Specifics
    public string ScreenName;
    public Transform StartingTransform;
    public Vector3 startingRotation = Vector3.zero;
    public GameObject limitObjectBounds;

    // Numeric values
    public int dragMouseSensitivity = 5;
    public int dragMouseSmoothFactor = 5;
    public int zoom2DMin = 140;
    public int zoom2DMax = 350;
    public int zoomSensitivity = 80;
    public int zoomDamping = 5;
    public int zoom3DMin = 20;
    public int zoom3DMax = 120;
    public float limitResetSpeedMultiplier = 200;
    public float limitTolerance = 1;

    // Private and protected (accessible by deriving classes)
    protected Camera camera;
    protected GameObject debugScreenPrefab;
    protected Vector3 screenTopRight;
    protected Vector3 screenBottomLeft;
    protected Vector3 cameraTopRight;
    protected Vector3 cameraBottomLeft;
    protected Vector3 dragLastPosition;
    protected float zoomCurrent = 10;

    [UsedImplicitly]
    protected virtual void Start() {
        camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        camera.orthographic = orthographic;
        float startingZ = camera.transform.position.z;

        // If the StartingTransform is empty/null, then use the current gameobject, making sure to NOT set the z axis which screws up the camera.
        camera.transform.position = StartingTransform != null ? new Vector3(StartingTransform.position.x, StartingTransform.position.y, startingZ) : new Vector3(this.transform.position.x, this.transform.position.y, startingZ);
        camera.transform.rotation = orthographic ?  Quaternion.Euler(0,0,0) : Quaternion.Euler(startingRotation);

        if (debugScreen) {
            debugScreenPrefab = Resources.Load("Debug/DebugScreenCanvas") as GameObject;
            if (debugScreenPrefab == null)
                Utility.UnityLog("Could not find DebugScreenCanvas in resources!", 1);
            else {
                debugScreenPrefab = Instantiate(debugScreenPrefab);
                debugScreenPrefab.GetComponent<Canvas>().worldCamera = this.camera;
            }
        }

        if (limitObjectBounds == null || !limitUseCustomObject) {
            Utility.UnityLog("Assuming current object is the correct bounding object");
            limitObjectBounds = this.gameObject;
        }

        Cursor.lockState = lockCursor;
    }

    [UsedImplicitly]
    protected virtual void Update() {
    }

    [UsedImplicitly]
    protected virtual void LateUpdate() {
        if (debugScreen)
            handleScreenDebug();
        if (autoLimitBoundaries)
            createBoundaries();
        if (dragControls)
            handleDragMovement();
        if (zoomControls)
            handleZoomMovement();
    }

    public void updateMode() {
        if(this.camera == null)
            this.camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        this.camera.orthographic = orthographic;
    }

    protected virtual void createBoundaries() {
        cameraTopRight = camera.ViewportToWorldPoint(new Vector3(1, 1, camera.nearClipPlane));
        cameraBottomLeft = camera.ViewportToWorldPoint(new Vector3(0, 0, camera.nearClipPlane));

        if (limitObjectBounds.GetComponent<Renderer>() != null) {
            screenTopRight = limitObjectBounds.transform.position + (limitObjectBounds.GetComponent<Renderer>().bounds.size / 2);
            screenBottomLeft = limitObjectBounds.transform.position - (limitObjectBounds.GetComponent<Renderer>().bounds.size / 2); ;
        } else if (limitObjectBounds.GetComponent<RectTransform>() != null) {
            screenTopRight =
                new Vector3(
                    limitObjectBounds.transform.position.x +
                    (limitObjectBounds.GetComponent<RectTransform>().rect.width/2),
                    limitObjectBounds.transform.position.y +
                    (limitObjectBounds.GetComponent<RectTransform>().rect.height/2), 0);
            screenBottomLeft =
                new Vector3(
                    limitObjectBounds.transform.position.x -
                    (limitObjectBounds.GetComponent<RectTransform>().rect.width/2),
                    limitObjectBounds.transform.position.y -
                    (limitObjectBounds.GetComponent<RectTransform>().rect.height/2), 0);
        } else {
            Utility.UnityLog("Could not find a suitable bound component!", 3);
        }

        if (debugScreen) {
            if (GameObject.Find("screenDebugTopRight") == null) {
                GameObject topright = new GameObject("screenDebugTopRight");
                topright.transform.position = screenTopRight;
                topright.AddComponent<TextMesh>().text = string.Format("x: {0}\ny: {1}", topright.transform.position.x,
                                                                       topright.transform.position.y);
                topright.GetComponent<TextMesh>().fontSize = 1000;
            }

            if (GameObject.Find("screenDebugBottomLeft") == null) {
                GameObject bottomleft = new GameObject("screenDebugBottomLeft");
                bottomleft.transform.position = screenBottomLeft;
                bottomleft.AddComponent<TextMesh>().text = string.Format("x: {0}\ny: {1}",
                                                                         bottomleft.transform.position.x,
                                                                         bottomleft.transform.position.y);
                bottomleft.GetComponent<TextMesh>().fontSize = 1000;
            }
        }
    }

    protected virtual void handleScreenDebug() {
        if (debugScreen && debugScreenPrefab != null) {
            Vector3 bottomLeftDifference = new Vector3(Mathf.Abs(cameraBottomLeft.x - screenBottomLeft.x),
                             Mathf.Abs(cameraBottomLeft.y - screenBottomLeft.y));
            Vector3 topRightDifference = new Vector3(Mathf.Abs(screenTopRight.x - cameraTopRight.x),
                                         Mathf.Abs(screenTopRight.y - cameraTopRight.y));

            debugScreenPrefab.transform.Find("xbottomcheck").GetComponent<TextMesh>().text = (bottomLeftDifference.x - limitTolerance).ToString();
            debugScreenPrefab.transform.Find("ybottomcheck").GetComponent<TextMesh>().text = (bottomLeftDifference.y - limitTolerance).ToString();
            debugScreenPrefab.transform.Find("xtopcheck").GetComponent<TextMesh>().text = (topRightDifference.x - limitTolerance).ToString();
            debugScreenPrefab.transform.Find("ytopcheck").GetComponent<TextMesh>().text = (topRightDifference.y - limitTolerance).ToString();

            debugScreenPrefab.transform.Find("xbottom").GetComponent<TextMesh>().text = cameraBottomLeft.x.ToString();
            debugScreenPrefab.transform.Find("ybottom").GetComponent<TextMesh>().text = cameraBottomLeft.y.ToString();
            debugScreenPrefab.transform.Find("xtop").GetComponent<TextMesh>().text = cameraTopRight.x.ToString();
            debugScreenPrefab.transform.Find("ytop").GetComponent<TextMesh>().text = cameraTopRight.y.ToString();
        }
    }

    protected virtual void handleDragMovement() {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(2)) {
            dragLastPosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(0) || Input.GetMouseButton(2)) {
            Vector3 delta = Input.mousePosition - dragLastPosition;

            if (autoLimitBoundaries) {
                Vector3 currentPosition = camera.transform.position;
                Vector3 posDiff = cameraTopRight - (delta + screenTopRight);
                Vector3 negDiff = (delta + screenBottomLeft) - cameraBottomLeft;

                float camHalfWidth = (cameraTopRight.x - cameraBottomLeft.x)/2;
                float camHalfHeight = (cameraTopRight.y - cameraBottomLeft.y)/2;

                if (posDiff.x > -limitTolerance) {
                    camera.transform.position =
                        new Vector3(
                            screenTopRight.x - (orthographic ? camHalfWidth : -camHalfWidth) - limitTolerance - 1,
                            currentPosition.y, currentPosition.z);
                    delta = new Vector3(0, delta.y);
                }
                if (posDiff.y > -limitTolerance) {
                    camera.transform.position = new Vector3(currentPosition.x,
                                                            screenTopRight.y -
                                                            (orthographic ? camHalfHeight : -camHalfHeight) -
                                                            limitTolerance - 1, currentPosition.z);
                    delta = new Vector3(delta.x, 0);
                }

                if (negDiff.x > -limitTolerance) {
                    camera.transform.position =
                        new Vector3(
                            screenBottomLeft.x + (orthographic ? camHalfWidth : -camHalfWidth) + limitTolerance + 1,
                            currentPosition.y, currentPosition.z);
                    delta = new Vector3(0, delta.y);
                }
                if (negDiff.y > -limitTolerance) {
                    camera.transform.position = new Vector3(currentPosition.x,
                                                            screenBottomLeft.y +
                                                            (orthographic ? camHalfHeight : -camHalfHeight) +
                                                            limitTolerance + 1, currentPosition.z);
                    delta = new Vector3(delta.x, 0);
                }
            }

            // Inverted axis feels so much better!
            delta = -delta;

            camera.transform.Translate(Mathf.Lerp(delta.x, delta.x * dragMouseSensitivity, Time.deltaTime * dragMouseSmoothFactor), Mathf.Lerp(delta.y, delta.y * dragMouseSensitivity, Time.deltaTime * dragMouseSmoothFactor), 0);
            dragLastPosition = Input.mousePosition;
        }
    }
    
    protected virtual void handleZoomMovement() {

        //todo: handle checking for zooming in a boundary, like drag does

        if (this.orthographic || this.dragForceCameraZ) {
            //zoomCurrent = camera.orthographicSize;
            zoomCurrent += (-Input.GetAxis("Mouse ScrollWheel")) * zoomSensitivity;
            zoomCurrent = Mathf.Clamp(zoomCurrent, zoom2DMin, zoom2DMax);
        } else {
            zoomCurrent += (-Input.GetAxis("Mouse ScrollWheel")) * zoomSensitivity;
            zoomCurrent = Mathf.Clamp(zoomCurrent, zoom3DMin, zoom3DMax);
        }

        if (orthographic && !dragForceCameraZ)
            camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, zoomCurrent, Time.deltaTime * zoomDamping);
        else {
//            float z = Mathf.Lerp(camera.transform.position.z, zoomCurrent + camera.transform.position.z, Time.deltaTime * zoomDamping);
//            camera.transform.Translate(new Vector3(0, 0, z));
//            zoomCurrent -= Input.GetAxis("Mouse ScrollWheel") * zoomSensitivity;
            camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, zoomCurrent, Time.deltaTime * zoomDamping);
        }
    }

    public virtual void transitionIn(string previousScreenName) {
        switch(previousScreenName)
        {
            case "MainScreen":
                break;
            case "ResearchHierarchy":
                break;
            default:
                break;

        }
    }

    public virtual void transitionOut(string nextScreenName) {
        
    }
}