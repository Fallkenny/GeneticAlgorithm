using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeneticAlgorithm
{
    class Population
    {
        public List<Individual> Individuals { get; set; }
        public int Size { get; set; }
        public Population(int size)
        {
            this.Size = size;
        }

        internal void Order()
        {
            this.Individuals = this.Individuals.OrderByDescending(individual => individual.Fitness).ToList();
        }
    }
}
