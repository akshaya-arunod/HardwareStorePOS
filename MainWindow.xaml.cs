using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MySql.Data.MySqlClient;
using HardwareStore.Data;
using HardwareStore.Views;


namespace HardwareStore
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Set initial placeholder
            UsernameTextBox.Text = "username";
            UsernameTextBox.Foreground = Brushes.Gray;
        }

        // Username placeholder
        private void UsernameTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (UsernameTextBox.Text == "username")
            {
                UsernameTextBox.Text = "";
                UsernameTextBox.Foreground = Brushes.Black;
            }
        }

        private void UsernameTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(UsernameTextBox.Text))
            {
                UsernameTextBox.Text = "username";
                UsernameTextBox.Foreground = Brushes.Gray;
            }
        }

        // Password placeholder
        private void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            PasswordPlaceholder.Visibility = Visibility.Collapsed;
        }

        private void PasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(PasswordBox.Password))
            {
                PasswordPlaceholder.Visibility = Visibility.Visible;
            }
        }

        // Close button
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // 🚀 Login Button Click Logic
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text.Trim();
            string password = PasswordBox.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || username == "username")
            {
                MessageBox.Show("Please enter valid username and password.");
                return;
            }

            using (var conn = DB.GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = "SELECT * FROM Users WHERE Username = @username";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string hashedPassword = reader.GetString("PasswordHash");
                                string role = reader.GetString("Role");

                                if (BCrypt.Net.BCrypt.Verify(password, hashedPassword))
                                {
                                    MessageBox.Show("Login successful!");

                                    // 🧑‍💼 Open Dashboard Based on Role
                                    if (role == "Admin")
                                    {
                                        var adminDashboard = new AdminDashboard();
                                        adminDashboard.Show();
                                    }
                                    else if (role == "Cashier")
                                    {
                                        var cashierDashboard = new CashierDashboard();
                                        cashierDashboard.Show();
                                    }

                                    this.Close();
                                }
                                else
                                {
                                    MessageBox.Show("Incorrect password.");
                                }
                            }
                            else
                            {
                                MessageBox.Show("User not found.");
                            }
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Database error: " + ex.Message);
                }
            }
        }
    }
}
