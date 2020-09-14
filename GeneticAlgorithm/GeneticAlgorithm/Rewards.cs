using System;
using System.Collections.Generic;
using System.Text;

namespace GeneticAlgorithm
{
    public static class Rewards
    {
        public const int VOID = -1000;
        public const int NORMALSPACE = 0;
        public const int OBSTACLE = -50;
        public const int GOINGBACK = -50;
        public const int GOAL = 10;
    }
}
