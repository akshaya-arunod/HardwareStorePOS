using HardwareStore.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace HardwareStore.Views
{
    /// <summary>
    /// Interaction logic for EditCategoryWindow.xaml
    /// </summary>
    public partial class EditCategoryWindow : Window
    {
        private CategorySummary _category;
        public EditCategoryWindow(CategorySummary category)
        {
            InitializeComponent();
            _category = category;
            CategoryNameBox.Text = category.Name;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            string newName = CategoryNameBox.Text.Trim();

            if (!string.IsNullOrWhiteSpace(newName))
            {
                string connectionString = "server=localhost;uid=root;pwd=;database=pos_hardware_store";

                using (var conn = new MySql.Data.MySqlClient.MySqlConnection(connectionString))
                {
                    conn.Open();
                    var cmd = new MySql.Data.MySqlClient.MySqlCommand("UPDATE categories SET Name = @name WHERE Id = @id", conn);
                    cmd.Parameters.AddWithValue("@name", newName);
                    cmd.Parameters.AddWithValue("@id", _category.Id);
                    cmd.ExecuteNonQuery();
                }

                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Category name cannot be empty.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
