using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RushHour {
    class Map {
        char[,] map;
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

        public Map makeMove(char car, Direction d, int dist) {
            char[,] mapResult = new char[map.GetLength(0), map.GetLength(1)];
            Point topLeft = new Point() { X = -1, Y = -1 };
            Point defaultPoint = topLeft;
            int xLength = 0, yLength = 0;
            for (int x = 0; x < map.GetLength(0); x++)
                for (int y = 0; y < map.GetLength(1); y++)
                    if (map[x, y] != car) mapResult[x, y] = map[x, y];
                    else {
                        mapResult[x, y] = Globals.emptyTile;
                        var test = topLeft.Equals(defaultPoint);
                        if (topLeft.Equals(defaultPoint)) { topLeft = new Point() { X = x, Y = y }; xLength = 1; yLength = 1; }
                        else if (x == topLeft.X) yLength++; else xLength++;
                    }

            bool valid = true;
            var target = new Point() { X = topLeft.X + d.GetX() * dist, Y = topLeft.Y + d.GetY() * dist };
            for (int x = 0; x < xLength; x++)
                for (int y = 0; y < yLength;y++ )
                    if (mapResult[target.X + x, target.Y + y] != Globals.emptyTile) valid = false;
                    else
                        mapResult[target.X + x, target.Y + y] = car;
            
            if (valid)
                return new Map(mapResult);
            else
                return null;
        }
    }
    public enum Direction {
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
    }
}
