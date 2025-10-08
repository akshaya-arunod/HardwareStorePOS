using System;
using System.Collections.ObjectModel;
using System.Windows;
using HardwareStore.Models;
using MySql.Data.MySqlClient;

namespace HardwareStore.Views
{
    public partial class EditItemWindow : Window
    {
        private Item _originalItem;
        private ObservableCollection<Category> _categories;

        public EditItemWindow(Item item)
        {
            InitializeComponent();
            _originalItem = item;

            LoadCategories();
            PopulateFields();
        }

        private void LoadCategories()
        {
            _categories = new ObservableCollection<Category>();

            string connectionString = "server=localhost;uid=root;pwd=;database=pos_hardware_store";
            string query = "SELECT Id, Name FROM Categories";

            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            _categories.Add(new Category
                            {
                                Id = reader.GetInt32("Id"),
                                Name = reader.GetString("Name")
                            });
                        }
                    }
                }

                CategoryComboBox.ItemsSource = _categories;

                // Select the original category
                foreach (var cat in _categories)
                {
                    if (cat.Name == _originalItem.CategoryName)
                    {
                        CategoryComboBox.SelectedItem = cat;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading categories: " + ex.Message);
            }
        }

        private void PopulateFields()
        {
            ItemNameTextBox.Text = _originalItem.Name;
            SKUTextBox.Text = _originalItem.SKU;
            PriceTextBox.Text = _originalItem.Price.ToString();
            StockTextBox.Text = _originalItem.Stock.ToString();
            DiscountTextBox.Text = _originalItem.DiscountPercent.ToString();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string name = ItemNameTextBox.Text.Trim();
            string priceText = PriceTextBox.Text.Trim();
            string stockText = StockTextBox.Text.Trim();
            string discountText = DiscountTextBox.Text.Trim();
            var selectedCategory = CategoryComboBox.SelectedItem as Category;

            if (string.IsNullOrWhiteSpace(name) ||
                !decimal.TryParse(priceText, out decimal price) ||
                !int.TryParse(stockText, out int stock) ||
                !decimal.TryParse(discountText, out decimal discount) ||  // new validation
                selectedCategory == null)
            {
                MessageBox.Show("Please enter valid values for all fields.");
                return;
            }

            try
            {
                string connectionString = "server=localhost;uid=root;pwd=;database=pos_hardware_store";
                string query = @"
                    UPDATE Items
                    SET Name = @Name, Price = @Price, Stock = @Stock, CategoryId = @CategoryId,DiscountPercent = @DiscountPercent
                    WHERE SKU = @SKU";

                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Name", name);
                        cmd.Parameters.AddWithValue("@Price", price);
                        cmd.Parameters.AddWithValue("@Stock", stock);
                        cmd.Parameters.AddWithValue("@CategoryId", selectedCategory.Id);
                        cmd.Parameters.AddWithValue("@DiscountPercent", discount);
                        cmd.Parameters.AddWithValue("@SKU", _originalItem.SKU); // Use SKU to find the row

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Item updated successfully.");
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating item: " + ex.Message);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
