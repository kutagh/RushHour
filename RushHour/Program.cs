using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace RushHour {
    class Program {
        static bool outputMode;
        static Point targetLocation;
        static Map map;
        static void Main(string[] args) {
            #region Parse input
            outputMode           = Console.ReadLine() == "0" ? false : true;
            int height           = int.Parse(Console.ReadLine());
            int targetY          = int.Parse(Console.ReadLine());
            int targetX          = int.Parse(Console.ReadLine());
            targetLocation       = new Point() { X = targetX, Y = targetY };
            var initialConfig = new string[height];
            for (int i = 0; i < height; i++) 
                initialConfig[i] = Console.ReadLine();
            map = new Map(initialConfig);
            #endregion

            // Here goes parallel search for the holy grail
            var tree = new Tree(map);
            
            // testing
            // var move1 = map.makeMove(Globals.targetCar, Direction.Down, 1);
            // var move2 = map.makeMove(Globals.targetCar, Direction.Down, 2).makeMove(Globals.targetCar, Direction.Up, 1);
            // tree.AddNeighbor(map, move1);
            // var test = tree.Find(move2);
            // Console.WriteLine("It does {0}work", test == null ? "not " : "");

            var queue = new ConcurrentQueue<Tuple<Map, char>>();
            queue.Enqueue(new Tuple<Map, char>(map, '.'));
            Map solution = null;
            while (solution == null) solution = Iterate(queue, tree);
            if (solution != Globals.NoSolutions) {
                // Winning solution inside
                Console.WriteLine("We found a winning solution");
                Console.WriteLine(solution.ToString());
                Console.WriteLine("Testing shortest path lookup");
                var stack = tree.FindShortest(solution);
                while (stack.Count > 0)
                    Console.WriteLine(stack.Pop().value);
            }
            else {
                // No solutions
                Console.WriteLine("We did not find a winning solution");
            }

            // Console.WriteLine("Tree debugging");
            // var treeQueue = new Queue<Node<Map>>();
            // var visited = new List<Node<Map>>();
            // treeQueue.Enqueue(tree.root);
            // using (var writer = new StreamWriter("Tree.txt")) {
            //     while (treeQueue.Count > 0) {
            //         var current = treeQueue.Dequeue();
            //         visited.Add(current);
            //         writer.WriteLine(current.value);
            //         foreach (var nb in current.neighbors.Where(x => !visited.Contains(x))) treeQueue.Enqueue(nb);
            //     }
            // }
            //Iterate(ref queue);
            //Console.WriteLine(queue.Count());
            //foreach (var m in queue) {
            //    Console.WriteLine("Permutation of input:");
            //    Console.WriteLine(m.Item1.ToString());
            //}
            // Testing input parsing
            while (true) {
                Console.WriteLine(map.ToString());
                Console.WriteLine("Please enter a command:");
                var input = Console.ReadLine();
                if (input.StartsWith("M")) {
                    Direction dir = Direction.Default;
                    switch(input[4]){
                        case 'U': 
                            dir = Direction.Up;
                            break;
                        case 'D': 
                            dir = Direction.Down;
                            break;
                        case 'L':
                            dir = Direction.Left;
                            break;
                        case 'R':
                            dir = Direction.Right;
                            break;
                    }
                    var temp = map.makeMove(input[2], dir, int.Parse(input[6].ToString()));
                    if (temp != null) map = temp;

                    continue;
                }

                if (input.StartsWith("L")) {
                    foreach (var kvp in map.Parse()) 
                        Console.WriteLine("Car {0}'s top-left is at {1}, with a {3} orientation and length {2}.", kvp.Key, kvp.Value.Item1, kvp.Value.Item2, kvp.Value.Item3 == Direction.Down ? "vertical" : "horizontal");
                    continue;
                }

                if (input.StartsWith("H")) {
                    Console.WriteLine(map.GetHashCode());
                }
            }
        }

        private static Map Iterate(ConcurrentQueue<Tuple<Map, char>> queue, Tree tree) {
            Tuple<Map, char> var;
            while (!queue.TryDequeue(out var)) System.Threading.Thread.Sleep(5);
            var currentMap = var.Item1;
            var cars = currentMap.Parse();
          Console.WriteLine("Checking:\n" + currentMap);
            if (cars.ContainsKey(Globals.TargetCar) && cars[Globals.TargetCar].Item1.Equals(targetLocation)) 
                return currentMap;
            foreach (var kvp in cars)
                if (kvp.Key != var.Item2) {
                    Map move;
                    bool horizontal = kvp.Value.Item3 == Direction.Right;
                    //all moves to the left or above the car
                    for (int i = 1; i <= (horizontal ? kvp.Value.Item1.X : kvp.Value.Item1.Y); i++) {
                        move = currentMap.makeMove(kvp.Key, kvp.Value.Item1, kvp.Value.Item3.Invert(), kvp.Value.Item2, i);
                        if (move != null) {
<<<<<<< HEAD
                            NewMethod(queue, tree, currentMap, kvp, move);
=======
                            var moveNode = tree.Find(move);
                            if (moveNode == null)               //this specific permutation of the board hasn't been found yet
                            {
                                tree.AddNeighbor(currentMap, move);
                                queue.Enqueue(new Tuple<Map, char>(move, kvp.Key));
                                Console.WriteLine("Queued:\n" + move);
                            }
                            else tree.rehangNeighbors(currentMap, moveNode);    //we have allready seen this permutation, maybe rehang some nodes?
>>>>>>> work
                        }
                        else break;
                    }
                    //all moves to the right or below the car
                    for (int i = 1; i < (horizontal ? map.map.GetLength(0) - kvp.Value.Item1.X : map.map.GetLength(1) - kvp.Value.Item1.Y); i++) {
                        move = currentMap.makeMove(kvp.Key, kvp.Value.Item1, kvp.Value.Item3, kvp.Value.Item2, i);
                        if (move != null) {
<<<<<<< HEAD
                            NewMethod(queue, tree, currentMap, kvp, move);
=======
                            var moveNode = tree.Find(move);
                            if (moveNode == null) {
                                tree.AddNeighbor(currentMap, move);
                                queue.Enqueue(new Tuple<Map, char>(move, kvp.Key));
                              Console.WriteLine("Queued:\n" + move);
                            }
                            else tree.rehangNeighbors(currentMap, moveNode);
>>>>>>> work
                        }
                        else break;
                    }
                }
            if (queue.Count == 0) return Globals.NoSolutions; // We don't have anything to add
            return null; // no solution found yet
        }

        private static void NewMethod(ConcurrentQueue<Tuple<Map, char>> queue, Tree tree, Map currentMap, KeyValuePair<char, Tuple<Point, int, Direction>> kvp, Map move) {
            var moveNode = tree.Find(move);
            if (moveNode == null) {
                tree.AddNeighbor(currentMap, move);
                queue.Enqueue(new Tuple<Map, char>(move, kvp.Key));
                //Console.WriteLine("Queued:\n" + move);
            }
            else ;// tree.AddNeighbor(currentMap, moveNode);
        }
    }
}

/*
 * Concept plan:
 * Een soort Breadth First Search op de moves die vanuit de huidige positie kan worden gedaan, zie ook:
 * http://citeseerx.ist.psu.edu/viewdoc/download?doi=10.1.1.151.126&rep=rep1&type=pdf
 * Dus per iteratie:
 * Haal een configuratie uit een concurrent queue (priority queue om hem meer breadth first search te maken?)
 * Voor elke auto die niet als laatste is verplaatst, genereer een permutatie van de configuratie voor elke move dat je met de auto kunt maken
 * Genereer een hash van elke permutatie, controleer of de hash al bestaat in de concurrent dictionary.
 * - Bestaat hij niet? Dan creeer je eerst een nieuwe node in de tree met die configuratie, voeg je de verwijzing toe aan de dictionary onder de hash
 * Vervolgens pak je de node van de huidige configuratie, voeg je een connectie toe naar de node van de gecreeerde permutatie.
 * Als de hash niet bestond, voeg de nieuwe configuratie toe aan de concurrent queue.
 * 
 * Iteraties kunnen parallel worden gedaan, zolang de queue niet leeg is.
 * 
 * Benodigdheden:
 * - Map structuur (af)
 * - Parser van map (af)
 * - Permutatie van map generator (af)
 * - Concurrent boom, d.w.z. boom waar je kunt traversen, nodes aanmaken en connecties maken. Connecties maken moet concurrent werken.
 * - Paralleliseren
 * 
 * Optioneel:
 * - Concurrent priority queue maken
 * - Alternatief voor concurrent dictionary bedenken
 */
