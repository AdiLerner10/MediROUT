using PL.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
    /// Interaction logic for ViewOrder.xaml
    /// </summary>
    public partial class ViewOrderWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        static int _userId;
        private readonly ObserverMutex _mutex = new();
        public ViewOrderWindow(int userId, int orderId, int courierId = 0)
        {
            _userId = userId;
            courier = s_bl.Courier.GetCourier(s_bl.Admin.GetConfig().ManagerId, courierId);
            order = s_bl.Order.GetOrder(s_bl.Admin.GetConfig().ManagerId, orderId);
            currentOrderInProgress = new BO.OrderInProgress (order, courier){};
            InitializeComponent();
        }

        /// <summary>
        /// order query
        /// </summary>
        /// <param name="courierId"></param>
        private async Task queryViewOrder(int courierId)
        {
            if (currentOrderInProgress != null)
            {
                BO.Courier courier = s_bl.Courier.GetCourier(_userId, courierId);
                double speed;
                switch (courier.Vehicle)
                {
                    case BO.Vehicle.Car:
                        speed = (double)s_bl.Admin.GetConfig().CarAvgSpeedKMH;
                        break;
                    case BO.Vehicle.Motorcycle:
                        speed = (double)s_bl.Admin.GetConfig().MotorcycleAvgSpeedKMH;
                        break;
                    case BO.Vehicle.Bicycle:
                        speed = (double)s_bl.Admin.GetConfig().BicycleAvgSpeedKMH;
                        break;
                    case BO.Vehicle.OnFoot:
                        speed = (double)s_bl.Admin.GetConfig().OnFootAvgSpeedKMH;
                        break;
                    default:
                        speed = (double)s_bl.Admin.GetConfig().CarAvgSpeedKMH!;
                        break;
                }
                currentOrderInProgress = await s_bl.Order.GetOrderInProgress(currentOrderInProgress.OrderId, speed);
            }
        }

        /// <summary>
        /// adds an observer
        /// </summary>
        private async void viewOrderObserver()
        {
            await Dispatcher.BeginInvoke(async () =>
            {
                // The actual work to be done on the UI thread

                await queryViewOrder(courier.Id);

                // After completing the work, check if a restart was requested
                if (await _mutex.UnsetLoadInProgressAndCheckRestartRequested())
                    viewOrderObserver();
            });
        }

        private static BO.Courier courier;

        /// <summary>
        /// current order dp
        /// </summary>
        public BO.OrderInProgress? currentOrderInProgress
        {
            get { return (BO.OrderInProgress?)GetValue(currentOrderInProgressProperty); }
            set { SetValue(currentOrderInProgressProperty, value); }
        }



        public BO.Order order
        {
            get { return (BO.Order)GetValue(orderProperty); }
            set { SetValue(orderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for order.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty orderProperty =
            DependencyProperty.Register("order", typeof(BO.Order), typeof(ViewOrderWindow), new PropertyMetadata(null));



        // Using a DependencyProperty as the backing store for currentOrderInProgress.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty currentOrderInProgressProperty =
            DependencyProperty.Register("currentOrderInProgress", typeof(BO.OrderInProgress), typeof(ViewOrderWindow), new PropertyMetadata(null));

        /// <summary>
        /// closes the window 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// adds an observer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            s_bl.Order.AddObserver(viewOrderObserver);
            s_bl.Admin.AddClockObserver(viewOrderObserver);
            await queryViewOrder(courier.Id);
        }

        /// <summary>
        /// removes observer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, EventArgs e)
        {
            s_bl.Order.RemoveObserver(viewOrderObserver);
            s_bl.Admin.RemoveClockObserver(viewOrderObserver);
        }
    }
}
