using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace GeneticAlgorithm
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        GenAlgorithm _algorithm;
        private bool _canRun;
        DispatcherTimer _timer;

        public MainWindow()
        {
            _timer = new DispatcherTimer();
            _timer.Tick += new EventHandler(timer_Tick);
            _timer.Interval = new TimeSpan(5000);
            _timer.Start();
            InitializeComponent();
            this.txtCrossoverRate.Text = GenAlgorithm.DEFAULT_CROSSOVER_RATE.ToString();
            this.txtMutationRate.Text = GenAlgorithm.DEFAULT_MUTATION_RATE.ToString();
            this.chkElitism.IsChecked = true;
            this.txtIndividuals.Text = "100";
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            _timer.Interval = new TimeSpan(1);

            if (!_canRun)
                return;

            if (_algorithm == null)
            {
                if (int.TryParse(this.txtIndividuals.Text, out int individuals) && individuals > 0)
                {
                    this.txtIndividuals.IsEnabled = false;
                    _algorithm = new GenAlgorithm(individuals);

                    _algorithm.Initialize(new ProblemTemplate());
                    this.MapViewer.DrawMap(_algorithm.Maze.Map, _algorithm.Maze.MapWidth, _algorithm.Maze.MapHeight, _algorithm.CurrentState.Id, 
                        _algorithm.CurrentBestIndividual?.StepById?.Keys?.Concat(new int[] { _algorithm.Maze.StartPosition.Id }).ToList());
                }
            }
            _algorithm.Elitism = this.chkElitism.IsChecked ?? false;
            _algorithm.CrossoverRate = float.Parse(this.txtCrossoverRate.Text.Replace(".", ","));
            _algorithm.MutationRate = float.Parse(this.txtMutationRate.Text.Replace(".", ","));
            this.lblGeneration.Content = $"Geração: {_algorithm.GenerationCount}";

            this.lblBestIndividual.Content = _algorithm.CurrentBestIndividual is null ? string.Empty :
                                             $"Melhor individuo:{ _algorithm.CurrentBestIndividual.ToDirectionString()}\n" +
                                             $"Genes: {_algorithm.CurrentBestIndividual.Genes}\n" +
                                             $"Fitness: {_algorithm.CurrentBestIndividual.Fitness}";

            this.lblCurrentIndividual.Content = _algorithm.CurrentIndividual is null ? string.Empty :
                                                $"Individuo Atual:\n{ _algorithm.CurrentIndividual.ToDirectionString()}\n" +
                                                $"{_algorithm.CurrentIndividual.Genes}\n";


            _algorithm.RunIteration();
            this.MapViewer.DrawMap(_algorithm.Maze.Map, _algorithm.Maze.MapWidth, _algorithm.Maze.MapHeight, _algorithm.CurrentState.Id, 
                _algorithm.CurrentBestIndividual?.StepById?.Keys?.Concat(new int[] { _algorithm.Maze.StartPosition.Id }).ToList());
        }

        private void txtCrossoverRate_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9.,-]+");
        }

        private void txtMutationRate_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9.,-]+");
        }

        private void play_Click(object sender, RoutedEventArgs e)
        {
            this._canRun = true;
        }

        private void pause_Click(object sender, RoutedEventArgs e)
        {
            this._canRun = false;
        }

        private void restart_Click(object sender, RoutedEventArgs e)
        {
            this.txtIndividuals.IsEnabled = true;
            this._algorithm = null;
            this._canRun = false;
        }

        private void txtIndividuals_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9-]+");
        }
    }
}
