using BlApi;
using BO;
using DO;
using PL.Delivery;
using PL.Helpers;
using PL.Order;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading;
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
    /// Interaction logic for OrderListWindow.xaml
    /// </summary>
    public partial class OrderListWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        static int _userId;
        private readonly ObserverMutex _mutex = new();
        private BO.OrderStatus? _orderStatus = null;


        public BO.OrderInList? SelectedOrder
        {
            get
            {
                return (BO.OrderInList?)GetValue(SelectedOrderProperty);
            }
            set
            {
                SetValue(SelectedOrderProperty, value);
            }
        }

        public static readonly DependencyProperty SelectedOrderProperty =
DependencyProperty.Register("SelectedOrder", typeof(BO.OrderInList), typeof(OrderListWindow), new PropertyMetadata(null));

        public OrderListWindow(int userId, BO.OrderStatus? status)
        {
            _userId = userId;
            _orderStatus = status;
            InitializeComponent();
        }

        /// <summary>
        /// order list dp
        /// </summary>
        public IEnumerable<BO.OrderInList> OrderList
        {
            get { return (IEnumerable<BO.OrderInList>)GetValue(OrderListProperty); }
            set { SetValue(OrderListProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OrderList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OrderListProperty =
            DependencyProperty.Register("OrderList", typeof(IEnumerable<BO.OrderInList>), typeof(OrderListWindow), new PropertyMetadata(null));



        /// <summary>
        /// property to sort by dp
        /// </summary>
        public BO.OrderInListProperty OrderFilter
        {
            get { return (BO.OrderInListProperty)GetValue(OrderFilterProperty); }
            set { SetValue(OrderFilterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OrderFilter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OrderFilterProperty =
            DependencyProperty.Register("OrderFilter", typeof(BO.OrderInListProperty), typeof(OrderListWindow), new PropertyMetadata(BO.OrderInListProperty.None));



        public BO.OrderInListProperty OrderSort
        {
            get { return (BO.OrderInListProperty)GetValue(OrderSortProperty); }
            set { SetValue(OrderSortProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OrderSort.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OrderSortProperty =
            DependencyProperty.Register("OrderSort", typeof(BO.OrderInListProperty), typeof(OrderListWindow), new PropertyMetadata(BO.OrderInListProperty.None));

        public object FilterSelectedItem
        {
            get { return (object)GetValue(FilterSelectedItemProperty); }
            set { SetValue(FilterSelectedItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FilterSelectedItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FilterSelectedItemProperty =
            DependencyProperty.Register("FilterSelectedItem", typeof(object), typeof(OrderListWindow), new PropertyMetadata(BO.OrderInListProperty.None));



        //public BO.OrderInListProperty OrderFilter { get; set; } = BO.OrderInListProperty.None;

        private async Task queryOrderList()
        {
            int? selectedId = SelectedOrder?.OrderId;
            if (_orderStatus != null && OrderFilter == BO.OrderInListProperty.None)
            {
                OrderList = await s_bl.Order.GetOrdersInListByStatus(_userId, _orderStatus);
                _orderStatus = null;
            }
            else
            {
                if (OrderFilter == BO.OrderInListProperty.None && OrderSort == BO.OrderInListProperty.None)
                    OrderList = await s_bl?.Order.GetOrdersInList(_userId, null, FilterSelectedItem, null)!;
                else if (OrderFilter == BO.OrderInListProperty.None && OrderSort != BO.OrderInListProperty.None)
                    OrderList = await s_bl?.Order.GetOrdersInList(_userId, null, FilterSelectedItem, OrderSort)!;
                else if (OrderFilter != BO.OrderInListProperty.None && OrderSort == BO.OrderInListProperty.None)
                    OrderList = await s_bl?.Order.GetOrdersInList(_userId, OrderFilter, FilterSelectedItem, null)!;
                else
                    OrderList = await s_bl?.Order.GetOrdersInList(_userId, OrderFilter, FilterSelectedItem, OrderSort)!;
            }
            if (selectedId != null)
                SelectedOrder = OrderList.FirstOrDefault(o => o.OrderId == selectedId);
        }

        /// <summary>
        /// observer
        /// </summary>
        private void orderListObserver()
        {
            if (_mutex.CheckAndSetLoadInProgressOrRestartRequired())
                return;
            Dispatcher.BeginInvoke(async () =>
           {
               // The actual work to be done on the UI thread
               await queryOrderList();

               // After completing the work, check if a restart was requested
               if (await _mutex.UnsetLoadInProgressAndCheckRestartRequested())
                   orderListObserver();
           });
        }

        /// <summary>
        /// adds an observer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            orderListObserver();
            s_bl.Order.AddObserver(orderListObserver);
            s_bl.Admin.AddClockObserver(orderListObserver);
        }

        /// <summary>
        /// removes an observer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, EventArgs e)
        {
            s_bl.Order.RemoveObserver(orderListObserver);
            s_bl.Admin.RemoveClockObserver(orderListObserver);
        }

        /// <summary>
        /// combo box change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FilterOrdersChoose_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded) return;
            _orderStatus = null;
            orderListObserver();
        }

        /// <summary>
        /// updates when double clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedOrder != null)
                new OrderWindow(_userId, SelectedOrder.OrderId).Show();
        }

        /// <summary>
        /// goes to add order window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddOrder_Click(object sender, RoutedEventArgs e)
        {
            new OrderWindow(_userId).Show();
        }

        /// <summary>
        /// updates order
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpdateOrder_Click(object sender, RoutedEventArgs e)
        {
            new OrderWindow(_userId, SelectedOrder!.OrderId).Show();
        }

        /// <summary>
        /// opens deliveries window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeliveriesList_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedOrder != null)
            {
                new DeliveryListWindow(_userId, SelectedOrder.OrderId).Show();
            }
        }

        /// <summary>
        /// combo box change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void filterCmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded) return;
            orderListObserver();
        }

        /// <summary>
        /// confirm sort
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSortConfirmation_Click(object sender, RoutedEventArgs e)
        {
            switch (OrderFilter)
            {
                case OrderInListProperty.NumberOfDeliveries:
                case OrderInListProperty.OrderId:
                case OrderInListProperty.DeliveryId:
                    try
                    {
                        int.TryParse((string)FilterSelectedItem, out int resultInt);
                        FilterSelectedItem = resultInt;
                    }
                    catch
                    { MessageBox.Show("Error! the value should be a natural number", "Validation Error", MessageBoxButton.OK); }
                    break;
                case OrderInListProperty.AirDistance:
                    try
                    {
                        double.TryParse((string)FilterSelectedItem, out double resultDouble);
                        FilterSelectedItem = resultDouble;
                    }
                    catch { MessageBox.Show("Error! the value should be a Real number", "Validation Error", MessageBoxButton.OK); }
                    break;
                case OrderInListProperty.RemainingTime:
                case OrderInListProperty.TotalTime:
                    try
                    {
                        TimeSpan.TryParse((string)FilterSelectedItem, out TimeSpan resultTimeSpan);
                        FilterSelectedItem = resultTimeSpan;
                    }
                    catch { MessageBox.Show("Error! expect a time value.", "Validation Error", MessageBoxButton.OK); }
                    break;
                default:
                    FilterSelectedItem = null;
                    break;
            }
            if ((OrderFilter != BO.OrderInListProperty.None))
                orderListObserver();
        }

        /// <summary>
        /// cancels an  order by calling completion function 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancleOrder_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is BO.OrderInList order)
            {
                try
                {
                    Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                    s_bl.Order.Cancel(_userId, order.OrderId);
                    Mouse.OverrideCursor = null;
                }
                catch (blEmailFailedException ex)
                {

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK);
                }
            }
        }

        /// <summary>
        /// filtered combo box
        /// </summary>
        public IEnumerable<BO.OrderInListProperty> FilterOrderList
        {
            get
            {
                yield return BO.OrderInListProperty.OrderType;
                yield return BO.OrderInListProperty.InOrOut;
                yield return BO.OrderInListProperty.AirDistance;
                yield return BO.OrderInListProperty.ScheduleStatus;
                yield return BO.OrderInListProperty.NumberOfDeliveries;
                yield return BO.OrderInListProperty.None;
            }
        }
    }
}
