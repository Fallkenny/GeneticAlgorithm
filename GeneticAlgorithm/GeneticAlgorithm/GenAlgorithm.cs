using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeneticAlgorithm
{
    class GenAlgorithm
    {
        private Random _random = new Random();

        public static readonly double DEFAULT_CROSSOVER_RATE = 0.7d;
        public static readonly double DEFAULT_MUTATION_RATE = 0.3d;

        public float CrossoverRate { get; set; }
        public float MutationRate { get; set; }

        public eAlgorithmState AlgorithmState { get; set; }

        public MazeProblem Maze { get; set; }

        public Individual CurrentBestIndividual { get; set; }

        public MapSpace StartState { get; private set; }
        public MapSpace CurrentState { get; set; }

        public Population CurrentPopulation { get; set; }

        private int _currentIndividualIndex;
        private int _currentEvaluationIndex;
        private int _individualSize = 28;
        private int _populationSize = 100;
        private eMovement? _previousMovement;

        public int GenerationCount { get; set; }
        public bool Elitism { get; set; }
        public Individual CurrentIndividual { get; private set; }

        public GenAlgorithm(int populationSize)
        {
            this._populationSize = populationSize;
        }

        public void RunIteration()
        {
            if (this.AlgorithmState == eAlgorithmState.Start)
            {
                Initialize();
                this.CurrentPopulation = new Population(_populationSize);
                CreateRandomPopulation();
                this.AlgorithmState = eAlgorithmState.Generating;
                GenerationCount = 0;
                return;
            }

            if (this.AlgorithmState == eAlgorithmState.Generating)
            {
                this.CurrentPopulation.Order();
                this.CurrentBestIndividual = CurrentPopulation.Individuals.First();
                this.CurrentPopulation = NewGeneration(this.CurrentPopulation, this.Elitism);
                this.CurrentPopulation.Order();
                GenerationCount++;
                _currentIndividualIndex = 0;
                this.CurrentState = StartState;
                this.AlgorithmState = eAlgorithmState.CalculatingFitness;
                return;
            }

            if (this.AlgorithmState == eAlgorithmState.CalculatingFitness)
            {
                if (_currentEvaluationIndex == _individualSize)
                {
                    CurrentState = StartState;
                    _currentEvaluationIndex = 0;
                    _currentIndividualIndex++;
                    _previousMovement = null;
                    if (_currentIndividualIndex == CurrentPopulation.Size)
                    {
                        _currentIndividualIndex = 0;                        
                        this.AlgorithmState = eAlgorithmState.Generating;
                    }
                    return;
                }

                var individual = this.CurrentPopulation.Individuals.ElementAt(_currentIndividualIndex);
                CurrentIndividual = individual;
                var movementGene = individual.Genes.Substring(_currentEvaluationIndex, 2);
                _currentEvaluationIndex += 2;
                var movement = GetMovement(movementGene);
                var isLastMovement = _currentEvaluationIndex == _individualSize;
                MoveAndCalculateFitness(individual, movement, isLastMovement);
                
                if (_currentIndividualIndex == CurrentPopulation.Size)
                {
                    _previousMovement = null;
                    _currentIndividualIndex = 0;
                    this.AlgorithmState = eAlgorithmState.Generating;
                }



                return;
            }

            if (this.AlgorithmState == eAlgorithmState.ReachedGoal)
            {
                return;
            }
        }

        private void CreateRandomPopulation()
        {
            for (int i = 0; i < CurrentPopulation.Size; i++)
                CurrentPopulation.Individuals.Add(RandomIndividual(_individualSize));
        }

        private Individual RandomIndividual(int individualSize)
        {
            var bits = "01";
            var genesString = "";
            while (individualSize-- > 0)
                genesString += bits.OrderBy(guid => Guid.NewGuid()).First();

            return new Individual(genesString);
        }

        public double CalculateEuclidianDistance(int x1, int y1, int x2, int y2)
            => Math.Sqrt(((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2)));

        private void MoveAndCalculateFitness(Individual individual, eMovement movement, bool isLastMovement)
        {
            individual.Fitness += this.PunishMovement(movement, out bool validMovement, out int nextX, out int nextY);
            if (validMovement)
            {
                
                if (Maze.Map[nextX, nextY].Reward == Rewards.GOAL)
                {

                }

                CurrentState = Maze.Map[nextX, nextY];
                individual.Fitness += CurrentState.Reward;
                _previousMovement = movement;

                if(isLastMovement)
                {
                    var distanceFromGoal = CalculateEuclidianDistance(this.Maze.EndPosition.X, this.Maze.EndPosition.Y, nextX, nextY);
                    individual.Fitness -= 5 * (int)distanceFromGoal;
                    var distanceFromStart = CalculateEuclidianDistance(this.Maze.StartPosition.X, this.Maze.StartPosition.Y, nextX, nextY);
                    individual.Fitness += 3 * (int)distanceFromStart;
                    if(individual.Fitness > 25)
                    {

                    }
                }

                //if (CurrentState.Reward == Rewards.GOAL)
                //    AlgorithmState = eAlgorithmState.ReachedGoal;
            }
            else
            {
                CurrentState = StartState;
                _previousMovement = null;
                _currentEvaluationIndex = 0;
                _currentIndividualIndex++;
            }

        }

        public int PunishMovement(eMovement movement, out bool validMovement, out int nextX, out int nextY)
        {
            validMovement = true;
            nextX = CurrentState.X;
            nextY = CurrentState.Y;

            var punish = 0;
            switch (movement)
            {
                case eMovement.Down:
                    nextY++;
                    if (CurrentState.WallBottom)
                        punish += Rewards.OBSTACLE;
                    if (_previousMovement == eMovement.Up)
                        punish += Rewards.GOINGBACK;
                    break;

                case eMovement.Left:
                    nextX--;
                    if (CurrentState.WallLeft)
                        punish = Rewards.OBSTACLE;
                    if (_previousMovement == eMovement.Right)
                        punish += Rewards.GOINGBACK;
                    break;

                case eMovement.Right:
                    nextX++;
                    if (CurrentState.WallRight)
                        punish = Rewards.OBSTACLE;
                    if (_previousMovement == eMovement.Left)
                        punish += Rewards.GOINGBACK;
                    break;

                case eMovement.Up:
                default:
                    nextY--;
                    if (CurrentState.WallUp)
                        punish = Rewards.OBSTACLE;
                    if (_previousMovement == eMovement.Down)
                        punish += Rewards.GOINGBACK;
                    break;
            }

            if (nextX >= Maze.MapWidth || nextX < 0 || nextY < 0 || nextY >= Maze.MapHeight)
            {
                punish = Rewards.VOID;
                validMovement = false;
            }

            return punish;
        }

        private eMovement GetMovement(string movementGene)
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

        public void Initialize()
        {
            this.Maze = new MazeProblem(8, 8, GetWalls(), 0, 7, 7, 0);
            //this.Map = Maze.Map;
            this.StartState = Maze.StartPosition;
            this.CurrentState = Maze.StartPosition;
        }

        public Population NewGeneration(Population population, bool elitism)
        {
            var newPopulation = new Population(population.Size);

            if (elitism)
                newPopulation.Add(new Individual(population.Individuals.First().Genes));

            while (newPopulation.Individuals.Count() < newPopulation.Size)
            {
                (Individual, Individual) parents = TournamentSelection(population);

                if (_random.NextDouble() <= CrossoverRate)
                    newPopulation.Individuals.AddRange(Crossover(parents));
                else
                {
                    newPopulation.Individuals.Add(new Individual(parents.Item1.Genes, MutationRate));
                    newPopulation.Individuals.Add(new Individual(parents.Item2.Genes, MutationRate));
                }
            }

            return newPopulation;
        }

        private IEnumerable<Individual> Crossover((Individual, Individual) parents)
        {
            Random r = new Random();

            var cutPoint1 = r.Next((parents.Item1.Genes.Length / 2) - 1) + 1;
            var cutPoint2 = r.Next((parents.Item1.Genes.Length / 2)) + parents.Item1.Genes.Length / 2;

            var parentGenes1 = parents.Item1.Genes;
            var parentGenes2 = parents.Item2.Genes;

            var child1Genes = $"{parentGenes1.Substring(0, cutPoint1) }" +
                             $"{parentGenes2.Substring(cutPoint1, cutPoint2 - cutPoint1)}" +
                             $"{parentGenes1.Substring(cutPoint2, parentGenes1.Length - cutPoint2)}";


            var child2Genes = $"{parentGenes2.Substring(0, cutPoint1)}" +
                             $"{parentGenes1.Substring(cutPoint1, cutPoint2 - cutPoint1)}" +
                             $"{parentGenes2.Substring(cutPoint2, parentGenes2.Length - cutPoint2)}";


            return new Individual[]
            {
                new Individual(child1Genes, MutationRate),
                new Individual(child2Genes, MutationRate)
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
