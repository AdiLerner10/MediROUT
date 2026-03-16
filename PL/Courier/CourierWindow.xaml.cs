using PL.Order;
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
using Helpers;
using System.Runtime.CompilerServices;

namespace PL.Courier
{
    /// <summary>
    /// Interaction logic for CourierWindow.xaml
    /// </summary>
    public partial class CourierWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        static int _userId;
        public CourierWindow(int userId, int id = 0)
        {
            _userId = userId;
            ButtonText = id == 0 ? "Add" : "Update";
            InitializeComponent();
            CurrentCourier = (id != 0) ? s_bl.Courier.GetCourier(s_bl.Admin.GetConfig().ManagerId, id)! : new BO.Courier()
            {
                Id = 0,
                Active = true,
                Email = "",
                EmploymentDate = s_bl.Admin.GetClock(),
                MaxPersonalDistance = 0,
                Name = "",
                NumOfProvidedLate = 0,
                NumOfProvidedOnTime = 0,
                Password = "",
                PhoneNumber = "",
                Salary = 35,
                Vehicle = BO.Vehicle.Car,

            };
            if (CurrentCourier!.Id != 0)
                s_bl.Courier.AddObserver(CurrentCourier!.Id, CourierObserver);
        }

        /// <summary>
        /// adds or updates a courier
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAddUpdate_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                if (ButtonText == "Add")
                {
                    s_bl.Courier.AddCourier(_userId, CurrentCourier!);
                    MessageBox.Show("Courier added successfully", "Add", MessageBoxButton.OK);
                }
                else
                {
                    s_bl.Courier.UpdateCourier(_userId, CurrentCourier!);
                    MessageBox.Show("Courier updated successfully", "update", MessageBoxButton.OK);
                }
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK);
            }
        }


        /// <summary>
        /// current courier
        /// </summary>
        public BO.Courier? CurrentCourier
        {
            get { return (BO.Courier?)GetValue(CurrentCourierProperty); }
            set { SetValue(CurrentCourierProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentCourier.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentCourierProperty =
            DependencyProperty.Register("CurrentCourier", typeof(BO.Courier), typeof(CourierWindow), new PropertyMetadata(null));

        /// <summary>
        /// button text depdendancy propeerty
        /// </summary>
        public string ButtonText
        {
            get { return (string)GetValue(ButtonTextProperty); }
            set { SetValue(ButtonTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ButtonText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ButtonTextProperty =
            DependencyProperty.Register("ButtonText", typeof(string), typeof(CourierWindow), new PropertyMetadata(""));

        public CourierListWindow ListWindow { get; set; }

        /// <summary>
        /// observer
        /// </summary>
        private void CourierObserver()
        {
            //int id = CurrentCourier!.Id;
            //CurrentCourier = null;
            //CurrentCourier = s_bl.Courier.GetCourier(_userId, id);
            Dispatcher.BeginInvoke(() =>
            {
                // עכשיו מותר לגשת ל-CurrentCourier כי אנחנו ב-UI Thread
                if (CurrentCourier == null) return;

                int id = CurrentCourier.Id;

                // שליפת הנתונים המעודכנים (שימי לב: אם הפעולה הזו איטית, זה עלול לתקוע את ה-UI לרגע)
                var updatedCourier = s_bl.Courier.GetCourier(_userId, id);

                // עדכון האובייקט
                CurrentCourier = updatedCourier;
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
            s_bl.Admin.RemoveClockObserver(CourierObserver);
        }

        /// <summary>
        /// shows the couriers current order details
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewBtn_click(object sender, RoutedEventArgs e)
        {
            if (CurrentCourier?.ExistingOrder != null)
            {

                new ViewOrderWindow(_userId, CurrentCourier.ExistingOrder.OrderId, CurrentCourier.Id).Show();
            }
            else
            {
                MessageBox.Show("No existing order for this courier.", "Info", MessageBoxButton.OK);
            }
        }


    }

}
