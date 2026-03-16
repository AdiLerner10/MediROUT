using BO;
using PL.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
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
    /// Interaction logic for OrderToHandle.xaml
    /// </summary>
    public partial class OrderToHandle : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        static int _userId;
        private readonly ObserverMutex _mutex = new();
        public OrderToHandle(int userId, int courierId)
        {
            Courier = s_bl.Courier.GetCourier(s_bl.Admin.GetConfig().ManagerId, courierId);
            _userId = userId;
            InitializeComponent();
        }

        /// <summary>
        /// courier dp
        /// </summary>
        public BO.Courier Courier
        {
            get { return (BO.Courier)GetValue(CourierProperty); }
            set { SetValue(CourierProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Courier.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CourierProperty =
            DependencyProperty.Register("Courier", typeof(BO.Courier), typeof(OrderToHandle), new PropertyMetadata(null));


        /// <summary>
        /// open order dp
        /// </summary>
        public IEnumerable<BO.OpenOrderInList> OpenOrders
        {
            get { return (IEnumerable<BO.OpenOrderInList>)GetValue(OpenOrdersProperty); }
            set { SetValue(OpenOrdersProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OpenOrders.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OpenOrdersProperty =
            DependencyProperty.Register("OpenOrders", typeof(IEnumerable<BO.OpenOrderInList>), typeof(OrderToHandle), new PropertyMetadata(null));



        /// <summary>
        /// current order dp
        /// </summary>
        public BO.OpenOrderInList SelectedOrder
        {
            get { return (BO.OpenOrderInList)GetValue(SelectedOrderProperty); }
            set { SetValue(SelectedOrderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedOrder.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedOrderProperty =
            DependencyProperty.Register("SelectedOrder", typeof(BO.OpenOrderInList), typeof(OrderToHandle), new PropertyMetadata(null));


        /// <summary>
        /// order filter dp
        /// </summary>
        public BO.OrderType OrderFilter
        {
            get { return (BO.OrderType)GetValue(OrderFilterProperty); }
            set { SetValue(OrderFilterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OrderFilter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OrderFilterProperty =
            DependencyProperty.Register("OrderFilter", typeof(BO.OrderType), typeof(OrderToHandle), new PropertyMetadata(BO.OrderType.None));

        /// <summary>
        /// order sort dp
        /// </summary>
        public BO.OpenOrderProperty OrderSort
        {
            get { return (BO.OpenOrderProperty)GetValue(OrderSortProperty); }
            set { SetValue(OrderSortProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OrderSort.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OrderSortProperty =
            DependencyProperty.Register("OrderSort", typeof(BO.OpenOrderProperty), typeof(OrderToHandle), new PropertyMetadata(BO.OpenOrderProperty.None));

        /// <summary>
        /// query list
        /// </summary>
        private async Task queryOrderList()
        {
            OpenOrders = null;
            if (OrderFilter == BO.OrderType.None && OrderSort == BO.OpenOrderProperty.None)
                OpenOrders = await s_bl?.Order.GetOpenOrders(Courier.Id, null, null)!;
            else if (OrderFilter == BO.OrderType.None && OrderSort != BO.OpenOrderProperty.None)
                OpenOrders = await s_bl?.Order.GetOpenOrders(Courier.Id, null, OrderSort)!;
            else if (OrderFilter != BO.OrderType.None && OrderSort == BO.OpenOrderProperty.None)
                OpenOrders = await s_bl?.Order.GetOpenOrders(Courier.Id, OrderFilter, null)!;
            else
                OpenOrders = await s_bl?.Order.GetOpenOrders(Courier.Id, OrderFilter, OrderSort)!;
        }

        /// <summary>
        /// observer
        /// </summary>
        private async void OrderListObserver()
        {
            await Dispatcher.BeginInvoke(async () =>
            {
                // The actual work to be done on the UI thread

                await queryOrderList();

                // After completing the work, check if a restart was requested
                if (await _mutex.UnsetLoadInProgressAndCheckRestartRequested())
                    OrderListObserver();
            });
        }

        /// <summary>
        /// adds observer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            s_bl.Order.AddObserver(OrderListObserver);
            await queryOrderList();
        }

        /// <summary>
        /// removes observer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, EventArgs e)
        {
            s_bl.Order.RemoveObserver(OrderListObserver);
        }

        /// <summary>
        /// collects an order
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CollectBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                s_bl.Order.OrderToHandle(_userId, Courier.Id, SelectedOrder.OrderId);
                Mouse.OverrideCursor = null;
                MessageBox.Show("Order assigned succesfully!", "Success", MessageBoxButton.OK);
               
                this.Close();
            }
            catch (blEmailFailedException ex)
            {

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// combo box change 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OrderFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OrderListObserver();
        }

        /// <summary>
        /// combo box change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OrderSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OrderListObserver();
        }

        /// <summary>
        /// changes the enums values only to the relevant.
        /// </summary>
        public IEnumerable<BO.OpenOrderProperty> FilterOpenOrderList
        {
            get
            {
                yield return BO.OpenOrderProperty.ActualDistance;
                yield return BO.OpenOrderProperty.Address;
                yield return BO.OpenOrderProperty.Volume;
                yield return BO.OpenOrderProperty.None;

            }
        }

    }
}
