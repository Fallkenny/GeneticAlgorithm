using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeneticAlgorithm
{
    public class Individual
    {
        public Individual()
        {

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

                if (random.NextDouble() <= mutationRate)
                {
                    swapPosition = random.Next(0, genes.Length);
                    bit = genes[swapPosition] == '0' ? '1' : '0';
                    array[swapPosition] = bit;
                }
                genes = new string(array);
            }
            this.Genes = genes;
        }

        public int Fitness { get; set; } = 0;

        public string Genes { get; set; }

        public Dictionary<int, List<int>> StepById { get; set; } = new Dictionary<int, List<int>>();

        public Individual(string genes)
        {
            this.Genes = genes;
        }

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

        public void PunishRepeatedSteps()
        {
            var repeatedSpaces = this.StepById.Values.Where(step => step.Count > 1);
            foreach (var repeatedSpace in repeatedSpaces)
                this.Fitness -= 2 * (repeatedSpace.Last() - repeatedSpace.First());
        }

        internal void AddStep(int spaceID, int stepNumber)
        {
            if (!this.StepById.ContainsKey(spaceID))
                this.StepById.Add(spaceID, new List<int>() { stepNumber });
            else
                this.StepById[spaceID].Add(stepNumber);
        }

        internal static Individual Random(int individualSize)
        {
            var bits = "01";
            var genesString = "";
            while (individualSize-- > 0)
                genesString += bits.OrderBy(guid => Guid.NewGuid()).First();

            return new Individual(genesString) { Fitness = int.MinValue };
        }
    }
}
