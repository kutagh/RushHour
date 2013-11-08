﻿using System;
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

        static SpinLock DLOCK = new SpinLock(); //DLOCK locks the dictionary, to prevent two threads from trying to add or rehang the same nodes at the same time

        public Tree(Map initialConfiguration) {
            root = new Node(initialConfiguration,0);
            mapDict.Add(initialConfiguration, root);
        }

        /// <summary>
        /// Find the node for a certain map configuration
        /// </summary>
        /// <param name="toFind">The configuration that we want to find</param>
        /// <returns>A node if it already exists or null</returns>
        public Node Find(Map toFind) {
            if (mapDict.ContainsKey(toFind))
                return mapDict[toFind];
            return null;
        }

        /// <summary>
        /// Add a new map configuration to the tree
        /// </summary>
        /// <param name="addto">The parent configuration to add it to</param>
        /// <param name="toAdd">The new configuration to add</param>
        /// <param name="tuple">The move that was just made</param>
        public void Add(Map addto, Map toAdd, Tuple<char,Direction,int> tuple)
        {
            bool dref = false;
            DLOCK.Enter(ref dref);
                if (!mapDict.ContainsKey(toAdd)) //yes we just did this already, but now we are inside of a lock so we do it safely here (we did it outside first for some speed up)
                {
                    var origin = Find(addto);
                    if (origin == null) throw new Exception(); //something has seriously gone wrong...
                    mapDict.Add(toAdd, new Node(toAdd, origin.depth + 1, origin, tuple));
                }
                else rehangNeighbors(toAdd, Find(addto), true);
            DLOCK.Exit();
        }

        /// <summary>
        /// Reposition the node in the tree if necessary
        /// </summary>
        /// <param name="key">The parent configuration</param>
        /// <param name="existingLoc">The node to reposition</param>
        /// <param name="tuple">The move we just made</param>
        /// <param name="haveLock">Whether we need to get the lock again</param>
        public void rehangNeighbors(Map oldParent, Node<Map> movingNode, bool haveLock) //relocate a node to a higher point in the tree
        {
            Node<Map> oldParentNode = Find(oldParent);
            if (oldParentNode.depth < movingNode.depth - 1)
            {
                if (!haveLock) { bool dref = false; DLOCK.Enter(ref dref); } //if we came from Add(), we don't need the lock again.

                var prevParent = movingNode.parent;        //the previous parent
                movingNode.parent = oldParentNode;         //new parent
                movingNode.depth = oldParentNode.depth + 1;//new depth (get from new parent)
                Tuple<char, Direction, int> tuple = GetMoveMade(oldParent, movingNode.value);
                movingNode.moves = oldParentNode.moves + tuple.Item1 + Globals.ToString(tuple.Item2) + tuple.Item3.ToString() + " "; //rewrite the path

                if (!haveLock) DLOCK.Exit(); //if we came from Add(), we don't want to lose our dictionary lock
            }
        }

        public static Tuple<char, Direction, int> GetMoveMade(Map origin, Map after)    //if we rehang, our most recent move is incorrect, let's get the correct one then
        {
            var originCars = origin.Parse();
            var afterCars = after.Parse();

            foreach (var key in originCars.Keys)
            {
                var origPos = originCars[key].Item1;
                var afterPos = afterCars[key].Item1;
                if (origPos.Equals(afterPos))
                    continue;
                var dir = (origPos.X > afterPos.X || origPos.Y > afterPos.Y) ? originCars[key].Item3 : originCars[key].Item3.Invert();
                var dist = (origPos.X != afterPos.X) ? origPos.X - afterPos.X : origPos.Y - afterPos.Y;
                if (dist < 0) dist *= -1;

                return new Tuple<char, Direction, int>(key, dir, dist);
            }
            return null;
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
