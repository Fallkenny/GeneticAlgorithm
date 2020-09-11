using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeneticAlgorithm
{
    class MazeProblem
    {
        public int Fitness(Individual individual)
        {


            return 0;
        }



        public MazeProblem(int mapWidth, int mapHeight)
        {
            this.MapWidth = mapWidth;
            this.MapHeight = mapHeight;
            this.Map = new MapSpace[mapWidth, mapHeight];

            this.PopulateMapRewards();
        }

        public int MapHeight { get; set; }

        public int MapWidth { get; set; }

        public MapSpace StartPosition { get; set; }

        //public MapSpace EndPosition { get; set; }

        public MapSpace[,] Map { get; set; }


        private IEnumerable<MapWall> GetWalls()
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
        public void PopulateMapWalls()
        {
            int index = 1;
            for (int j = 0; j < MapHeight; j++)
                for (int i = 0; i < MapWidth; i++)
                    Map[i, j] = new MapSpace(Rewards.NORMALSPACE, index++, i, j);



            Map[7, 0].Reward = Rewards.GOAL;
            this.StartPosition = Map[1, 8];


            var walls = GetWalls();

            for (int j = 0; j < MapHeight; j++)
                for (int i = 0; i < MapWidth; i++)
                {
                    var mapSpace = Map[i, j];
                    var coordinateWalls = walls.Where(wall => wall.XFrom == i || wall.XTo == i || wall.YFrom == j || wall.YTo == j);

                    foreach (var wall in coordinateWalls.Where(w => w.VerticalWall()))
                    {
                        //set walls
                    }


                }

            //for (int y = 5; y < 10; y++)
            //    for (int x = 0; x < 4; x++)
            //        Map[x, y].Reward = Rewards.VOID;

            //for (int x = 8; x < 12; x++)
            //    for (int y = 5; y < 10; y++)
            //        Map[x, y].Reward = Rewards.VOID;
        }
    }
}
