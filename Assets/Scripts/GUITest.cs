using System;
using System.Text;
using System.Xml.Linq;

using UnityEngine;

public class GUITest : MonoBehaviour {
    public GameObject researchNodePrefab;

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

    private void Start() {
        int x = 0;
        int y = 0;
        int column = 1;
        foreach (Research r in GameController.instance.allResearch) {
            // Produce 8 columns, then create new row
            if (column >= 8) {
                column = 1;
                y -= 150;
                x = 0;
            }

            // Instantiate, and then angle to face camera
            GameObject newNode = (GameObject) Instantiate(researchNodePrefab, new Vector3(x, y, 0), Quaternion.identity);
            newNode.transform.eulerAngles = new Vector3(90, 180, 0);

            var newNodeTitle = newNode.transform.Find("title");
            var newNodeDesc = newNode.transform.Find("description");
            newNodeTitle.gameObject.GetComponent<TextMesh>().text = FormatString(r.name, 13);
            newNodeDesc.gameObject.GetComponent<TextMesh>().text = FormatString(r.description, 24);
            while (newNodeTitle.GetComponent<Renderer>().bounds.size.y > 55) {
                newNodeTitle.GetComponent<TextMesh>().fontSize = (int)Math.Floor(newNodeTitle.GetComponent<TextMesh>().fontSize * 0.9);
            }
            float xContainer = newNodeTitle.parent.transform.position.x + (newNodeTitle.parent.GetComponent<Renderer>().bounds.size.x * 0.45f);
            float xWord = newNodeTitle.transform.position.x + newNodeTitle.GetComponent<Renderer>().bounds.size.x;
            float difference = xContainer - xWord; 

            newNodeTitle.transform.position = new Vector3(newNodeTitle.transform.position.x + difference, newNodeTitle.transform.position.y, newNodeTitle.transform.position.z);

            float xDesc = newNodeDesc.transform.position.x + newNodeDesc.GetComponent<Renderer>().bounds.size.x;
            difference = xContainer - xDesc;

            newNodeDesc.transform.position = new Vector3(newNodeDesc.transform.position.x + difference, newNodeDesc.transform.position.y, newNodeDesc.transform.position.z);
       
            // xWord + xWord.Width (1)
            // xContainer + xContainer.Width - 1 (3)
            // difference = xContainerTotal - xWordTotal (0)
            // xNewWord += difference

            x += 300;
            column++;
        }
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
