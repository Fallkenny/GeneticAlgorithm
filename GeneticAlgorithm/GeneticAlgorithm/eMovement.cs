using System;
using System.Collections.Generic;
using System.Text;

namespace GeneticAlgorithm
{
    public enum eMovement
    {
        Left,
        Right,
        Up,
        Down
    }

    public static class MovementExtension
    {
        public static string Arrow(this eMovement eMovement)
        {
            //↑↓→←
            switch (eMovement)
            {
                case eMovement.Down:
                    return "↓";
                case eMovement.Up:
                    return "↑";
                case eMovement.Left:
                    return "←";
                case eMovement.Right:
                default:
                    return "→";
            }
        }
    }
}
