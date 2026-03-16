using PL.Helpers;
using System;
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
using System.Windows.Shapes;

namespace PL.Courier
{
    /// <summary>
    /// Interaction logic for CourierMainWindow.xaml
    /// </summary>
    public partial class CourierMainWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        static int _userId;
        private readonly ObserverMutex _mutex = new();
        public CourierMainWindow(int CourierId)
        {
            InitializeComponent();
            CurrentCourier = s_bl.Courier.GetCourier(s_bl.Admin.GetConfig().ManagerId, CourierId);
            _userId = CourierId;
            SelectedEndStatus = BO.OrderStatus.Delivered;
        }


        /// <summary>
        /// selected status dependancy property
        /// </summary>
        public BO.OrderStatus SelectedEndStatus
        {
            get { return (BO.OrderStatus)GetValue(SelectedEndStatusProperty); }
            set { SetValue(SelectedEndStatusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedEndStatus.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedEndStatusProperty =
            DependencyProperty.Register("SelectedEndStatus", typeof(BO.OrderStatus), typeof(CourierMainWindow), new PropertyMetadata(BO.OrderStatus.InProcess));


        /// <summary>
        /// current currier dp
        /// </summary>
        public BO.Courier? CurrentCourier
        {
            get { return (BO.Courier)GetValue(CurrentCourierProperty); }
            set { SetValue(CurrentCourierProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentCourier.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentCourierProperty =
            DependencyProperty.Register("CurrentCourier", typeof(BO.Courier), typeof(CourierMainWindow), new PropertyMetadata(null));

        /// <summary>
        /// observer
        /// </summary>
        private void CourierObserver()
        {

            if (_mutex.CheckAndSetLoadInProgressOrRestartRequired())
                return;
            _ = Dispatcher.BeginInvoke(async () =>
            {
                // Do the actual work
                int id = CurrentCourier!.Id;
                CurrentCourier = null;
                CurrentCourier = s_bl.Courier.GetCourier(_userId, id);

                // Check if a restart was requested while we were working
                if (await _mutex.UnsetLoadInProgressAndCheckRestartRequested())
                    CourierObserver();
            });

        }

        /// <summary>
        /// adds an observer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, EventArgs e)
        {
            if (CurrentCourier?.Id != 0)
                s_bl.Courier.AddObserver(CurrentCourier.Id, CourierObserver);
            s_bl.Order.AddObserver(CourierObserver);
            s_bl.Admin.AddClockObserver(CourierObserver);
        }

        /// <summary>
        /// closes the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, EventArgs e)
        {
            if (CurrentCourier?.Id != 0)
                s_bl.Courier.RemoveObserver(CurrentCourier.Id, CourierObserver);
            s_bl.Order.RemoveObserver(CourierObserver);
            s_bl.Admin.RemoveClockObserver(CourierObserver);
        }

        /// <summary>
        /// shows the couriers history window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnHistory_Click(object sender, RoutedEventArgs e)
        {
            new DeliveriesHistoryList(_userId, CurrentCourier.Id).Show();
        }

        /// <summary>
        /// shows the order to handle window 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPickOrder_Click(object sender, RoutedEventArgs e)
        {
            new OrderToHandle(_userId, CurrentCourier!.Id).Show();
        }

        /// <summary>
        /// updates the courier
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                s_bl.Courier.UpdateCourier(_userId, CurrentCourier);
                MessageBox.Show("Courier updated successfully", "update", MessageBoxButton.OK);
                s_bl.Courier.MaxDiststance(CurrentCourier);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK);
            }
        }

        /// <summary>
        /// ends the couriers current order
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEndDelivery_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                s_bl.Order.OrderCompletion(_userId, CurrentCourier.Id, CurrentCourier.ExistingOrder.OrderId, SelectedEndStatus);
                Mouse.OverrideCursor = null;
                CurrentCourier.ExistingOrder = null;
            }
            catch { }
        }

        /// <summary>
        /// shows the details of the couriers current order
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnView_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                new ViewOrderWindow(_userId, CurrentCourier.ExistingOrder.OrderId, CurrentCourier.Id).Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK);
            }
        }

        /// <summary>
        /// filtered enum for end type combo box
        /// </summary>
        public IEnumerable<BO.OrderStatus> FilteredStatusList
        {
            get
            {
                yield return BO.OrderStatus.NoRespond;
                yield return BO.OrderStatus.Delivered;
                yield return BO.OrderStatus.Refuses;
            }
        }
    }
}
