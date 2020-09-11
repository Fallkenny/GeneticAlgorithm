using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeneticAlgorithm
{
    class GeneticAlgorithm
    {
        private Random _random = new Random();

        public float CrossoverRate { get; set; }

        public Population NewGeneration(Population population, bool elitism)
        {
            var newPopulation = new Population(population.Size);

            if (elitism)
                newPopulation.Individuals.Add(population.Individuals.First());

            while (newPopulation.Individuals.Count() < newPopulation.Size)
            {
                (Individual, Individual) parents = TournamentSelection(population);

                if (_random.NextDouble() <= CrossoverRate)
                    newPopulation.Individuals.AddRange(Crossover(parents));
                else
                {
                    newPopulation.Individuals.Add(new Individual(parents.Item1.Genes));
                    newPopulation.Individuals.Add(new Individual(parents.Item2.Genes));
                }
            }

            newPopulation.Order();
            return newPopulation;

        }

        private IEnumerable<Individual> Crossover((Individual, Individual) parents)
        {
            Random r = new Random();

            var cutPoint1 = r.Next((parents.Item1.Genes.Length / 2) - 2) + 1;
            var cutPoint2 = r.Next((parents.Item1.Genes.Length / 2) - 2) + parents.Item1.Genes.Length / 2;

            var parentGenes1 = parents.Item1.Genes;
            var parentGenes2 = parents.Item2.Genes;

            var geneFilho1 = $"{parentGenes1.Substring(0, cutPoint1) }" +
                             $"{parentGenes2.Substring(cutPoint1, cutPoint2)}" +
                             $"{parentGenes1.Substring(cutPoint2, parentGenes1.Length)}";


            var geneFilho2 = $"{parentGenes2.Substring(0, cutPoint1)}" +
                             $"{parentGenes1.Substring(cutPoint1, cutPoint2)}" +
                             $"{parentGenes2.Substring(cutPoint2, parentGenes2.Length)}";


            return new Individual[]
            {
                new Individual(geneFilho1),
                new Individual(geneFilho2)
            };
        }

        private (Individual, Individual) TournamentSelection(Population population)
        {
            var randomIndividuals = population.Individuals
                                    .OrderBy(guid => Guid.NewGuid()).Take(3)
                                    .OrderByDescending(individual => individual.Fitness).Take(2);

            return (randomIndividuals.First(), randomIndividuals.Last());
        }
    }
}
