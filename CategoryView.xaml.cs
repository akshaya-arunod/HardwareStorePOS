using System.Windows;
using System.Windows.Controls;
using HardwareStore.Models;
using HardwareStore.ViewModels;
using MySql.Data.MySqlClient;

namespace HardwareStore.Views
{
    public partial class CategoryView : UserControl
    {
        private CategoryViewModel _viewModel;
        public CategoryView()
        {
            InitializeComponent();

            _viewModel = new CategoryViewModel();
            DataContext = _viewModel;

            // Bind the ItemsControl's ItemsSource to the ViewModel's Categories collection
            // CategoryItemsControl.ItemsSource = _viewModel.Categories;
        }

        private void AddCategory_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddCategoryWindow();
            addWindow.Owner = Window.GetWindow(this); // set owner to current window

            bool? result = addWindow.ShowDialog();

            if (result == true)
            {
                (this.DataContext as CategoryViewModel)?.LoadCategories();
            }
        }

        private void EditCategory_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var category = button.DataContext as CategorySummary; // <-- Change here

            if (category == null)
            {
                MessageBox.Show("Selected category is invalid.");
                return;
            }

            var editWindow = new EditCategoryWindow(category);
            bool? result = editWindow.ShowDialog();

            if (result == true)
            {
                (this.DataContext as CategoryViewModel)?.LoadCategories();
            }
        }

        private void DeleteCategory_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var category = button.DataContext as CategorySummary; // <-- Change here

            if (category == null)
            {
                MessageBox.Show("Selected category is invalid.");
                return;
            }

            var confirm = MessageBox.Show(
                $"Are you sure you want to delete category '{category.Name}'?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (confirm == MessageBoxResult.Yes)
            {
                string connectionString = "server=localhost;uid=root;pwd=;database=pos_hardware_store";

                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    var cmd = new MySqlCommand("DELETE FROM categories WHERE Id = @id", conn);
                    cmd.Parameters.AddWithValue("@id", category.Id);
                    cmd.ExecuteNonQuery();
                }

                (this.DataContext as CategoryViewModel)?.LoadCategories();
            }
        }
    }
}
