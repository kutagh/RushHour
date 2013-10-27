using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RushHour {
    class Tree {
        public Node<Map> root { get; protected set; }

        public Tree(Map initialConfiguration) {
            root = new Node<Map>(initialConfiguration);
        }

        public Node<Map> Find(Map toFind) {
            var queue = new Queue<Node<Map>>();
            var visited = new List<Node<Map>>();
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

        public void AddNeighbor(Map addTo, Node<Map> toAdd) {
            var origin = Find(addTo);
            if (origin == null || origin.neighbors.Contains(toAdd)) return; // Unlikely
            origin.AddNeighbor(toAdd);
        }

        public Stack<Node<Map>> FindShortest(Map target) {
            var stack = new Stack<Node<Map>>();
            var visited = new List<Node<Map>>();
            stack.Push(root);
            Iterate(stack, visited, target);
            return stack;
        }
        bool Iterate(Stack<Node<Map>> stack, List<Node<Map>> visited, Map target) {
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
