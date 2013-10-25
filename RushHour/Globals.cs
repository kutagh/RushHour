using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RushHour {
    static class Globals {
        public static char emptyTile = '.';

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
    }
}
