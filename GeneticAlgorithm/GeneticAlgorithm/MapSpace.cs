using System;
using System.Collections.Generic;
using System.Text;

namespace GeneticAlgorithm
{
    class MapSpace
    {
        public MapSpace(int reward, int id, int x, int y)
        {
            Reward = reward;
            Id = id;
            X = x;
            Y = y;
        }

        public bool WallLeft { get; set; }
        public bool WallRight { get; set; }
        public bool WallUp { get; set; }
        public bool WallBottom { get; set; }

        public int Reward { get; set; }
        public int Id { get; }
        public int X { get; }
        public int Y { get; }
    }
}
