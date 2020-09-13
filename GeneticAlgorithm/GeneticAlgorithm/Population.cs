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
            this.Individuals = new List<Individual>(size);
        }

        internal void Order()
        {
            this.Individuals = Individuals.Take(Size).ToList();
            this.Individuals = this.Individuals.OrderByDescending(individual => individual.Fitness).ToList();
        }

        internal void AddRange(IEnumerable<Individual> enumerable)
        {
            foreach (var individual in enumerable)
                this.Add(individual);
        }

        internal void Add(Individual individual)
        {
            if (Individuals.Count < Size)
                Individuals.Add(individual);
        }
    }
}
