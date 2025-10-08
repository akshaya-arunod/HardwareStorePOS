using System;
using System.Collections.Generic;
using System.Windows;
using MySql.Data.MySqlClient;

namespace HardwareStore.Views
{
    public partial class AddItemWindow : Window
    {
        public AddItemWindow()
        {
            InitializeComponent();
            LoadCategories();
        }

        private void LoadCategories()
        {
            var categories = new List<Category>();

            string connectionString = "server=localhost;user=root;database=pos_hardware_store;";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT Id, Name FROM Categories";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        categories.Add(new Category
                        {
                            Id = reader.GetInt32("Id"),
                            Name = reader.GetString("Name")
                        });
                    }

                    CategoryComboBox.ItemsSource = categories;
                    CategoryComboBox.DisplayMemberPath = "Name";
                    CategoryComboBox.SelectedValuePath = "Id";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to load categories: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string name = ItemNameTextBox.Text.Trim();
            string sku = SKUTextBox.Text.Trim();
            string priceText = PriceTextBox.Text.Trim();
            string stockText = StockTextBox.Text.Trim();
            var selectedCategory = CategoryComboBox.SelectedItem as Category;
            decimal.TryParse(DiscountTextBox.Text.Trim(), out decimal discount);

            // Validation messages
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Please enter the item name.");
                return;
            }

            if (string.IsNullOrWhiteSpace(sku))
            {
                MessageBox.Show("Please enter the SKU.");
                return;
            }

            if (!decimal.TryParse(priceText, out decimal price) || price < 0)
            {
                MessageBox.Show("Please enter a valid non-negative price (e.g., 1200.50).");
                return;
            }

            if (!int.TryParse(stockText, out int stock) || stock < 0)
            {
                MessageBox.Show("Please enter a valid non-negative stock quantity.");
                return;
            }

            if (selectedCategory == null)
            {
                MessageBox.Show("Please select a category.");
                return;
            }

            // Save to database
            try
            {
                string connectionString = "server=localhost;uid=root;pwd=;database=pos_hardware_store";
                string query = @"
                INSERT INTO Items (Name, SKU, Price, Stock, CategoryId,DiscountPercent, CreatedAt)
                VALUES (@Name, @SKU, @Price, @Stock, @CategoryId,@DiscountPercent, NOW())";

                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Name", name);
                        cmd.Parameters.AddWithValue("@SKU", sku);
                        cmd.Parameters.AddWithValue("@Price", price);
                        cmd.Parameters.AddWithValue("@Stock", stock);
                        cmd.Parameters.AddWithValue("@CategoryId", selectedCategory.Id);
                        cmd.Parameters.AddWithValue("@DiscountPercent", discount);


                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Item added successfully.");
                this.DialogResult = true; // ✅ Notify InventoryView to refresh
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding item: " + ex.Message);
            }
        }

    }

    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
