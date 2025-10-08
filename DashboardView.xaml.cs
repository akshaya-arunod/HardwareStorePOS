using HardwareStore.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Linq;

namespace HardwareStore.Views
{
    public partial class DashboardView : UserControl
    {
        private DashboardViewModel _viewModel;
        private bool showAllLowStock = false;

        public DashboardView()
        {
            InitializeComponent();

            _viewModel = new DashboardViewModel();
            this.DataContext = _viewModel;

            // Show only top 4 low stock items initially
            ShowCollapsedLowStockItems();
        }

        private void ShowCollapsedLowStockItems()
        {
            // Display only first 4 items in the collapsed view
            var topItems = new ObservableCollection<Models.LowStockItem>(_viewModel.LowStockItems.Take(4));
            LowStockCollapsedList.ItemsSource = topItems;

            LowStockToggleButton.Content = "View All";
            showAllLowStock = false;
        }

        private void ShowFullLowStockItems()
        {
            // Display all low stock items
            LowStockCollapsedList.ItemsSource = _viewModel.LowStockItems;

            LowStockToggleButton.Content = "View Less";
            showAllLowStock = true;
        }

        private void ToggleLowStockView_Click(object sender, RoutedEventArgs e)
        {
            if (showAllLowStock)
                ShowCollapsedLowStockItems();
            else
                ShowFullLowStockItems();
        }
    }
}
