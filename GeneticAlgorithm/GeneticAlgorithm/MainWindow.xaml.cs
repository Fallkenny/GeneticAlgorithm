using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
            _timer.Interval = TimeSpan.FromSeconds(0.05);//new TimeSpan((int.Parse((_comboTime.SelectedItem as ComboBoxItem).Tag as string)));
            _timer.Interval = new TimeSpan(1);//new TimeSpan((int.Parse((_comboTime.SelectedItem as ComboBoxItem).Tag as string)));

            if (!_canRun)
                return;

            if (_algorithm == null)
            {
                if (int.TryParse(this.txtIndividuals.Text, out int individuals) && individuals > 0)
                {
                    this.txtIndividuals.IsEnabled = false;
                    _algorithm = new GenAlgorithm(individuals);

                    _algorithm.Initialize();
                    this.MapViewer.DrawMap(_algorithm.Maze.Map, _algorithm.Maze.MapWidth, _algorithm.Maze.MapHeight, _algorithm.CurrentState.Id);
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
            //this._episodeLabel.Content = $"Episódio: {_algorithm.Episodes.ToString() }";
            //this._movesLabel.Content = $"Movimentos: {_algorithm.Moves.ToString() }";
            //var path = _algorithm.BestPath;
            //
            //this._bestPathLabel.Content = string.Empty;
            //for (int i = 0; i < path.Length; i++)
            //{
            //    if (i == 15)
            //        this._bestPathLabel.Content += "\n";
            //    this._bestPathLabel.Content += $"{path[i]}";
            //}
            //if (_algorithm.ReachedGlobalMaximum)
            //    this._bestPathLabel.Foreground = Brushes.Green;
            //else
            //    this._bestPathLabel.Foreground = Brushes.Black;

            _algorithm.RunIteration();
            this.MapViewer.DrawMap(_algorithm.Maze.Map, _algorithm.Maze.MapWidth, _algorithm.Maze.MapHeight, _algorithm.CurrentState.Id);
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
