using BO;
using DO;
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

namespace PL.Delivery
{
    /// <summary>
    /// Interaction logic for DeliveryListWindow.xaml
    /// </summary>
    public partial class DeliveryListWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        static int _userId;
        private readonly ObserverMutex _mutex = new();
        public DeliveryListWindow(int userId, int orderId)
        {
            _userId = userId;
            CurrentOrder = s_bl.Order.GetOrder(_userId, orderId)!;
            Deliveries = CurrentOrder.DeliveryPerOrderInList!;
            InitializeComponent();
        }

        /// <summary>
        /// current order dp
        /// </summary>
        public BO.Order CurrentOrder
        {
            get { return (BO.Order)GetValue(CurrentOrderProperty); }
            set { SetValue(CurrentOrderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentOrder.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentOrderProperty =
            DependencyProperty.Register("CurrentOrder", typeof(BO.Order), typeof(DeliveryListWindow), new PropertyMetadata(null));


        /// <summary>
        /// deliveries dp
        /// </summary>
        public IEnumerable<DeliveryPerOrderInList> Deliveries
        {
            get { return (IEnumerable<DeliveryPerOrderInList>)GetValue(DeliveriesProperty); }
            set { SetValue(DeliveriesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DeliveriesProperty =
            DependencyProperty.Register("MyProperty", typeof(IEnumerable<DeliveryPerOrderInList>), typeof(DeliveryListWindow), new PropertyMetadata(null));

        /// <summary>
        /// calls list
        /// </summary>
        /// <param name="orderId"></param>
        private void queryDeliveryList(int orderId)
        {
            CurrentOrder = s_bl.Order.GetOrder(_userId, orderId)!;
            Deliveries = CurrentOrder.DeliveryPerOrderInList!;
        }

        /// <summary>
        /// observer
        /// </summary>
        private void DeliveryListObserver()
        {

            if (_mutex.CheckAndSetLoadInProgressOrRestartRequired())
                return;
            _ = Dispatcher.BeginInvoke(async () =>
            {
                // Do the actual work
                queryDeliveryList(CurrentOrder.Id);

                // Check if a restart was requested while we were working
                if (await _mutex.UnsetLoadInProgressAndCheckRestartRequested())
                    DeliveryListObserver();
            });

        }

        /// <summary>
        /// adds observer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            s_bl.Order.AddObserver(DeliveryListObserver);
            s_bl.Admin.AddClockObserver(DeliveryListObserver);
        }

        /// <summary>
        /// removes observer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, EventArgs e)
        {
            s_bl.Order.RemoveObserver(DeliveryListObserver);
            s_bl.Admin.RemoveClockObserver(DeliveryListObserver);
        }
    }
}
