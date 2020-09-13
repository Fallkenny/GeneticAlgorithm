using System;
using System.Collections.Generic;
using System.Text;

namespace GeneticAlgorithm
{
    public static class Rewards
    {
        public const int VOID = -1000;
        public const int NORMALSPACE = 0;
        public const int OBSTACLE = -45;
        public const int GOINGBACK = -45;
        public const int GOAL = 10;
    }
}
