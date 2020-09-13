﻿using System;
using System.Collections.Generic;
using System.Text;

namespace GeneticAlgorithm
{
    public class Individual
    {
        public Individual()
        {

        }

        public Individual(string genes)
        {
            this.Genes = genes;
        }

        public Individual(string genes, float mutationRate)
        {
            var random = new Random();
            if (random.NextDouble() <= mutationRate)
            {
                var swapPosition = random.Next(0, genes.Length);
                var bit = genes[swapPosition] == '0' ? '1' : '0';
                var array = genes.ToCharArray();
                array[swapPosition] = bit;
                genes = new string(array);
            }
            this.Genes = genes;
        }

        public int Fitness { get; set; } = 0;

        public string Genes { get; set; }

        internal object ToDirectionString()
        {
            var stringDirections = "";
            var startIndex = 0;
            while (startIndex < Genes.Length)
            {
                var movementGene = Genes.Substring(startIndex, 2);
                switch (movementGene)
                {
                    case "00":
                        stringDirections += eMovement.Right.Arrow();
                        break;
                    case "01":
                        stringDirections += eMovement.Up.Arrow();
                        break;
                    case "10":
                        stringDirections += eMovement.Left.Arrow();
                        break;
                    case "11":
                    default:
                        stringDirections += eMovement.Down.Arrow();
                        break;
                }
                startIndex += 2;
            }
            return stringDirections;
        }
    }
}
