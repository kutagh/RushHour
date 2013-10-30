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

        public Tree(Map initialConfiguration) {
            root = new Node(initialConfiguration);
        }

        public Node Find(Map toFind) {
            var queue = new Queue<Node>();
            var visited = new List<Node>();
            queue.Enqueue(root);
            while (queue.Count > 0) {
                var current = queue.Dequeue();
                if (current.value.Equals(toFind)) return current;
                visited.Add(current);
                foreach (var nb in current.neighbors.Where(x => !visited.Contains(x))) queue.Enqueue(nb);
            }
            return null;
        }

        public void AddNeighbor(Map addTo, Map toAdd) {
            var origin = Find(addTo);
            if (origin == null || origin.FindNeighbor(toAdd) != null) return; // Unlikely
            origin.AddNeighbor(toAdd);
            
        }

        public void AddNeighbor(Map addTo, Node toAdd) {
            var origin = Find(addTo);
            if (origin == null || origin.neighbors.Contains(toAdd)) return; // Unlikely
            origin.AddNeighbor(toAdd);
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
        
        public Node(T value) {
            neighbors = new List<Node<T>>();
            this.value = value;
        }

        public virtual void AddNeighbor(Node<T> node, bool sendToNB = true) {
            this.neighbors.Add(node);
            if (sendToNB)
                node.AddNeighbor(this, false);
        }

        public virtual void AddNeighbor(T value) {
            AddNeighbor(new Node<T>(value));
        }

        public virtual Node<T> FindNeighbor(T value) {
            return neighbors.FirstOrDefault(x => x.value.Equals(value));
        }
    }
}
