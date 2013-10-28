using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RushHour {
    static class Globals {
        public static char EmptyTile = '.';
        public static char TargetCar = 'x';
        public static Map NoSolutions = new Map(new string[] { "" });

        public static int GetX(this Direction d) {
            if (d == Direction.Left)    return -1;
            if (d == Direction.Right)   return 1;
                                        return 0;
        }

        public static int GetY(this Direction d) {
            if (d == Direction.Up)      return -1;
            if (d == Direction.Down)    return 1;
                                        return 0;
        }

        public static Direction Invert(this Direction d) {
            switch (d) {
                case Direction.Down  : return Direction.Up;
                case Direction.Up    : return Direction.Down;
                case Direction.Left  : return Direction.Right;
                case Direction.Right : return Direction.Left;
                default              : return Direction.Default;
            }
        }
    }
}
