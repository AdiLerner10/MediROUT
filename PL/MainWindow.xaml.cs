using BlApi;
using PL.Courier;
using PL.Helpers;
using PL.Order;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        static int _userId;
        private readonly ObserverMutex _mutex = new();



        public int Interval
        {
            get { return (int)GetValue(IntervalProperty); }
            set { SetValue(IntervalProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Interval.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.Register("Interval", typeof(int), typeof(MainWindow), new PropertyMetadata(3400));



        public bool ModeSimulator
        {
            get { return (bool)GetValue(ModeSimulatorProperty); }
            set { SetValue(ModeSimulatorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ModeSimulator.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ModeSimulatorProperty =
            DependencyProperty.Register("ModeSimulator", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));



        public int[] endStatus
        {
            get { return (int[])GetValue(endStatusProperty); }
            set { SetValue(endStatusProperty, value); }
        }

        public static readonly DependencyProperty endStatusProperty =
            DependencyProperty.Register("endStatus", typeof(int[]), typeof(MainWindow));

        public DateTime CurrentTime
        {
            get { return (DateTime)GetValue(CurrentTimeProperty); }
            set { SetValue(CurrentTimeProperty, value); }
        }


        // Using a DependencyProperty as the backing store for CurrentTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentTimeProperty =
            DependencyProperty.Register("CurrentTime", typeof(DateTime), typeof(MainWindow));


        public BO.Config Configuration
        {
            get { return (BO.Config)GetValue(ConfigurationProperty); }
            set { SetValue(ConfigurationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Configuration.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ConfigurationProperty =
            DependencyProperty.Register("Configuration", typeof(BO.Config), typeof(MainWindow));




        public MainWindow(int userId)
        {
            _userId = userId;
            endStatus = new int[7];
            InitializeComponent();

            endStatus = s_bl.Order.OrderAmountsByStatus(userId);
        }



        private void btnAddOneMinute_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ForwardClock(BO.TimeUnit.Minute);
        }

        private void btnAddOneHour_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ForwardClock(BO.TimeUnit.Hour);
        }

        private void btnAddOneDay_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ForwardClock(BO.TimeUnit.Day);
        }

        private void btnAddOneMonth(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ForwardClock(BO.TimeUnit.Month);
        }

        private void btnAddOneYear(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ForwardClock(BO.TimeUnit.Year);
        }

        private void clockObserver()
        {
            if (_mutex.CheckAndSetLoadInProgressOrRestartRequired())
                return;
            Dispatcher.BeginInvoke(async () =>
            {
                // The actual work to be done on the UI thread
                CurrentTime = s_bl.Admin.GetClock();

                // After completing the work, check if a restart was requested
                if (await _mutex.UnsetLoadInProgressAndCheckRestartRequested())
                    clockObserver();
            });
        }

        private void configObserver()
        {
            if (_mutex.CheckAndSetLoadInProgressOrRestartRequired())
                return;
            Dispatcher.BeginInvoke(async () =>
           {
               // The actual work to be done on the UI thread
               Configuration = s_bl.Admin.GetConfig();

               // After completing the work, check if a restart was requested
               if (await _mutex.UnsetLoadInProgressAndCheckRestartRequested())
                   configObserver();
           });
        }

        private void amountObserver()
        {
            if (_mutex.CheckAndSetLoadInProgressOrRestartRequired())
                return;
            Dispatcher.BeginInvoke(async () =>
            {
                // The actual work to be done on the UI thread
                endStatus = s_bl.Order.OrderAmountsByStatus(_userId);

                // After completing the work, check if a restart was requested
                if (await _mutex.UnsetLoadInProgressAndCheckRestartRequested())
                    amountObserver();
            });
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CurrentTime = s_bl.Admin.GetClock();
            Configuration = s_bl.Admin.GetConfig();
            s_bl.Admin.AddClockObserver(clockObserver);
            s_bl.Admin.AddConfigObserver(configObserver);
            s_bl.Order.AddObserver(amountObserver);
        }

        private async void Window_Closed(object sender, EventArgs e)
        {
            if (ModeSimulator)
            {
                await Task.Run(() => s_bl.Admin.StopSimulator());
                ModeSimulator = !ModeSimulator;
            }
            s_bl.Admin.RemoveClockObserver(clockObserver);
            s_bl.Admin.RemoveConfigObserver(configObserver);
            s_bl.Order.RemoveObserver(amountObserver);
        }

        private void btnCourier_Click(object sender, RoutedEventArgs e)
        {
            var existingWindow = Application.Current.Windows.OfType<CourierListWindow>().FirstOrDefault();

            if (existingWindow != null)
            {

                existingWindow.Activate();
                if (existingWindow.WindowState == WindowState.Minimized)
                    existingWindow.WindowState = WindowState.Normal;
            }
            else
            {

                new CourierListWindow(_userId).Show();
            }

        }

        private void btnOrder_Click(object sender, RoutedEventArgs e)
        {
            var existingWindow = Application.Current.Windows.OfType<OrderListWindow>().FirstOrDefault();

            if (existingWindow != null)
            {
                existingWindow.Activate();
                if (existingWindow.WindowState == WindowState.Minimized)
                    existingWindow.WindowState = WindowState.Normal;
            }
            else
            {
                new OrderListWindow(_userId, null).Show();
            }
        }

        private void btnResetDB_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to reset the data base?", "Confirmation", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                foreach (Window window in Application.Current.Windows)
                {
                    if (window != this && !(window is Entrance))
                        window.Close();
                }
                s_bl.Admin.ResetDB();
            }

            endStatus = s_bl.Order.OrderAmountsByStatus(_userId);
            Mouse.OverrideCursor = null;
        }

        private void btnInitializeDB_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to initialize the data base?", "Confirmation", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                foreach (Window window in Application.Current.Windows)
                {
                    if (window != this && !(window is Entrance))
                        window.Close();
                }
                s_bl.Admin.InitializeDB();
            }

            endStatus = s_bl.Order.OrderAmountsByStatus(_userId);
            Mouse.OverrideCursor = null;
        }

        private void UpdateConfig_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
            s_bl.Admin.SetConfig(Configuration);
            Mouse.OverrideCursor = null;
        }

        private void btnDeliverdList_Click(object sender, RoutedEventArgs e)
        {
            new OrderListWindow(_userId, BO.OrderStatus.Delivered).Show();
        }

        private void btnRefusesList_Click(object sender, RoutedEventArgs e)
        {
            new OrderListWindow(_userId, BO.OrderStatus.Refuses).Show();
        }

        private void btnCanceledList_Click(object sender, RoutedEventArgs e)
        {
            new OrderListWindow(_userId, BO.OrderStatus.Canceled).Show();
        }

        private void btnNoRespondList_Click(object sender, RoutedEventArgs e)
        {
            new OrderListWindow(_userId, BO.OrderStatus.NoRespond).Show();
        }

        private void btnNoFailedList_Click(object sender, RoutedEventArgs e)
        {
            new OrderListWindow(_userId, BO.OrderStatus.Faild).Show();
        }

        private void btnInProcessList_Click(object sender, RoutedEventArgs e)
        {
            new OrderListWindow(_userId, BO.OrderStatus.InProcess).Show();
        }

        private void btnPendingList_Click(object sender, RoutedEventArgs e)
        {
            new OrderListWindow(_userId, BO.OrderStatus.Pending).Show();
        }

        private async void btnStartStopSimulator_Click(object sender, RoutedEventArgs e)
        {

            if (!ModeSimulator)
            {
                s_bl.Admin.StartSimulator(Interval);
                ModeSimulator = !ModeSimulator;
            }
            else
            {
                await Task.Run(() => s_bl.Admin.StopSimulator());
                ModeSimulator = !ModeSimulator;
            }
        }
    }
}