using System;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class ResearchHierarchy : MonoBehaviour {
    public GameObject researchNodePrefab;
    public GameObject researchRowPrefab;

//    public Texture aTexture;
//    void OnGUI() {
//        if (!aTexture) {
//            Debug.LogError("Assign a Texture in the inspector.");
//            return;
//        }
//        Rect r = new Rect(0, 0, 0, 0) {
//            height = 1000,
//            width = 1000
//        };
//        GUI.DrawTexture(r, aTexture, ScaleMode.ScaleToFit, true, 10.0F);
//    }

    public int maxLineChars = 10;

        //maximum number of characters per line...experiment with different values to make it work

    public int minLineChars = 5; //a limiter to avoid abusive use of return carriage

    private int x;
    private int y;
    private int column = 1;

    private Dictionary<int, GameObject> researchRows;
    public static Dictionary<int, GameObject> researchNodes;
    public Transform researchGrid;

    private IEnumerator Start() {
        researchNodes = new Dictionary<int, GameObject>();
        researchRows = new Dictionary<int, GameObject>();
        foreach (Research rNode in GameController.instance.allResearch) {
            if (!researchNodes.ContainsKey(rNode.ID))
                researchNodes.Add(rNode.ID, makeNode(rNode));
        }

        return createHierarchy();
    }

    private IEnumerator createHierarchy() {
        yield return new WaitForEndOfFrame();

        foreach (Research genesisResearch in GameController.instance.allResearch.Where(z => z.Dependencies.Count == 0)) {
            makeTree(genesisResearch, ref researchNodes);
        }
    }
        
    private GameObject makeNode(Research r) {

        // Produce 8 columns, then create new row
        if (column >= 8) {
            column = 1;
            y -= 1000;
            x = 0;
        }
        x += 2000;
        column++;

        

        // Instantiate, and then angle to face camera
        GameObject newNode = (GameObject)Instantiate(researchNodePrefab, new Vector3(0, 0, 0), Quaternion.identity);
        int nodeDepth = getMaxDepth(r);

        if (!researchRows.ContainsKey(nodeDepth)) {
            researchRows.Add(nodeDepth,
                             (GameObject)
                                 Instantiate(researchRowPrefab, new Vector3(0, 0, 0), Quaternion.Euler(90, 180, 0)));
            researchRows[nodeDepth].transform.SetParent(researchGrid.transform);
            researchRows[nodeDepth].transform.SetAsLastSibling();
        }


        newNode.transform.SetParent(researchRows[nodeDepth].transform, true);
        newNode.transform.eulerAngles = new Vector3(90, 180, 0);
        newNode.name = string.Format("Node ({0})", r.ID);
        newNode.GetComponent<Node>().research = r;

        Transform title = newNode.transform.Find("title");
        Transform newNodeDesc = newNode.transform.Find("description");
        Transform depth = newNode.transform.Find("depth");

        depth.GetComponent<TextMesh>().text = nodeDepth.ToString();

        title.gameObject.GetComponent<TextMesh>().text = FormatString(r.name, 13);
        newNodeDesc.gameObject.GetComponent<TextMesh>().text = FormatString(r.description, 24);
        while (title.GetComponent<Renderer>().bounds.size.y > 55) {
            title.GetComponent<TextMesh>().fontSize = (int)Math.Floor(title.GetComponent<TextMesh>().fontSize * 0.9);
        }
        float xContainer = title.parent.transform.position.x + (title.parent.GetComponent<Renderer>().bounds.size.x * 0.45f);
        float xWord = title.transform.position.x + title.GetComponent<Renderer>().bounds.size.x;
        float difference = xContainer - xWord;



        //depth.GetComponent<TextMesh>().text = getMaxNodeDepth(r).ToString();

        title.transform.position = new Vector3(title.transform.position.x + difference, title.transform.position.y, title.transform.position.z);

        float xDesc = newNodeDesc.transform.position.x + newNodeDesc.GetComponent<Renderer>().bounds.size.x;
        difference = xContainer - xDesc;

        newNodeDesc.transform.position = new Vector3(newNodeDesc.transform.position.x + difference, newNodeDesc.transform.position.y, newNodeDesc.transform.position.z);

        return newNode;
    }

    private void makeTree(Research r, ref Dictionary<int, GameObject> researchNodesDictionary) {
        if (researchNodes[r.ID].transform.Find("parents") == null) {
            GameObject parents = new GameObject("parents");
            parents.transform.SetParent(researchNodes[r.ID].transform);
            foreach (Research rParent in r.Dependencies) {
                GameObject go = new GameObject();
                go.transform.SetParent(parents.transform);
                go.SetActive(true);
                go.name = rParent.name;
                go.transform.SetAsFirstSibling();

                //go.transform.position = go.transform.parent.position;
                //go.transform.localPosition = Vector3.zero;

                //Utility.UnityLog(string.Format("Parent <b>{0}</b> local ({1}) world ({2}), child <b>{3}</b> local ({4}) world ({5})", rParent.name, researchNodes[rParent.ID].transform.localPosition, researchNodes[rParent.ID].transform.position, r.name, researchNodes[r.ID].transform.localPosition, researchNodes[r.ID].transform.position));

                LineRenderer line = go.AddComponent<LineRenderer>();
                line.useWorldSpace = true;
                line.SetVertexCount(2);
                line.SetWidth(10, 10);
                line.SetPosition(0,
                                 new Vector3(researchNodes[r.ID].transform.position.x,
                                             researchNodes[r.ID].transform.position.y, 10));
                line.SetPosition(1,
                                 new Vector3(researchNodes[rParent.ID].transform.position.x,
                                             researchNodes[rParent.ID].transform.position.y, 10));
            }
        }
        foreach (Research rChild in r.Children) {
            makeTree(rChild, ref researchNodesDictionary);
        }

    }

    private static List<List<Research>> getAllPaths(Research r) {
        if (r.Dependencies == null || r.isOrphan)
            return new List<List<Research>> { new List<Research> { r } };
        List<List<Research>> allPaths = r.Dependencies.SelectMany(item => getAllPaths(item)).ToList();
        allPaths.ForEach(path => path.Insert(0, r));
        return allPaths;
    }

    private static int getMaxDepth(Research r) {
        return getAllPaths(r).Max(path => path.Count);
    }

    private string FormatString(string text, int maxWidth = 20) {
        string[] words = text.Split(' ');

        StringBuilder wrappedText = new StringBuilder();
        string line = "";

        foreach (string word in words) {
            if ((line + word).Length > maxWidth) {
                wrappedText.AppendLine(line);
                line = "";
            }

            line += string.Format("{0}", word + " ");
        }
        if (line.Length > 0) {
            wrappedText.AppendLine(line);
        }

        return wrappedText.ToString();
    }
}