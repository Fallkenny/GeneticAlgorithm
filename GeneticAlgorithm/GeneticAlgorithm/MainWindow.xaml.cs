﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        DispatcherTimer _timer;

        public MainWindow()
        {
            _timer = new DispatcherTimer();
            _timer.Tick += new EventHandler(timer_Tick);
            _timer.Interval = new TimeSpan(1);
            _timer.Start();
            InitializeComponent();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            //_timer.Interval = new TimeSpan((int.Parse((_comboTime.SelectedItem as ComboBoxItem).Tag as string)));

            if (_algorithm == null)
            {
                _algorithm = new GenAlgorithm();
                _algorithm.Initialize();
                this.MapViewer.DrawMap(_algorithm.Maze.Map, _algorithm.Maze.MapWidth, _algorithm.Maze.MapHeight, _algorithm.CurrentState.Id);
            }

            //if (!_canRun)
            //    return;

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
    }
}
