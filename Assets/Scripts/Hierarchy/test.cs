using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts {

    public class Node {
        public string name;
        public int ID;
        public readonly List<Node> Dependencies = new List<Node>();
        public readonly List<Node> Children = new List<Node>();

        public bool isOrphan {
            get {
                return Dependencies.Count == 0;
            }
        }

        public bool isParent {
            get {
                return Children.Count != 0;
            }
        }
    }

    public class test {

        public static void main() {
            Node A = new Node() {
                name  = "A",
                ID = 1
            };

            Node B = new Node() {
                name = "B",
                ID = 2
            };

            Node C = new Node() {
                name = "C",
                ID = 3
            };

            Node D = new Node() {
                name = "D",
                ID = 4
            };

            A.Children.Add(B);
            B.Children.Add(C);
            B.Children.Add(D);
            C.Children.Add(D);

            B.Dependencies.Add(A);
            C.Dependencies.Add(B);
            D.Dependencies.Add(B);
            D.Dependencies.Add(C);
        }

        private int getMaxNodeDepth(Node n, string listIndex = "base",
                        Dictionary<string, List<int>> paths = null) {
            bool firstIteration = false;

            if (paths == null) {
                firstIteration = true;
                listIndex = n.name.Replace(" ", "-");
                paths = new Dictionary<string, List<int>> {
            {listIndex, new List<int>(0)} 
        };
            }

            // Prevent the starting node from being added to the path
            if (!paths[listIndex].Contains(n.ID) && !firstIteration)
                paths[listIndex].Add(n.ID);

            // This variable should take the CURRENT path and store it; 
            // not the value after all the recursion has completed.
            // Right now, the current path is affected by the recursions, somehow...
            List<int> currentPath = new List<int>();
            int[] referenceToValue = paths[listIndex].ToArray();
            currentPath = referenceToValue.ToList();

            foreach (Node parent in n.Dependencies) {
                if (n.Dependencies.Count >= 2) {
                    paths.Remove(listIndex);
                    listIndex = parent.name.Replace(" ", "-");
                    paths.Add(listIndex, firstIteration ? currentPath : new List<int>());
                }
                getMaxNodeDepth(parent, listIndex, paths);
            }
            // Order all paths by length, return the highest count
            // This is to be used to space out the hierarchy properly
            return paths.Values.OrderByDescending(path => path.Count).First().Count;
        }
    }
}
