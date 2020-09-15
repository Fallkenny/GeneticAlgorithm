using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeneticAlgorithm
{
    class GenAlgorithm
    {
        private Random _random = new Random();

        public static readonly double DEFAULT_CROSSOVER_RATE = 0.75d;
        public static readonly double DEFAULT_MUTATION_RATE = 0.45d;

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
        private int _generationsWithoutEvolution = 0;

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
                Start();
                return;
            }

            if (this.AlgorithmState == eAlgorithmState.Generating)
            {
                GeneratePopulation();
                return;
            }

            if (this.AlgorithmState == eAlgorithmState.CalculatingFitness)
            {
                CalculateIndividualsFitness();
                return;
            }

            if (this.AlgorithmState == eAlgorithmState.ReachedGoal)
            {
                this.CurrentPopulation.Order();
                var newBest = CurrentPopulation.Individuals.First();
                CurrentPopulation = new Population(1) { Individuals = new List<Individual> { new Individual(newBest.Genes) } };
                CurrentBestIndividual = newBest;
                CalculateIndividualsFitness();
                return;
            }
        }
        private void Start()
        {
            this.CurrentPopulation = new Population(_populationSize);
            CreateRandomPopulation();
            this.AlgorithmState = eAlgorithmState.Generating;
            GenerationCount = 0;
        }


        private void GeneratePopulation()
        {
            this.CurrentPopulation.Order();
            var newBest = CurrentPopulation.Individuals.First();
            if (!(CurrentBestIndividual is null))
            {
                var totalDistance = CalculateEuclidianDistance(this.Maze.EndPosition.X, this.Maze.EndPosition.Y,
                    this.Maze.StartPosition.X, this.Maze.StartPosition.Y);

                var globalMaxReward = 3 * (int)totalDistance + Rewards.GOAL;
                //var globalMaxReward =  Rewards.GOAL;

                if (globalMaxReward == CurrentBestIndividual.Fitness)
                {
                    AlgorithmState = eAlgorithmState.ReachedGoal;
                    return;
                }
            }

            this.CurrentBestIndividual = newBest;
            this.CurrentPopulation = NewGeneration(this.CurrentPopulation, this.Elitism);
            this.CurrentPopulation.Order();
            GenerationCount++;
            _currentIndividualIndex = 0;
            this.CurrentState = StartState;
            this.AlgorithmState = eAlgorithmState.CalculatingFitness;
        }
        private void CalculateIndividualsFitness()
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

            return new Individual(genesString) { Fitness = int.MinValue };
        }

        private double CalculateEuclidianDistance(int x1, int y1, int x2, int y2)
            => Math.Sqrt(((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2)));

        private void MoveAndCalculateFitness(Individual individual, eMovement movement, bool isLastMovement)
        {
            individual.Fitness += this.PunishMovement(movement, out bool validMovement, out int nextX, out int nextY, isLastMovement);
            if (validMovement)
                PerformMovement(individual, movement, isLastMovement, nextX, nextY);
            else
            {
                CurrentState = StartState;
                _previousMovement = null;
                _currentEvaluationIndex = 0;
                _currentIndividualIndex++;
            }
        }

        private void PerformMovement(Individual individual, eMovement movement, bool isLastMovement, int nextX, int nextY)
        {
            if (!isLastMovement)
            {
                //var previousDistanceFromGoal = CalculateEuclidianDistance(this.Maze.EndPosition.X, this.Maze.EndPosition.Y, CurrentState.X, CurrentState.Y); ;
                //var currentDistanceFromGoal = CalculateEuclidianDistance(this.Maze.StartPosition.X, this.Maze.StartPosition.Y, nextX, nextY);

                //if (previousDistanceFromGoal < currentDistanceFromGoal)
                //    individual.Fitness -= 1;

            }

            CurrentState = Maze.Map[nextX, nextY];
            individual.Fitness += CurrentState.Reward;
            _previousMovement = movement;

            individual.AddStep(CurrentState.Id, _currentEvaluationIndex / 2);

            if (isLastMovement)
            {
                individual.PunishRepeatedSteps();

                var distanceFromGoal = CalculateEuclidianDistance(this.Maze.EndPosition.X, this.Maze.EndPosition.Y, nextX, nextY);
                individual.Fitness -= (int)(4.5 * distanceFromGoal);
                var distanceFromStart = CalculateEuclidianDistance(this.Maze.StartPosition.X, this.Maze.StartPosition.Y, nextX, nextY);
                individual.Fitness += 3 * (int)distanceFromStart;
            }
        }

        private int PunishMovement(eMovement movement, out bool validMovement, out int nextX, out int nextY, bool isLastMovement)
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
                if (!isLastMovement)
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

        public void Initialize(IProblemTemplate problemTemplate)
        {
            this.Maze = new MazeProblem(problemTemplate);
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
            var cutPoint2 = r.Next((parents.Item1.Genes.Length / 2) - 1) + parents.Item1.Genes.Length / 2;

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
