using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using UnityEngine.UI;

// ReSharper disable RedundantOverridenMember
public class ResearchHierarchyScreen : Screen {
    protected override void Start() {
        base.Start();

        // Get (and store amount) children (rows) of hierarchy grid

        List<Transform> rows = GameObject.FindGameObjectsWithTag("ResearchNodeRow").Select(x => x.transform).ToList();

        int numRows = rows.Count;

        // Count number of columns in each row, eventually finding the max amount
        // int maxNum = rows.Max(x => x.GetChildren.Count);

        int maxColumns = rows.Max(row => row.childCount);

        // Grid width = max number of columns  * width-inc-padding
        // Grid height = number of rows * height-inc-padding

        float gridWidth = maxColumns * (rows[0].GetChild(0).GetComponent<Renderer>().bounds.size.x + 250);
        float gridHeight = numRows * (rows[0].GetChild(0).GetComponent<Renderer>().bounds.size.y + this.GetComponent<GridLayoutGroup>().cellSize.y + this.GetComponent<GridLayoutGroup>().spacing.y);

        // grid top right = x : width
        // grid bottom right = x: width, y : -height
        // grid bottom left = y : -height

        //bug: this not being overridden for some reason...

        screenTopRight = new Vector3(this.transform.position.x + (gridWidth / 2), this.transform.position.y + (gridHeight / 2), 0);
        screenBottomLeft = new Vector3(this.transform.position.x - (gridWidth / 2), this.transform.position.y - (gridHeight / 2));
    }

    protected override void Update() {
        base.Update();
    }

    protected override void LateUpdate() {
        base.LateUpdate();
    }

    protected override void createBoundaries() {
        cameraTopRight = camera.ViewportToWorldPoint(new Vector3(1, 1, camera.nearClipPlane));
        cameraBottomLeft = camera.ViewportToWorldPoint(new Vector3(0, 0, camera.nearClipPlane));

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
}
