using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeneticAlgorithm
{
    class MazeProblem
    {
        public MazeProblem(int mapWidth, int mapHeight, IEnumerable<MapWall> walls, 
            int startX, int startY, int endX, int endY)
        {
            this.MapWidth = mapWidth;
            this.MapHeight = mapHeight;
            this.Map = new MapSpace[mapWidth, mapHeight];

            this.PopulateMapWalls(walls, startX, startY, endX,endY);
        }

        public int MapHeight { get; set; }

        public int MapWidth { get; set; }

        public MapSpace StartPosition { get; set; }

        public MapSpace EndPosition { get; set; }

        public MapSpace[,] Map { get; set; }
               
        public void PopulateMapWalls(IEnumerable<MapWall> walls, int startX, int startY, int endX, int endY)
        {
            int index = 1;
            for (int j = 0; j < MapHeight; j++)
                for (int i = 0; i < MapWidth; i++)
                    Map[i, j] = new MapSpace(Rewards.NORMALSPACE, index++, i, j);
                       
            Map[7, 0].Reward = Rewards.GOAL;
            this.StartPosition = Map[0, 7];
            this.EndPosition = Map[7, 0];

            for (int j = 0; j < MapHeight; j++)
                for (int i = 0; i < MapWidth; i++)
                {
                    var mapSpace = Map[i, j];

                    var verticalWalls = walls.Where(w => w.VerticalWall());
                    var horizontalWalls = walls.Where(w => w.HorizontalWall());

                    var coordinateWalls = walls.Where(wall => wall.XFrom == i || wall.XTo == i || wall.YFrom == j || wall.YTo == j);

                    foreach (var wall in verticalWalls.Where(w=>w.YFrom == j))
                    {
                        if(mapSpace.X == wall.XFrom)
                            mapSpace.WallRight = true;
                        if(mapSpace.X == wall.XTo)                        
                            mapSpace.WallLeft = true;
                    }

                    foreach (var wall in horizontalWalls.Where(w=>w.XFrom == i))
                    {
                        if (mapSpace.Y == wall.YFrom)
                            mapSpace.WallBottom = true;
                        if (mapSpace.Y == wall.YTo)
                            mapSpace.WallUp = true;
                    }
                }
        }
    }
}
