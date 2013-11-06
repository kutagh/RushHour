using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RushHour {
    static class Globals {
        public static char EmptyTile = '.';
        public static char TargetCar = 'x';
        public static Map Solution = null;
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

    class IntHelp   //helper class to fix this: "A property, indexer or dynamic member access may not be passed as an out or ref parameter."
    {
        public int value;

        public IntHelp(int n)
        {
            value = n;
        }
    }

    public class TTASLock
    {
        int AtomState = 0;                                  //an indicator used to see wether or not someone allready has the lock. (0 for free, 1 for in use)

        override public void LockIt()
        {
            while (true)
            {
                while (AtomState == 1) { }                           //we check wether or not the someone else is in there as long as someone is in there,
                if (Interlocked.Exchange(ref AtomState, 1) != 1)    //as soon as we don't see someone in there, we try to get the lock
                    return;
            }
        }

        override public void UnlockIt()
        {
            AtomState = 0;
        }
    }
}
