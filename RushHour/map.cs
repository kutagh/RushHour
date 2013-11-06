using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RushHour {
    class Map  {
        internal char[,] map;
        
        public Map(string[] m) {
            map = new char[m[0].Length, m.Length];
            for (int y = 0; y < m.Length; y++)
                for (int x = 0; x < m[0].Length; x++)
                    map[x, y] = m[y][x];
        }

        public Map(char[,] m) {
            map = m;
        }

        public override string ToString() {
            StringBuilder result = new StringBuilder();

            for (int y = 0; y < map.GetLength(1); y++) {
                for (int x = 0; x < map.GetLength(0); x++)
                    result.Append(map[x, y]);
                result.AppendLine();
            }

            return result.ToString();
        }

        public override int GetHashCode() { //we need this function to hash using maps
            int calc = 0;
            for (int x = 0; x < map.GetLength(0); x++)
                for (int y = 0; y < map.GetLength(1); y++)
                    calc += map[x, y] * (x + 1) * (y + 1);
            return calc;
        }

        public override bool Equals(object obj) {
            if (obj.GetType() == typeof(Map)) {
                var other = (Map) obj;
                if (map.GetLength(0) == other.map.GetLength(0) && map.GetLength(1) == other.map.GetLength(1))
                    for (int x = 0; x < map.GetLength(0); x++)
                        for (int y = 0; y < map.GetLength(1); y++)
                            if (map[x, y] != other.map[x, y])
                                return false;
                return true;
            }
            return false;
        }

        public Map makeMove(char car, Point topLeft, Direction d, int length, int dist) {
            char[,] mapResult = new char[map.GetLength(0), map.GetLength(1)];
            var xLength       = d == Direction.Right || d == Direction.Left ? length : 1;
            var yLength       = xLength > 1                                 ? 1      : length;
            for (int x = 0; x < map.GetLength(0); x++)
                for (int y = 0; y < map.GetLength(1); y++)
                    if (map[x, y] != car) mapResult[x, y] = map[x, y];
                    else                  mapResult[x, y] = Globals.EmptyTile;

            var target = new Point() { X = topLeft.X + d.GetX() * dist, Y = topLeft.Y + d.GetY() * dist };
            for (int x = 0; x < xLength; x++)
                for (int y = 0; y < yLength; y++)
                    if (target.X + x < map.GetLength(0) &&
                        target.Y + y < map.GetLength(1))
                        if (mapResult[target.X + x, target.Y + y] != Globals.EmptyTile)
                            return null;
                        else
                            mapResult[target.X + x, target.Y + y] = car;
                    else /*if(car != Globals.TargetCar)*/ return null;


            return new Map(mapResult);
        }

        public Dictionary<char, Tuple<Point, int, Direction>> Parse() { //set up a dictionary with the cars in the map
            var result = new Dictionary<char, Tuple<Point, int, Direction>>();
            for (int x = 0; x < map.GetLength(0); x++)
                for (int y = 0; y < map.GetLength(1); y++) {
                    if (map[x, y] != Globals.EmptyTile)
                        if (!result.ContainsKey(map[x, y])) {
                            var startPoint = new Point() { X = x, Y = y };
                            var dir        = Direction.Down;
                            if (x + 1 != map.GetLength(0) && map[x, y] == map[x + 1, y]) dir = Direction.Right;
                            int length = 1;
                            var nextX      = x + dir.GetX() * length;
                            var nextY      = y + dir.GetY() * length;
                            while (nextX < map.GetLength(0) && nextY < map.GetLength(1) && map[x, y] == map[nextX, nextY]) {
                                nextX      = x + dir.GetX() * length;
                                nextY      = y + dir.GetY() * length;
                                length++;
                            }
                            length--;
                            result.Add(map[x, y], new Tuple<Point, int, Direction>(startPoint, length, dir));
                        }
                }

            return result;
        }
    }
    
    public enum Direction { //directions for easier writing of code
        Default,
        Up,
        Down,
        Left,
        Right
    }
    public struct Point {
        public int X;
        public int Y;

        public override bool Equals(object obj) {
            if (obj.GetType() != typeof(Point)) return false;
            var other = (Point)obj;
            return other.X == X && other.Y == Y;
        }
        public override int GetHashCode() {
            return base.GetHashCode();
        }

        public override string ToString() {
            return string.Format("X: {0}, Y: {1}", X, Y);
        }
    }
}
