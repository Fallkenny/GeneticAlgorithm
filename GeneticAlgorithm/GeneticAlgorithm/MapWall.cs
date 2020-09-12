namespace GeneticAlgorithm
{
    public class MapWall
    {
        public int XFrom { get; set; }
        public int XTo { get; set; }
        public int YFrom { get; set; }
        public int YTo { get;set;}

        public MapWall(int xFrom, int xTo, int yFrom, int yTo)
        {
            this.XFrom = xFrom;
            this.XTo = xTo;
            this.YFrom = yFrom;
            this.YTo = yTo;
        }

        public bool VerticalWall() => YFrom == YTo;
        public bool HorizontalWall() => XFrom == XTo;
    }
}