using System;
using System.Collections.Generic;
using System.Text;

namespace GeneticAlgorithm
{
    interface IProblemTemplate
    {
        int Width { get; }
        int Height { get; }        
        int StartX { get; }
        int StartY { get; }
        int EndX { get; }
        int EndY { get; }

        IEnumerable<MapWall> GetWalls();
    }
}
