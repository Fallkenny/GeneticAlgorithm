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
        public static eMovement ToMovement(this string movementGene)
        {
            switch (movementGene)
            {
                case "00": return eMovement.Right;
                case "01": return eMovement.Up;
                case "10": return eMovement.Left;
                case "11":
                default:
                    return eMovement.Down;
            }
        }

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
