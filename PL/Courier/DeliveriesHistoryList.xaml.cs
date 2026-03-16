using BO;
using DO;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace PL.Courier
{

    public partial class DeliveriesHistoryList : Window
    {
        private readonly int _courierId;
        private readonly int _userId;
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="courierId"></param>
        public DeliveriesHistoryList(int userId, int courierId)
        {
            _courierId = courierId;
            _userId = userId;
            InitializeComponent();
            queryClosedDeliveryList();
        }

        /// <summary>
        /// closed deliveries dp
        /// </summary>
        public IEnumerable<BO.ClosedDeliveryInList> ClosedDeliveries
        {
            get { return (IEnumerable<BO.ClosedDeliveryInList>)GetValue(ClosedDeliveriesProperty); }
            set { SetValue(ClosedDeliveriesProperty, value); }
        }

        public static readonly DependencyProperty ClosedDeliveriesProperty =
            DependencyProperty.Register("ClosedDeliveries", typeof(IEnumerable<BO.ClosedDeliveryInList>),
                typeof(DeliveriesHistoryList), new PropertyMetadata(null));


        public BO.OrderType SelectedFilter { get; set; } = BO.OrderType.None;
        public BO.ClosedDeliveryProperty DeliverySort { get; set; } = BO.ClosedDeliveryProperty.None;

        /// <summary>
        /// selected delivery dp
        /// </summary>
        public BO.ClosedDeliveryInList SelectedDelivery
        {
            get { return (BO.ClosedDeliveryInList)GetValue(SelectedDeliveryProperty); }
            set { SetValue(SelectedDeliveryProperty, value); }
        }

        public static readonly DependencyProperty SelectedDeliveryProperty =
            DependencyProperty.Register("SelectedDelivery", typeof(BO.ClosedDeliveryInList),
                typeof(DeliveriesHistoryList), new PropertyMetadata(null));

        /// <summary>
        /// query
        /// </summary>
        private void queryClosedDeliveryList()
            => ClosedDeliveries = (DeliverySort == BO.ClosedDeliveryProperty.None) ?
                s_bl?.Order.GetClosedOrders(_userId, _courierId, SelectedFilter, null)! :
                s_bl?.Order.GetClosedOrders(_userId, _courierId, SelectedFilter, DeliverySort)!;


        private void ClosedDeliveryListObserver() => queryClosedDeliveryList();

        /// <summary>
        /// adds an observer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            s_bl.Order.AddObserver(ClosedDeliveryListObserver);
            s_bl.Admin.AddClockObserver(ClosedDeliveryListObserver);
        }
        /// <summary>
        /// removes observer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, EventArgs e)
        {
            s_bl.Order.RemoveObserver(ClosedDeliveryListObserver);
            s_bl.Admin.RemoveClockObserver(ClosedDeliveryListObserver);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            queryClosedDeliveryList();
        }

        /// <summary>
        /// shows the couriers current orders details window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewDetails_Click(object sender, RoutedEventArgs e)
        {
            new ViewOrderWindow(SelectedDelivery.OrderId, _courierId).Show();
        }
    }
}
