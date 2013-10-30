using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Node = RushHour.Node<RushHour.Map>;

namespace RushHour {
    class Tree {
        public Node root { get; protected set; }

        public Dictionary<Map, Node<Map>> mapDict = new Dictionary<Map,Node<Map>>();

        public Tree(Map initialConfiguration) {
            root = new Node(initialConfiguration,0);
        }

        //public Node Find(Map toFind) {
        //    var queue = new Queue<Node>();
        //    var visited = new List<Node>();
        //    queue.Enqueue(root);
        //    while (queue.Count > 0) {
        //        var current = queue.Dequeue();
        //        if (current.value.Equals(toFind)) return current;
        //        visited.Add(current);
        //        foreach (var nb in current.neighbors.Where(x => !visited.Contains(x))) queue.Enqueue(nb);
        //    }
        //    root = new Node<Map>(initialConfiguration, 0);
        //}

        public Node<Map> Find(Map toFind) {
            if (mapDict.ContainsKey(toFind))   //O(1) [hash tabel: je vind (bijna) direct wat je zoekt]
                return mapDict[toFind];
            //oude code voor vinden in een boom; O(n) [worst case: je moet alle elementen bekijken]
            //var queue = new Queue<Node<Map>>();
            //var visited = new List<Node<Map>>();
            //queue.Enqueue(root);
            //while (queue.Count > 0) {
            //    var current = queue.Dequeue();
            //    if (current.value.Equals(toFind)) return current;
            //    visited.Add(current);
            //    foreach (var nb in current.neighbors.Where(x => !visited.Contains(x))) queue.Enqueue(nb);
            //}
            return null;
        }

        public void AddNeighbor(Map addTo, Map toAdd) {
            var origin = Find(addTo);
            if (origin == null || origin.FindNeighbor(toAdd) != null) return; // Unlikely
            origin.AddNeighbor(toAdd);
            mapDict.Add(toAdd, origin.FindNeighbor(toAdd));
        }

        public void AddNeighbor(Map addTo, Node toAdd) {
            var origin = Find(addTo);
            if (origin == null || origin.neighbors.Contains(toAdd)) return; // Unlikely
            origin.AddNeighbor(toAdd);
            mapDict.Add(toAdd.value, toAdd);
        }

        public void rehangNeighbors(Map key, Node<Map> existingLoc) {
            Node<Map> prevBoard = Find(key);
            if (prevBoard.depth < existingLoc.depth)
            {
                var prevParent = existingLoc.parent;    //the previous parent
                existingLoc.parent = prevBoard;         //new parent
                existingLoc.depth = prevBoard.depth + 1;//new depth (get from new parent)
                existingLoc.AddNeighbor(prevBoard);     //add new parent to connections
                existingLoc.removeNeighbor(prevParent); //remove old parent from connections
                prevParent.removeNeighbor(existingLoc); //remove from connections of old parent
                prevBoard.AddNeighbor(existingLoc);     //add to connections of new parent
            }
        }

        public Stack<Node> FindShortest(Map target) {
            var stack = new Stack<Node>();
            var visited = new List<Node>();
            stack.Push(root);
            Iterate(stack, visited, target);
            return stack;
        }
        bool Iterate(Stack<Node> stack, List<Node> visited, Map target) {
            var current = stack.Peek();
            visited.Add(current);
            foreach (var nb in current.neighbors.Where(x => !visited.Contains(x))) {
                stack.Push(nb);
                if (nb.value == target) return true;
                if (Iterate(stack, visited, target))
                    return true;
            }
            stack.Pop();
            return false;
        }
    }


    class Node<T> {
        internal T value;
        internal List<Node<T>> neighbors;

        internal Node<T> parent = null;
        internal int depth;
        
        public Node(T value, int deep) {
            neighbors = new List<Node<T>>();
            this.value = value;
            depth = deep;
        }

        public virtual void AddNeighbor(Node<T> node, bool sendToNB = true) {
            this.neighbors.Add(node);
            if (sendToNB)
                node.AddNeighbor(this, false);
        }

        public virtual void AddNeighbor(T value) {
            AddNeighbor(new Node<T>(value, depth + 1));
        }

        public void removeNeighbor(Node<T> node) {
            if (node == parent) throw new Exception();  //something must have gone wrong in rehanging
            neighbors.Remove(node);
        }

        public virtual Node<T> FindNeighbor(T value) {
            return neighbors.FirstOrDefault(x => x.value.Equals(value));
        }
    }
}
