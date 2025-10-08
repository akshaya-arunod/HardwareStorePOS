using System;
using System.Windows;
using System.Windows.Controls;
using HardwareStore.Models;
using HardwareStore.ViewModels;
using MySql.Data.MySqlClient;

namespace HardwareStore.Views
{
    public partial class InventoryView : UserControl
    {
        private InventoryViewModel _viewModel;

        public InventoryView()
        {
            InitializeComponent();
            _viewModel = new InventoryViewModel();
            DataContext = _viewModel;
        }

        private void AddItemButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var addWindow = new AddItemWindow();
            bool? result = addWindow.ShowDialog(); // Show the AddItem window modally

            if (result == true)
            {
                _viewModel.LoadItems(); // Refresh the item list if the user clicked Add
            }
        }

        private void EditItem_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.DataContext is Item selectedItem)
            {
                var editWindow = new EditItemWindow(selectedItem)
                {
                    Owner = Window.GetWindow(this)
                };

                bool? result = editWindow.ShowDialog();

                if (result == true)
                {
                    // Reload items after edit
                    _viewModel.LoadItems();
                }
            }
        }


        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.DataContext is Item selectedItem)
            {
                var result = MessageBox.Show(
                    $"Are you sure you want to delete \"{selectedItem.Name}\"?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        string connectionString = "server=localhost;uid=root;pwd=;database=pos_hardware_store";
                        string query = "DELETE FROM Items WHERE SKU = @SKU";

                        using (var conn = new MySqlConnection(connectionString))
                        {
                            conn.Open();
                            using (var cmd = new MySqlCommand(query, conn))
                            {
                                cmd.Parameters.AddWithValue("@SKU", selectedItem.SKU);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        MessageBox.Show("Item deleted successfully.", "Deleted", MessageBoxButton.OK, MessageBoxImage.Information);
                        _viewModel.LoadItems(); // Refresh the list
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error deleting item: " + ex.Message);
                    }
                }
            }
        }


        private void ItemsListView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var listView = sender as ListView;
            if (listView?.View is GridView gridView)
            {
                double totalAvailableWidth = listView.ActualWidth - SystemParameters.VerticalScrollBarWidth - 10;
                if (totalAvailableWidth <= 0)
                    return;

                // Define fixed widths (for example: SKU=100, Stock=80), rest will stretch proportionally
                double skuWidth = 100;
                double stockWidth = 80;
                double actionsWidth = 160;

                double fixedColumns = skuWidth + stockWidth + actionsWidth;

                double remainingWidth = totalAvailableWidth - fixedColumns;

                if (remainingWidth < 100)
                    remainingWidth = 100; // minimal space for main columns

                // Now assign widths
                gridView.Columns[0].Width = remainingWidth * 0.6;  // Item Name
                gridView.Columns[1].Width = skuWidth;              // SKU
                gridView.Columns[2].Width = remainingWidth * 0.25; // Category
                gridView.Columns[3].Width = remainingWidth * 0.15; // Price
                gridView.Columns[4].Width = stockWidth;            // Stock
                gridView.Columns[5].Width = actionsWidth;          // Actions
            }
        }


    }


}
