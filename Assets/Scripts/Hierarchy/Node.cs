using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using UnityEngineInternal;

public class Node : MonoBehaviour {
    public Shader highlightShader;
    public Research research;

    public void OnMouseEnter() {
        this.GetComponent<Renderer>().material.shader = highlightShader;

        Stack<Research> stack = new Stack<Research>();
        stack.Push(research);

        while (stack.Count > 0) {
            Research currentResearch = stack.Pop();
            
            changeLineColour(ResearchHierarchy.researchNodes.First(x => x.Value.GetComponent<Node>().research.ID == currentResearch.ID)
                             .Value, true);

            foreach (Research parent in currentResearch.Dependencies) {
                stack.Push(parent);
            }
        }
    }

    void changeLineColour(GameObject go, bool changeColour) {
        if(changeColour)
            go.transform.Find("parents").GetComponentsInChildren<LineRenderer>().ToList().ForEach(x => x.SetColors(Color.yellow, Color.yellow));
        else
            go.transform.Find("parents").GetComponentsInChildren<LineRenderer>().ToList().ForEach(x => x.SetColors(Color.magenta, Color.magenta));
    }

    public void OnMouseExit() {
        this.GetComponent<Renderer>().material.shader = Shader.Find("Diffuse");
        Stack<Research> stack = new Stack<Research>();
        stack.Push(research);

        while (stack.Count > 0) {
            Research currentResearch = stack.Pop();

            changeLineColour(ResearchHierarchy.researchNodes.Single(x => x.Value.GetComponent<Node>().research.ID == currentResearch.ID).Value, false);

            foreach (Research parent in currentResearch.Dependencies) {
                stack.Push(parent);
            }
        }
    }
}
