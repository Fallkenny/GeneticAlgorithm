using System;
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

        public int Fitness { get; set; } = 0;

        public string Genes { get; set; }
    }
}
