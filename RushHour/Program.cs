using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;


namespace RushHour {
    class Program {
        #region static variables
        static bool outputMode;
        static Point targetLocation;
        static Map map;

        static SpinLock QCLOCK = new SpinLock();

        static Tree tree;
        static List<ConcurrentQueue<Tuple<Map, char>>> queues = new List<ConcurrentQueue<Tuple<Map, char>>>();
        static List<IntHelp> workersOnLvl = new List<IntHelp>();
        static int currQue = 0;

        private const int NumTasks = 10000;
        #endregion

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

            // Initialize the search setup
            tree = new Tree(map);
            queues.Add(new ConcurrentQueue<Tuple<Map, char>>());
            queues[0].Enqueue(new Tuple<Map, char>(map, '.'));
            workersOnLvl.Add(new IntHelp(0));

            // And the crusaders to search
            Task[] tasks = new Task[NumTasks];
            for (int i = 0; i < NumTasks; i++)
            {
                int n = i;  //[c# multithreaded bug]
                tasks[i] = Task.Factory.StartNew(() => worker(n));
            }
            Task.WaitAll(tasks); // Wait for all crusaders to report

            if (Globals.Solution != Globals.NoSolutions) { //we found a solution
                if (outputMode) Console.WriteLine(tree.Find(Globals.Solution).moves);
                else Console.WriteLine(tree.Find(Globals.Solution).depth);
            }
            else { // No solutions
                if (outputMode) Console.WriteLine("Geen oplossing gevonden");
                else Console.WriteLine("-1");
            }
            Console.ReadLine();
        }

        private static void worker(int tasknr) //a concurrent worker
        {
            Map solution = null; 
            while (solution == null) // As long as we don't have a solution nor confirmation that no solutions exist, keep searching
            {
                bool qcref = false;
                QCLOCK.Enter(ref qcref);
                    if (!workToDo(currQue)) { solution = Globals.NoSolutions; QCLOCK.Exit(); break; } //No more work to do, so we finish up here
                    if (queues.Count <= currQue + 1) { queues.Add(new ConcurrentQueue<Tuple<Map, char>>()); workersOnLvl.Add(new IntHelp(0)); } //We don't want to run out of queues...
                    if (workFin(currQue)) { currQue++; } //One Queue done, next one!
                    Tuple<Map, char> var;
                    if (queues[currQue].TryDequeue(out var)) solution = Iterate(var, currQue, tree); //We found some work, let's do that work
                    if (QCLOCK.IsHeldByCurrentThread) { QCLOCK.Exit(); } //If we have the lock we need to drop it here [Iterate drops the lock allready]
            }
            if (solution != Globals.NoSolutions)
                Globals.Solution = solution;
        }

        /// <summary>
        /// Generate permutations for a given Rush Hour configuration
        /// </summary>
        /// <param name="var">The configuration and last moved car to permute</param>
        /// <param name="workOnQue">The queue on which we are working</param>
        /// <param name="tree">The tree in which new nodes have to be inserted</param>
        /// <returns>Null if no valid solutions are found, the winning configuration if found</returns>
        private static Map Iterate(Tuple<Map, char> var, int workOnQue, Tree tree)
        {
            Interlocked.Increment(ref workersOnLvl[workOnQue].value); //we start working on this height
                QCLOCK.Exit();  //we drop the lock here to pass currQue safely (could have been done differently, I know...)
            var currentMap = var.Item1;
            var cars = currentMap.Parse();
            if (cars.ContainsKey(Globals.TargetCar) && cars[Globals.TargetCar].Item1.Equals(targetLocation)) 
                return currentMap; // The target car is at the right place so this must be the winning move.
            foreach (var kvp in cars)
                if (kvp.Key != var.Item2) { // Don't move the car we already moved, it will only generate already existing moves that aren't closer to the root
                    Map move;
                    bool horizontal = kvp.Value.Item3 == Direction.Right;
                    //all moves to the left or above the car
                    for (int i = 1; i <= (horizontal ? kvp.Value.Item1.X : kvp.Value.Item1.Y); i++) {
                        move = currentMap.makeMove(kvp.Key, kvp.Value.Item1, kvp.Value.Item3.Invert(), kvp.Value.Item2, i);
                        if (move != null) MoveCheck(workOnQue, tree, currentMap, kvp, kvp.Value.Item3.Invert(), i, move);
                        else break;
                    }
                    //all moves to the right or below the car
                    for (int i = 1; i < (horizontal ? map.map.GetLength(0) - kvp.Value.Item1.X : map.map.GetLength(1) - kvp.Value.Item1.Y); i++) {
                        move = currentMap.makeMove(kvp.Key, kvp.Value.Item1, kvp.Value.Item3, kvp.Value.Item2, i);
                        if (move != null) MoveCheck(workOnQue, tree, currentMap, kvp, kvp.Value.Item3, i, move);
                        else break;
                    }
                }
            Interlocked.Decrement(ref workersOnLvl[workOnQue].value); //we do not work on this height anymore
            return null; // no solution found yet
        }

        private static bool workToDo(int workOnQue) //is there still work left? (either to be done or being done)
        {
            if (queues.Count > workOnQue + 1)       
            {
                if (workersOnLvl[workOnQue+1].value > 0) return true;
                else if (!queues[workOnQue+1].IsEmpty) return true;
            }
            if (workersOnLvl[workOnQue].value > 0) return true;
            else if (!queues[workOnQue].IsEmpty) return true;
            return false;
        }

        private static bool workFin(int workOnQue) //can we proceed to the next layer? i.e.: is the previous layer done and, if so, is there work to be done on this layer left?
        {
            if (!queues[workOnQue].IsEmpty) return false;
            if (workOnQue > 0 && workersOnLvl[workOnQue - 1].value > 0) return false;
            return true; //voorgaande laag is afgewerkt en degene waar je op staat is accounted for door andere threads -> volgende laag!
        }

        private static void MoveCheck(int workOnQue, Tree tree, Map currentMap, KeyValuePair<char, Tuple<Point, int, Direction>> kvp, Direction d, int n, Map move) {
            Tuple<char, Direction, int> tuple = new Tuple<char, Direction, int>(kvp.Key, d, n);
            var moveNode = tree.Find(move); //we check if the move is allready present, if it isn't we are going to add it
            if (moveNode == null) {
                tree.Add(currentMap, move, tuple); //we add the new board to the dictionary
                if (queues.Count <= workOnQue + 1) { queues.Add(new ConcurrentQueue<Tuple<Map, char>>()); workersOnLvl.Add(new IntHelp(0)); } //We don't want to run out of queues...
                queues[workOnQue+1].Enqueue(new Tuple<Map, char>(move, kvp.Key)); //we add the new board to the next queue
            }
            else tree.rehangNeighbors(currentMap, moveNode, tuple, false); //maybe there was a shorter way to get here?
        }
    }
}
