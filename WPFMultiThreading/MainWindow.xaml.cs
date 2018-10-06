using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace WPFMultiThreading
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BackgroundWorker bg;
        DispatcherTimer timer;
        System.Timers.Timer t;
        public MainWindow()
        {
            InitializeComponent();
            InitilaizeProgressBar();
            InitializeBackgroundWorker();
            InitializeSystemTimer();
            InitializeDispatchTimer();
        }
        private void InitilaizeProgressBar()
        {
            pb.Value = 0;
            pb.Maximum = 10;
        }
        private void InitializeBackgroundWorker()
        {
            bg = new BackgroundWorker();
            bg.DoWork += Bg_DoWork;
            bg.ProgressChanged += Bg_ProgressChanged;
            bg.RunWorkerCompleted += Bg_RunWorkerCompleted;
            bg.WorkerReportsProgress = true;
        }
        private void InitializeDispatchTimer()
        {
            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            timer.Tick += Timer_Tick;
        }

        private void InitializeSystemTimer()
        {
            t = new System.Timers.Timer(1000);
            t.Elapsed += T_Elapsed;
        }

        private void T_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                pb.Value += 1;
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() =>
                {
                    pb.Value = 0;
                    lblProgress.Content = ex.Message;
                });
            }
            finally
            {
                t.Stop();
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            pb.Value += 1;

            if (pb.Value == 10)
            {
                lblProgress.Content += "\nUpdate progress using Dispatch Timer completed!";
                timer.Stop();
            }
        }

        private void Bg_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            lblProgress.Content += "\nUpdate progress using BackgroundWorker completed!";
        }

        private void Bg_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pb.Value += 1;
        }

        private void Bg_DoWork(object sender, DoWorkEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                pb.Value = 0;
                lblProgress.Content = "Update progress using BackgroundWorker started!";
            });

            for (int i = 1; i <= 10; i++)
            {
                Thread.Sleep(1000); //do some time consuming task
                bg.ReportProgress(0);//raise background worker's progress changed event
            }

            //Dispatcher.Invoke(() =>
            //{
            //    lblProgress.Content = "\nUpdate progress using BackgroundWorker completed!";
            //});
        }

        private void BtnProgressBackgroundWorker_Click(object sender, RoutedEventArgs e)
        {
            lblProgress.Content = string.Empty;
            //Using Bakground Worker -> Progress bar value updated at each step asynchronously.
            bg.RunWorkerAsync();
        }

        private void BtnProgressDispatchTimer_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                pb.Value = 0;
                lblProgress.Content = "Update progress using Dispatch Timer started!";
            });
            timer.Start();
        }
        private void BtnProgressSyncronous_Click(object sender, RoutedEventArgs e)
        {
            pb.Value = 0;
            lblProgress.Content = "Update progress started synchronously!";

            for (int i = 1; i <= 10; i++)
            {
                Thread.Sleep(1000); //do some task
                pb.Value += 1;
            }
            lblProgress.Content += "Update progress finished synchronously!";
        }
        private void BtnProgressAsyncronous_Click(object sender, RoutedEventArgs e)
        {
            pb.Value = 0;
            lblProgress.Content = string.Empty;

            Task.Factory.StartNew(async() =>
            {
                await Dispatcher.Invoke(async() =>
                {
                    lblProgress.Content = "Update progress started asynchronously using thread!";
                    for (int i = 0; i < 10; i++)
                    {
                        await Task.Delay(1000);
                        pb.Value += 1;
                    }
                    lblProgress.Content += "\nUpdate progress completed asynchronously using thread!";
                });
            });
        }

        private void btnSystemTimer_Click(object sender, RoutedEventArgs e)
        {
            t.Start();
        }
    }
}
