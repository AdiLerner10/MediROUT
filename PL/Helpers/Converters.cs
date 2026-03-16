using BlApi;
using BO;
using DalApi;
using DO;
using FontAwesome.WPF;
using PL.Order;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Navigation;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PL.Helpers;

/// <summary>
/// A converter that claims what will be the content on the vutton
/// </summary>
public class ConvertUpdateToTrue : IValueConverter
{
    public object Convert(object value, Type targetType, object paremeter, CultureInfo culture)
    {
        string mode = (string)value;
        if (mode == "Add")
            return false;
        if (mode == "Update")
            return true;
        return false;
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }


}

/// <summary>
/// a converter that claims the vissibility of a current based on the sent "mode"
/// </summary>
public class ConvertUpdateToVisibility : IValueConverter
{
    public object Convert(object value, Type targetType, object paremeter, CultureInfo culture)
    {
        string mode = (string)value;
        if (mode == "Add")
            return Visibility.Collapsed;
        if (mode == "Update")
            return Visibility.Visible;
        return Visibility.Collapsed;
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converter for a spesific filter combo box vissibility
/// </summary>
public class ComboVissibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        switch ((OrderInListProperty)value)
        {
            case OrderInListProperty.OrderType:
            case OrderInListProperty.InOrOut:
            case OrderInListProperty.ScheduleStatus:
                return Visibility.Visible;
            default:
                return Visibility.Collapsed;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// a converter for the textBox in the OrderListView, it changes the textBox vissibility to visible only if the selectedItem in the filter comboBox matches the requierments.
/// </summary>
public class OrderFilterToVisibilityConverter : IValueConverter
{
    // הפונקציה שמחזירה Visibility לפי הערך שנבחר
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is OrderInListProperty filter)
        {
            // כאן רשימת הערכים שמאפשרים להראות את ה-TextBox
            if (filter == OrderInListProperty.RemainingTime ||
                filter == OrderInListProperty.NumberOfDeliveries ||
                filter == OrderInListProperty.AirDistance ||
                filter == OrderInListProperty.TotalTime)
                return Visibility.Visible;
        }
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// a converter that claims the filter collection comboBox
/// </summary>
public class FilterCollectionConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {

        switch ((OrderInListProperty)value)
        {
            case OrderInListProperty.OrderType:
                return Enum.GetValues(typeof(BO.OrderType)).Cast<BO.OrderType>();
            case OrderInListProperty.InOrOut:
                return Enum.GetValues(typeof(BO.InOrOut)).Cast<BO.InOrOut>();
            case OrderInListProperty.ScheduleStatus:
                return Enum.GetValues(typeof(ScheduleStatus)).Cast<ScheduleStatus>();
            default:
                return Binding.DoNothing; ;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// a converter that claims the vissibility of a textBlock by a chosen filter
/// </summary>
public class TextBlockVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null) return Visibility.Collapsed;
        switch ((OrderInListProperty)value)
        {
            case OrderInListProperty.AirDistance:
            case OrderInListProperty.NumberOfDeliveries:
                return Visibility.Visible;
            default:
                return Visibility.Collapsed;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// a filter that put the wanted value to be inserted in the textbox, by a chosen filter.
/// </summary>
public class FilterChooseToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        switch ((OrderInListProperty)value)
        {
            case OrderInListProperty.AirDistance:
                return "Air distance";
            case OrderInListProperty.NumberOfDeliveries:
                return "number of deliveries";
            case OrderInListProperty.None:
            default:
                return "error";
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// a converter that returns a value with multiValueConverter
/// </summary>
public class FilterSelectedItemToValueConverter : IMultiValueConverter
{
    public static OrderInListProperty tempProp = OrderInListProperty.None;
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values[0] != null)
            tempProp = (OrderInListProperty)values[0];
        return null;
    }


    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// a converter that returns the color of the row by type.
/// </summary>
public class RowColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        switch ((BO.OrderType)value)
        {
            case BO.OrderType.Medicine:
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFB3BA"));
            case BO.OrderType.Machines:
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#BAE1FF"));
            case BO.OrderType.OperatingRoomEquipment:
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#BAFFC9"));
            case BO.OrderType.MobilityEquipment:
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFBA"));
            case BO.OrderType.SterilazationEquipment:
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E1CCFF"));
            case BO.OrderType.HomeTests:
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFDFBA"));
            case BO.OrderType.None:
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F2F2F2"));
            default:
                return Brushes.White;
        }

    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}




/// <summary>
/// checks if courier can be deleted and if so return visibility.visible
/// </summary>
public class ConvertCourierCanBeDeletedToVisibility : IValueConverter
{
    static readonly IBl s_bl = BlApi.Factory.Get();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {

        if (value is CourierInList courier)
        {
            try
            {

                bool canDelete = s_bl.Courier.CheckIfCanDelete(courier.Id);
                return canDelete ? Visibility.Visible : Visibility.Collapsed;
            }
            catch
            {
                return Visibility.Collapsed; // Default to hidden if there's an error
            }
        }
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// converts an enum in to words with spaces
/// </summary>
public class ConvertEnumToWords : IValueConverter
{
    public object Convert(object value, Type targetType, object paremeter, CultureInfo culture)
    {
        if (value == null) return "";
        // Add spaces before capital letters except the first
        return Regex.Replace(value.ToString(), "(?<!^)([A-Z])", " $1");
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// a converter that returns the amount filled in by remaining time.
/// </summary>
public class RemainingTimeColorConverer : IValueConverter
{
    static readonly IBl s_bl = BlApi.Factory.Get();
    public object Convert(object value, Type targetType, object paremeter, CultureInfo culture)
    {
        TimeSpan risk = s_bl.Admin.GetConfig().RiskRange;
        if ((TimeSpan)value <= TimeSpan.Zero)
            return 0;
        if ((TimeSpan)value <= risk)
            return 10;
        if ((TimeSpan)value <= risk + TimeSpan.FromDays(4))
            return 20;
        if ((TimeSpan)value <= risk + TimeSpan.FromDays(6))
            return 30;
        if ((TimeSpan)value <= risk + TimeSpan.FromDays(8))
            return 40;
        if ((TimeSpan)value <= risk + TimeSpan.FromDays(10))
            return 50;

        if ((risk + TimeSpan.FromDays(10)).TotalDays <( ((TimeSpan)value).TotalDays) && (((TimeSpan)value).TotalDays < s_bl.Admin.GetConfig().MaxTimeWindow.TotalDays))
            return 80;
        return 100;
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// checks if id has an error and if so returns true
/// </summary>
public class IdToErrorConverter : IValueConverter
{
    static readonly IBl s_bl = BlApi.Factory.Get();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null || value.ToString() == "0" || string.IsNullOrWhiteSpace(value.ToString()))
        {
            return false;
        }

        if (int.TryParse(value.ToString(), out int id))
        {
            try
            {
                s_bl.Courier.ValidateCourierId(id);
                return false;
            }
            catch
            {
                return true;
            }
        }
        return true;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}


/// <summary>
/// checks if phone number has an error and if so returns true
/// </summary>
public class PhoneToErrorConverter : IValueConverter
{
    static readonly IBl s_bl = BlApi.Factory.Get();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        string phone = value as string;

        // Don't show red if the box is empty (initial state)
        if (string.IsNullOrWhiteSpace(phone)) return false;

        try
        {
            // Call your BL function (e.g., ValidatePhone)
            s_bl.Courier.ValidateCourierPhoneNumber(phone);
            return false; // Valid
        }
        catch
        {
            return true; // Invalid (turn border red)
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// checks if email has an error and if so returns true
/// </summary>
public class EmailToErrorConverter : IValueConverter
{
    static readonly IBl s_bl = BlApi.Factory.Get();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        string phone = value as string;

        // Don't show red if the box is empty (initial state)
        if (string.IsNullOrWhiteSpace(phone)) return false;

        try
        {
            // Call your BL function (e.g., ValidatePhone)
            s_bl.Courier.ValidateCourierEmail(phone);
            return false; // Valid
        }
        catch
        {
            return true; // Invalid (turn border red)
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// checks if password has an error and if so returns true
/// </summary>
public class PasswordToErrorconverter : IValueConverter
{
    static readonly IBl s_bl = BlApi.Factory.Get();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        string phone = value as string;

        // Don't show red if the box is empty (initial state)
        if (string.IsNullOrWhiteSpace(phone)) return false;

        try
        {
            // Call your BL function (e.g., ValidatePhone)
            s_bl.Courier.ValidateCourierPassword(phone);
            return false; // Valid
        }
        catch
        {
            return true; // Invalid (turn border red)
        }

    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// checks if courier has an order in order to make view button visible 
/// </summary>
public class OrderExistsToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value != null ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class PasswordVissibleConverter : IValueConverter
{
    static readonly IBl s_bl = BlApi.Factory.Get();
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (!s_bl.Courier.ValidateCourierPassword((string)value))
            return Brushes.LightBlue;
        else
            return Brushes.Black;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// a cinverter that returns the vissibility of the "cancle" button if the order can be cancled.
/// </summary>
public class VissibilityToCancelConverter : IValueConverter
{
    static readonly IBl s_bl = BlApi.Factory.Get();
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        int orderId = (int)value;
        if (s_bl.Order.CanBeCanceled(orderId))
            return Visibility.Visible;
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// a converter that enables the cancel button if the order can be canceled
/// </summary>
public class canBeCanceledToEnabledConverter : IValueConverter
{
    static readonly IBl s_bl = BlApi.Factory.Get();
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        int orderId = (int)value;
        if (s_bl.Order.CanBeCanceled(orderId))
            return true;
        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}


/// <summary>
/// checks if order is closed and we are in update in order to make update button diabled
/// </summary>
public class OrderClosedToIsEnabledUpdateConvertor : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        string mode = values[0] as string;
        if (values[1] is BO.OrderStatus status)
        {
            if (mode == "Add")
                return true;
            if (status == BO.OrderStatus.Pending || status == BO.OrderStatus.InProcess || status == BO.OrderStatus.NoRespond || status == BO.OrderStatus.Faild)
                return true;
        }

        return false;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// a converter that turns the textbox red if the id is not valid
/// </summary>
public class IdValidationColorConverter : IValueConverter
{
    static readonly IBl s_bl = BlApi.Factory.Get();
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value != null)
        {
            int id = (int)value;

            try
            {
                s_bl.Courier.ValidateCourierId(id);
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC986EA"));
            }
            catch
            {
                return Brushes.Black;
            }
        }
        return Brushes.Black;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }


}

/// <summary>
///  enables the button if the order exists
/// </summary>
public class OrderExistsToenabledConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        bool isOrderNull = values[0] == null;
        bool isActiveCourier = values[1] is bool b && b == true;
        return (isOrderNull && isActiveCourier);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// a converter that sets the color of the courier row depending on the vehicle type
/// </summary>
public class VehicleRowColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        switch ((BO.Vehicle)value)
        {
            case BO.Vehicle.Car:
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFB3BA")); // ורוד דובדבן בהיר

            case BO.Vehicle.Motorcycle:
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#BAE1FF")); // תכלת שמיים קלאסי

            case BO.Vehicle.Bicycle:
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#BAFFC9")); // סגול לילך נוכח

            case BO.Vehicle.OnFoot:
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFBA")); // ירוק מנטה עמוק
            default:
                return Brushes.White;
        }

    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// changes the text on the start/stop button according to the simulator mode
/// </summary>
public class startStopSimulatorTextConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if ((bool)value)
            return "Stop";
        return "Start";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// changes the icon on the button of the simulator to start/stop depending on the mode
/// </summary>
public class startStopSimulatorIconConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (bool)value ? FontAwesomeIcon.Pause : FontAwesomeIcon.Play;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// enables the interval box when the simulator is running
/// </summary>
public class IsEnabledIntervalConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return !(bool)value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// makes the loading icon visible if the window is loading
/// </summary>
public class visibilityToLoadedIcon : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is null)
            return Visibility.Visible;
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// makes the combo boxes enabled if the window is loading
/// </summary>
public class enabledToLoadedconvertor : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is null)
            return false;
        return true;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// if the remaining time is at risk returns red, otherwise return green.
/// </summary>
public class RemainingTimeColorValueConverer : IValueConverter
{
    static readonly IBl s_bl = BlApi.Factory.Get();
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        TimeSpan risk = s_bl.Admin.GetConfig().RiskRange;
        if (((TimeSpan)value).TotalDays < risk.TotalDays && ((TimeSpan)value).TotalDays > 0)
            return "#FF0000";
        return "#4CAF50";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}




