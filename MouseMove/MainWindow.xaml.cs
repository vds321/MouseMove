using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Input;
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8604 // Possible null reference argument.

namespace MouseMove
{
    public partial class MainWindow
    {
        private readonly BackgroundWorker _Worker = new();

        public MainWindow()
        {
            InitializeComponent();
            Title = $"Имитация движения мыши (ver. {Application.ResourceAssembly.GetName().Version})";
            cmb_interval.ItemsSource = new List<int>() { 500, 1000, 1500, 2000 };
            cmb_interval.SelectedItem = 1000;
            rb_small.IsChecked = true;
            _Worker.WorkerSupportsCancellation = true;
            _Worker.DoWork += Worker_DoWork;
            KeyDown += MainWindow_KeyDown;
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.S:
                    {
                        if (_Worker.IsBusy) break;
                        var interval = int.Parse(cmb_interval.SelectedItem.ToString());
                        var step = 15;
                        if (rb_medium.IsChecked.GetValueOrDefault())
                        {
                            step = 50;
                        }
                        if (rb_big.IsChecked.GetValueOrDefault())
                        {
                            step = 100;
                        }

                        _Worker.RunWorkerAsync(new[] { interval, step });
                        break;
                    }
                case Key.Q:
                    if (!_Worker.IsBusy) break;
                    _Worker.CancelAsync();
                    break;
            }
        }

        private void Worker_DoWork(object? sender, DoWorkEventArgs e)
        {
            var args = e.Argument as int[];
            var interval = args[0];
            var step = args[1];
            var worker = sender as BackgroundWorker;


            while (true)
            {
                MouseOperations.MouseMove(step, step);

                Thread.Sleep(interval);
                if (worker is { CancellationPending: true })
                {
                    e.Cancel = true;
                    return;
                }

                step = -step;
            }
        }
    }
}
