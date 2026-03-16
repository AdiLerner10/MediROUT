using BO;
using PL.Courier;
using PL.Delivery;
using PL.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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

namespace PL.Order
{
    /// <summary>
    /// Interaction logic for OrderWindow.xaml
    /// </summary>
    public partial class OrderWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        static int _userId;
        private readonly ObserverMutex _mutex = new();

        public BO.Order? CurrentOrder
        {
            get { return (BO.Order)GetValue(CurrentOrderProperty); }
            set { SetValue(CurrentOrderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Order.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentOrderProperty =
            DependencyProperty.Register("CurrentOrder", typeof(BO.Order), typeof(OrderWindow), new PropertyMetadata(null));

        /// <summary>
        /// button text dp
        /// </summary>
        public string ButtonText
        {
            get { return (string)GetValue(ButtonTextProperty); }
            set { SetValue(ButtonTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ButtonText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ButtonTextProperty =
            DependencyProperty.Register("ButtonText", typeof(string), typeof(OrderWindow), new PropertyMetadata(null));


        public BO.InOrOut InOrOutFilter { get; set; } = BO.InOrOut.None;
        public BO.OrderType OrderTypeFilter { get; set; } = BO.OrderType.None;
        public BO.ScheduleStatus scheduleStatusFilter { get; set; } = BO.ScheduleStatus.None;
        public int NumberOfDeliveries { get; set; } = 0;

        private void OrderDetailsUpdateObserver()
        {
            int id = CurrentOrder!.Id;
            CurrentOrder = null;

            if (_mutex.CheckAndSetLoadInProgressOrRestartRequired())
                return;
            Dispatcher.BeginInvoke(async () =>
            {
                // The actual work to be done on the UI thread
                CurrentOrder = s_bl.Order.GetOrder(_userId, id);

                // After completing the work, check if a restart was requested
                if (await _mutex.UnsetLoadInProgressAndCheckRestartRequested())
                    OrderDetailsUpdateObserver();
            });
            
        }



        /// <summary>
        /// sets the add/update button
        /// </summary>
        /// <param name="id"></param>
        public OrderWindow(int userId, int id = 0)
        {
            _userId = userId;
            ButtonText = id == 0 ? "Add" : "Update";
            InitializeComponent();
            CurrentOrder = (id != 0) ? s_bl.Order.GetOrder(_userId, id)! :
                new BO.Order() { Id = 0 };
            if (id != 0)
                s_bl.Order.AddObserver(id, OrderDetailsUpdateObserver);
            //NumberOfDeliveries = CurrentOrder.DeliveryPerOrderInList.Count();

        }

        /// <summary>
        /// adds or updates the order
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ButtonText == "Add")
                    s_bl.Order.Add(_userId, CurrentOrder);
                else
                    s_bl.Order.Update(_userId, CurrentOrder);
                MessageBox.Show($"The attempt to {ButtonText} the order succeded");
            }
            catch (blEmailFailedException ex)
            {

            }
            catch (Exception ex)
            {
                MessageBox.Show($"The attempt to {ButtonText} the order failed", ButtonText, MessageBoxButton.OK);

            }
            this.Close();
        }

        /// <summary>
        /// adds observer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, EventArgs e)
        {
            if (CurrentOrder?.Id != 0)
                s_bl.Order.AddObserver(CurrentOrder.Id, OrderDetailsUpdateObserver);
            s_bl.Admin.AddClockObserver(OrderDetailsUpdateObserver);
        }

        /// <summary>
        /// removes observer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, EventArgs e)
        {
            if (CurrentOrder.Id != 0)
                s_bl.Order.RemoveObserver(CurrentOrder.Id, OrderDetailsUpdateObserver);
            s_bl.Admin.RemoveClockObserver(OrderDetailsUpdateObserver);
        }

        /// <summary>
        /// opens orders deliveries
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOpenDeliveries_Click(object sender, RoutedEventArgs e)
        {
            new DeliveryListWindow(_userId, CurrentOrder.Id).Show();
        }
    }
}
