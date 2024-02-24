using System.Globalization;
using System.Windows;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            // Create a new CultureInfo instance
            CultureInfo culture = new CultureInfo("en-US");

            // Customize the date format
            culture.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd";

            // Set the culture for the entire application
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }
    }
}