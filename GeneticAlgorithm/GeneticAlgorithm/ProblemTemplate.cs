using System;
using System.Collections.Generic;
using System.Text;

namespace GeneticAlgorithm
{
    class ProblemTemplate : IProblemTemplate
    {
        public int Width => 8;

        public int Height =>8;

        public int StartX => 0;

        public int StartY => 7;

        public int EndX => 7;

        public int EndY => 0;

        public IEnumerable<MapWall> GetWalls()
        {
            return new MapWall[]
            {
                new MapWall(1, 2, 0, 0),
                new MapWall(4, 5, 0, 0),
                new MapWall(2, 3, 1, 1),
                new MapWall(5, 6, 1, 1),
                new MapWall(3, 4, 2, 2),
                new MapWall(6, 7, 2, 2),
                new MapWall(0, 1, 3, 3),
                new MapWall(1, 2, 3, 3),
                new MapWall(2, 3, 3, 3),
                new MapWall(1, 2, 4, 4),
                new MapWall(2, 3, 4, 4),
                new MapWall(5, 6, 4, 4),
                new MapWall(6, 7, 4, 4),
                new MapWall(0, 1, 6, 6),
                new MapWall(2, 3, 6, 6),
                new MapWall(4, 5, 6, 6),
                new MapWall(5, 6, 6, 6),
                new MapWall(6, 7, 7, 7),

                new MapWall(1, 1, 0, 1),
                new MapWall(1, 1, 2, 3),
                new MapWall(2, 2, 1, 2),
                new MapWall(2, 2, 5, 6),
                new MapWall(3, 3, 4, 5),
                new MapWall(4, 4, 1, 2),
                new MapWall(4, 4, 3, 4),
                new MapWall(4, 4, 6, 7),
                new MapWall(5, 5, 4, 5),
                new MapWall(6, 6, 5, 6),
                new MapWall(7, 7, 2, 3),
            };

        }

    }
}
