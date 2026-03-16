using PL.Courier;
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
    /// Interaction logic for CourierListWindow.xaml
    /// </summary>
    public partial class CourierListWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        static int _userId;
        private readonly ObserverMutex _mutex = new();
        public BO.CourierInList? SelectedCourier
        {
            get
            {
                return (BO.CourierInList?)GetValue(SelectedCourierProperty);
            }
            set
            {
                SetValue(SelectedCourierProperty, value);
            }
        }

        public static readonly DependencyProperty SelectedCourierProperty =
DependencyProperty.Register("SelectedCourier", typeof(BO.CourierInList), typeof(CourierListWindow), new PropertyMetadata(null));

        /// <summary>
        /// courier list
        /// </summary>
        public IEnumerable<BO.CourierInList> CourierList
        {
            get { return (IEnumerable<BO.CourierInList>)GetValue(CourierListProperty); }
            set { SetValue(CourierListProperty, value); }
        }

        /// <summary>
        /// the filter that is selected
        /// </summary>
        public bool? SelectedActiveFilter
        {
            get { return (bool?)GetValue(SelectedActiveFilterProperty); }
            set { SetValue(SelectedActiveFilterProperty, value); }
        }

        public static readonly DependencyProperty SelectedActiveFilterProperty =
            DependencyProperty.Register("SelectedActiveFilter", typeof(bool?), typeof(CourierListWindow));

        // Using a DependencyProperty as the backing store for CourierList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CourierListProperty =
            DependencyProperty.Register("CourierList", typeof(IEnumerable<BO.CourierInList>), typeof(CourierListWindow), new PropertyMetadata(null));

        public BO.SortCourierByProperty Courier { get; set; } = BO.SortCourierByProperty.None;

        /// <summary>
        /// ctor
        /// </summary>
        public CourierListWindow(int userId)
        {
            InitializeComponent();
            _userId = userId;
        }

        /// <summary>
        /// sort combo box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBox_SortCourierProperty(object sender, SelectionChangedEventArgs e)
        {
            QueryCourierList();
        }

        /// <summary>
        /// courier list
        /// </summary>
        private void QueryCourierList()
        {
            int? selectedId = SelectedCourier?.Id;
            CourierList = s_bl?.Courier.GetCourierList(_userId, SelectedActiveFilter, Courier)!;

            if (selectedId != null)
                SelectedCourier = CourierList.FirstOrDefault(c => c.Id == selectedId);
        }

        /// <summary>
        /// courier list observer
        /// </summary>
        private void CourierListObserver()
        {
            if (_mutex.CheckAndSetLoadInProgressOrRestartRequired())
                return;
            _ = Dispatcher.BeginInvoke(async () =>
            {
                // Do the actual work
                QueryCourierList();

                // Check if a restart was requested while we were working
                if (await _mutex.UnsetLoadInProgressAndCheckRestartRequested())
                    CourierListObserver();
            });
        }

        /// <summary>
        /// adds an observer 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            s_bl.Courier.AddObserver(CourierListObserver);
            s_bl.Admin.AddClockObserver(CourierListObserver);
        }
        /// <summary>
        /// removes an observer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, EventArgs e)
        {
            s_bl.Courier.RemoveObserver(CourierListObserver);
            s_bl.Admin.RemoveClockObserver(CourierListObserver);
        }
        /// <summary>
        /// combo box change 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ActiveFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            QueryCourierList();
        }

        /// <summary>
        /// double click to update
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CourierList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedCourier != null)
                new CourierWindow(_userId, SelectedCourier.Id).Show();
        }

        /// <summary>
        /// shows window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            new CourierWindow(_userId).Show();
            QueryCourierList();
        }

        /// <summary>
        /// confirms that user wants to delete courier and deletes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show($"Are you sure you want to delete {SelectedCourier.Name}?",
                                    "Confirm Delete", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                    s_bl.Courier.DeleteCourier(_userId, SelectedCourier.Id);
                    Mouse.OverrideCursor = null;
                    QueryCourierList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK);
                }
            }
        }
    }

}
