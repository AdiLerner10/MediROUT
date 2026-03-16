using PL.Courier;
using PL.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

namespace PL
{
    /// <summary>
    /// Interaction logic for Entrance.xaml
    /// </summary>
    public partial class Entrance : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public Entrance()
        {
            InitializeComponent();
        }



        public int UserId
        {
            get { return (int)GetValue(UserIdProperty); }
            set { SetValue(UserIdProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UserId.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UserIdProperty =
            DependencyProperty.Register("UserId", typeof(int), typeof(Entrance), new PropertyMetadata(0));

        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Password.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register("Password", typeof(string), typeof(Entrance), new PropertyMetadata(""));

        public bool IsPasswordVisible
        {
            get { return (bool)GetValue(IsPasswordVisibleProperty); }
            set { SetValue(IsPasswordVisibleProperty, value); }
        }

        public static readonly DependencyProperty IsPasswordVisibleProperty =
            DependencyProperty.Register("IsPasswordVisible", typeof(bool), typeof(Entrance), new PropertyMetadata(false));

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (s_bl.Courier.ValidateCourierId(UserId) && s_bl.Courier.ValidateCourierPassword(Password))
                {
                    string user = s_bl.Courier.CourierEntrance(UserId, Password);
                    if (user == "Manager")
                    {
                        WindowShow();
                    }
                    if (user == "Courier")
                    {
                        CourierMainWindow main = new CourierMainWindow(UserId);
                        main.Show(); ;
                    }
                }
                else
                {
                    MessageBox.Show("This User Doesn't exist", "Login Fail", MessageBoxButton.OK);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Validation Error", MessageBoxButton.OK);
            }
        }

        private void WindowShow()
        {
            var existingWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();

            if (existingWindow != null)
            {
                existingWindow.Activate();
                if (existingWindow.WindowState == WindowState.Minimized)
                    existingWindow.WindowState = WindowState.Normal;
            }
            else
            {
                new MainWindow(UserId).Show();
            }
        }

    }



    /// <summary>
    /// A helper class containing attached properties to enable Two-Way data binding
    /// of the <see cref="PasswordBox.Password"/> property in WPF.
    /// </summary>
    public static class PasswordHelper
    {

        // Define the Attached Property to hold the bound password value (ViewModel source).
        public static readonly DependencyProperty BoundPasswordProperty =
            DependencyProperty.RegisterAttached("BoundPassword",
            typeof(string),
            typeof(PasswordHelper),
            new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnBoundPasswordChanged, null, false, UpdateSourceTrigger.PropertyChanged));

        // Define the helper Attached Property used to activate the binding logic (Attach/Detach events).
        public static readonly DependencyProperty AttachProperty =
            DependencyProperty.RegisterAttached("Attach",
            typeof(bool),
            typeof(PasswordHelper),
            new PropertyMetadata(false, OnAttachChanged));

        // Getters / Setters for the BoundPassword Attached Property.
        public static string GetBoundPassword(DependencyObject d) => (string)d.GetValue(BoundPasswordProperty);
        public static void SetBoundPassword(DependencyObject d, string value) => d.SetValue(BoundPasswordProperty, value);
        public static bool GetAttach(DependencyObject d) => (bool)d.GetValue(AttachProperty);
        public static void SetAttach(DependencyObject d, bool value) => d.SetValue(AttachProperty, value);


        // Event Logic
        private static void OnAttachChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PasswordBox passwordBox)
            {
                if ((bool)e.OldValue == false && (bool)e.NewValue == true)
                {
                    // Subscribe to the PasswordChanged event when Attach is set to true.
                    passwordBox.PasswordChanged += PasswordChanged;
                }
                else if ((bool)e.OldValue == true && (bool)e.NewValue == false)
                {
                    // Unsubscribe from the event when Attach is set to false.
                    passwordBox.PasswordChanged -= PasswordChanged;
                }
            }
        }

        private static void PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                // Update the BoundPassword Attached Property with the new password string.
                SetBoundPassword(passwordBox, passwordBox.Password);
            }
        }

        private static void OnBoundPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PasswordBox passwordBox)
            {
                // remove the event handler to prevent infinite loop
                passwordBox.PasswordChanged -= PasswordChanged;

                string newPassword = (string)e.NewValue;

                // update the PasswordBox's Password only if it differs from the new value
                if (passwordBox.Password != newPassword)
                {
                    passwordBox.Password = newPassword ?? string.Empty;
                }

                // return the event handler
                passwordBox.PasswordChanged += PasswordChanged;
            }
        }
    }
}
