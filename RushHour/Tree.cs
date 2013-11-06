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

        static SpinLock DLOCK = new SpinLock();

        public Tree(Map initialConfiguration) {
            root = new Node(initialConfiguration,0);
            mapDict.Add(initialConfiguration, root);
        }

        public Node<Map> Find(Map toFind) {
            if (mapDict.ContainsKey(toFind))
                return mapDict[toFind];
            return null;
        }

        public void Add(Map addto, Map toAdd, Tuple<char,Direction,int> tuple)
        {
            bool dref = false;
            DLOCK.Enter(ref dref);
                if (!mapDict.ContainsKey(toAdd)) //yes we just did this allready, but now we are inside of a lock so we do it safely here (we did it outside first for some speed up)
                {
                    var origin = Find(addto);
                    if (origin == null) throw new Exception(); //something has seriously gone wrong...
                    mapDict.Add(toAdd, new Node(toAdd, origin.depth + 1, origin, tuple));
                }
            DLOCK.Exit();
        }

        public void rehangNeighbors(Map key, Node<Map> existingLoc, Tuple<char, Direction, int> tuple) //relocate a node to a higher point in the tree
        {
            Node<Map> prevBoard = Find(key);
            if (prevBoard.depth < existingLoc.depth)
            {
                var prevParent = existingLoc.parent;    //the previous parent
                existingLoc.parent = prevBoard;         //new parent
                existingLoc.depth = prevBoard.depth + 1;//new depth (get from new parent)
                existingLoc.moves = prevBoard.moves + tuple.Item1 + Globals.ToString(tuple.Item2) + tuple.Item3.ToString() + " "; //rewrite the path
            }
        }
    }

    class Node<T> {
        internal T value;

        internal Node<T> parent = null;
        internal int depth;

        internal string moves;
        
        public Node(T value, int deep) {
            this.value = value;
            depth = deep;
            moves = "";
        }

        public Node(T value, int deep, Node<T> p, Tuple<char,Direction,int> tuple)
        {
            this.value = value;
            depth = deep;
            parent = p;
            moves = parent.moves + tuple.Item1 + Globals.ToString(tuple.Item2) + tuple.Item3.ToString() + " ";
        }
    }
}
