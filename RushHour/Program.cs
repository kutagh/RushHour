using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RushHour {
    class Program {
        static bool outputMode;
        static int targetX, targetY;
        static Map map;
        static void Main(string[] args) {
            #region Parse input
            outputMode        = Console.ReadLine() == "0" ? false : true;
            int height        = int.Parse(Console.ReadLine());
            targetX           = int.Parse(Console.ReadLine());
            targetY           = int.Parse(Console.ReadLine());
            var initialConfig = new string[height];
            for (int i = 0; i < height; i++) 
                initialConfig[i] = Console.ReadLine();
            map = new Map(initialConfig);
            #endregion
            
            // Here goes parallel search for the holy grail
            var queue = new ConcurrentQueue<Tuple<Map, char>>();
            queue.Enqueue(new Tuple<Map, char>(map, '.'));
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

        private static void Iterate(ref ConcurrentQueue<Tuple<Map, char>> queue) {
            Tuple<Map, char> var;
            while (!queue.TryDequeue(out var)) System.Threading.Thread.Sleep(5);
            var currentMap = var.Item1;
            var cars = currentMap.Parse();
            foreach (var kvp in cars)
                if (kvp.Key != var.Item2) {
                    Map move;
                    bool horizontal = kvp.Value.Item3 == Direction.Right;
                    for (int i = 1; i < (horizontal ? kvp.Value.Item1.X : kvp.Value.Item1.Y); i++) {
                        move = currentMap.makeMove(kvp.Key, kvp.Value.Item1, kvp.Value.Item3.Invert(), kvp.Value.Item2, i);
                        if (move != null)
                            queue.Enqueue(new Tuple<Map, char>(move, kvp.Key));
                    }
                    for (int i = 1; i < (horizontal ? map.map.GetLength(0) - kvp.Value.Item1.X :  map.map.GetLength(1) - kvp.Value.Item1.Y) ; i++) {
                        move = currentMap.makeMove(kvp.Key, kvp.Value.Item1, kvp.Value.Item3, kvp.Value.Item2, i);
                        if (move != null)
                            queue.Enqueue(new Tuple<Map, char>(move, kvp.Key));
                    }
                }
            

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
 * - Map structuur (redelijk af)
 * - Parser van map (af)
 * - Permutatie van map generator 
 * - Concurrent boom, d.w.z. boom waar je kunt traversen, nodes aanmaken en connecties maken. Connecties maken moet concurrent werken.
 * 
 * Optioneel:
 * - Concurrent priority queue maken
 * - Alternatief voor concurrent dictionary bedenken
 */
