using System.Windows.Controls;
using Wpf.Ui.Appearance;

namespace RRuleGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            ApplicationThemeManager.Apply(this);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.SelectedItem is LanguageEnum language)
            {
                var grid = MainContent;
                Content = null;
                Content = grid;
            }
        }
    }
}