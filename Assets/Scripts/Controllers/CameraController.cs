using System;

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

using UnityEngine.UI;

public class CameraController : MonoBehaviour {

    public Transform mainScreen;
    public Transform hierarchyScreen;
    public float mouseSensitivity;
    public float tolerance;
    public float speedMult;
    public bool bMainScreen;
    public bool bHierarchy;
    public float minZoom;
    public float maxZoom;
    public float zoomSensitivity;
    public float zoomDamping;
    public Screen screen;

    private Camera camera;
    private bool debugCamera;
    private Vector3 lastPosition;
    private Vector3 screenTopRight;
    private Vector3 screenBottomLeft;
    private float zoom;

    // Use this for initialization
    void Start() {
        // todo: add logic to enable/disable this
        bMainScreen = false;
        bHierarchy = true;
        camera = this.GetComponent<Camera>();

        if(screen != null)
            screen.Start();

        // Get (and store amount) children (rows) of hierarchy grid

        List<Transform> rows = GameObject.FindGameObjectsWithTag("ResearchNodeRow").Select(x => x.transform).ToList();

        int numRows = rows.Count;

        // Count number of columns in each row, eventually finding the max amount
        // int maxNum = rows.Max(x => x.GetChildren.Count);

        int maxColumns = rows.Max(row => row.childCount);

        // Grid width = max number of columns  * width-inc-padding
        // Grid height = number of rows * height-inc-padding

        float gridWidth = maxColumns * (rows[0].GetChild(0).GetComponent<Renderer>().bounds.size.x + 250);
        float gridHeight = numRows * (rows[0].GetChild(0).GetComponent<Renderer>().bounds.size.y + hierarchyScreen.GetComponent<GridLayoutGroup>().cellSize.y + hierarchyScreen.GetComponent<GridLayoutGroup>().spacing.y);

        // grid top right = x : width
        // grid bottom right = x: width, y : -height
        // grid bottom left = y : -height

        screenTopRight = new Vector3(hierarchyScreen.position.x + (gridWidth / 2), hierarchyScreen.position.y + (gridHeight / 2), 0);
        screenBottomLeft = new Vector3(hierarchyScreen.position.x - (gridWidth / 2), hierarchyScreen.position.y - (gridHeight / 2));

        // You may have to half these if you're using the pivot point of the hierarchyScreen, which lies in the middle!

        //camera.transform.position = screenBottomLeft;

        debugCamera = (GameObject.Find("DebugCamera") != null && GameObject.Find("DebugCamera").activeSelf);
    }

    void OnGUI() {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.gray);
        texture.wrapMode = TextureWrapMode.Repeat;
        texture.Apply();
        GUI.skin.box.normal.background = texture;
        Rect background = new Rect {
            xMin = screenBottomLeft.x,
            xMax = screenTopRight.x,
            yMin = screenTopRight.y,
            yMax = screenBottomLeft.y
        };
        GUI.Box(background, GUIContent.none);
    }

    void handleZoom() {
        zoom = camera.orthographicSize;
        zoom += (-Input.GetAxis("Mouse ScrollWheel")) * zoomSensitivity;
        //zoom = Mathf.Clamp(zoom, minZoom, maxZoom);
    }

    void LateUpdate() {
        camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, zoom, Time.deltaTime * zoomDamping);
    }
	
	// Update is called once per frame
	void Update () {
        // Only allow the drag camera if you are on the hierarchy screen
	    if (bHierarchy && !bMainScreen) {
            Vector3 cameraTopRight = camera.ViewportToWorldPoint(new Vector3(1, 1, camera.nearClipPlane));
            Vector3 cameraBottomLeft = camera.ViewportToWorldPoint(new Vector3(0, 0, camera.nearClipPlane));

	        if (debugCamera) {
                GameObject.Find("xbottomcheck").GetComponent<TextMesh>().text = (cameraBottomLeft.x - screenBottomLeft.x).ToString();
                GameObject.Find("ybottomcheck").GetComponent<TextMesh>().text = (cameraBottomLeft.y - screenBottomLeft.y).ToString();
                GameObject.Find("xtopcheck").GetComponent<TextMesh>().text = (screenTopRight.x - cameraTopRight.x).ToString();
                GameObject.Find("ytopcheck").GetComponent<TextMesh>().text = (screenTopRight.y - cameraTopRight.y).ToString();
	        }

	        if (GameObject.Find("topright") == null) {
	            GameObject topright = new GameObject("topright");
	            topright.transform.position = screenTopRight;
                topright.AddComponent<TextMesh>().text = string.Format("x: {0}\ny: {1}", topright.transform.position.x, topright.transform.position.y);
                topright.GetComponent<TextMesh>().fontSize = 1000;
	        }

            if (GameObject.Find("bottomleft") == null) {
                GameObject bottomleft = new GameObject("bottomleft");
                bottomleft.transform.position = screenBottomLeft;
                bottomleft.AddComponent<TextMesh>().text = string.Format("x: {0}\ny: {1}", bottomleft.transform.position.x, bottomleft.transform.position.y);
                bottomleft.GetComponent<TextMesh>().fontSize = 1000;
            }

            if (debugCamera) {
                GameObject.Find("xbottom").GetComponent<TextMesh>().text = cameraBottomLeft.x.ToString();
                GameObject.Find("ybottom").GetComponent<TextMesh>().text = cameraBottomLeft.y.ToString();
                GameObject.Find("xtop").GetComponent<TextMesh>().text = cameraTopRight.x.ToString();
                GameObject.Find("ytop").GetComponent<TextMesh>().text = cameraTopRight.y.ToString();
            }

            // If cameraTopRight.x equals or is greater than hBounds.x, block movement
            // If cameraTopRight.y equals or is greater than hBounds.y

	        if ((cameraBottomLeft.x - screenBottomLeft.x) < tolerance) {
                camera.transform.position = new Vector3(camera.transform.position.x + (tolerance * speedMult), camera.transform.position.y, camera.transform.position.z);
	            if (debugCamera)
	                GameObject.Find("xbottomcheck").GetComponent<TextMesh>().text = "false";
	            return;
	        }

	        if ((cameraBottomLeft.y - screenBottomLeft.y) < tolerance) {
                camera.transform.position = new Vector3(camera.transform.position.x, camera.transform.position.y + (tolerance * speedMult), camera.transform.position.z);
                if (debugCamera)
                    GameObject.Find("ybottomcheck").GetComponent<TextMesh>().text = "false";
	            return;
	        }

	        if ((screenTopRight.x - cameraTopRight.x) < tolerance) {
                camera.transform.position = new Vector3(camera.transform.position.x - (tolerance * speedMult), camera.transform.position.y, camera.transform.position.z);
                if (debugCamera)
                    GameObject.Find("xtopcheck").GetComponent<TextMesh>().text = "false";
                return;
	        }

            if ((screenTopRight.y - cameraTopRight.y) < tolerance) {
                camera.transform.position = new Vector3(camera.transform.position.x, camera.transform.position.y - (tolerance * speedMult), camera.transform.position.z);
                if (debugCamera)
                    GameObject.Find("ytopcheck").GetComponent<TextMesh>().text = "false";
                return;
            }

            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(2)) {
	            lastPosition = Input.mousePosition;
	        }

            if (Input.GetMouseButton(0) || Input.GetMouseButton(2)) {
	            Vector3 delta = Input.mousePosition - lastPosition;
	            transform.Translate((-delta.x)*mouseSensitivity, (-delta.y)*mouseSensitivity, 0);
	            lastPosition = Input.mousePosition;
	        }

            handleZoom();
	    }
	}
}
