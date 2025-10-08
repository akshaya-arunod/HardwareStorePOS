using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HardwareStore.Views
{
    public partial class AdminDashboard : Window
    {
        public AdminDashboard()
        {
            InitializeComponent();
            MainContentControl.Content = new DashboardView(); // Set dashboard as default view
        }

        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var tb = sender as TextBox;
            if (tb.Text == "Search by item or SKU...")
            {
                tb.Text = "";
                tb.Foreground = Brushes.Black;
            }
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var tb = sender as TextBox;
            if (string.IsNullOrWhiteSpace(tb.Text))
            {
                tb.Text = "Search by item or SKU...";
                tb.Foreground = Brushes.Gray;
            }
        }

        private void DashboardButton_Click(object sender, RoutedEventArgs e)
        {
            MainContentControl.Content = new DashboardView();
        }
        private void InventoryButton_Click(object sender, RoutedEventArgs e)
        {
            MainContentControl.Content = new InventoryView();
        }

        private void CategoriesButton_Click(object sender, RoutedEventArgs e)
        {
            MainContentControl.Content = new CategoryView();
        }

        private void ReportsButton_Click(object sender, RoutedEventArgs e)
        {
            // Create a new ReportsView instance
            ReportsView reportsView = new ReportsView();

            // Set the ContentControl content to ReportsView
            MainContentControl.Content = reportsView;
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            var settingsView = new Views.SettingsView();

            // Wrap SettingsView to align it left
            var wrapper = new Grid
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                Children = { settingsView }
            };

            MainContentControl.Content = wrapper;
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                "Are you sure you want to log out?",
                "Confirm Logout",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var MainWindow = new MainWindow();
                MainWindow.Show();
                this.Close();
            }
            // else: do nothing
        }


    }
}
