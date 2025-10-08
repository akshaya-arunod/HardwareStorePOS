using HardwareStore.Models;
using MySql.Data.MySqlClient;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace HardwareStore.ViewModels
{
    public class DashboardViewModel : INotifyPropertyChanged
    {
        private string connectionString = "server=localhost;uid=root;pwd=;database=pos_hardware_store";

        private DashboardStats _stats = new DashboardStats();
        public DashboardStats Stats
        {
            get => _stats;
            set
            {
                _stats = value;
                OnPropertyChanged(nameof(Stats));
            }
        }

        private ObservableCollection<LowStockItem> _lowStockItems = new ObservableCollection<LowStockItem>();
        public ObservableCollection<LowStockItem> LowStockItems
        {
            get => _lowStockItems;
            set
            {
                _lowStockItems = value;
                OnPropertyChanged(nameof(LowStockItems));
            }
        }

        public DashboardViewModel()
        {
            LoadStats();
            LoadLowStockItems();
        }

        public void LoadStats()
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                var cmd = new MySqlCommand(@"
                    SELECT 
                        COUNT(*) AS TotalTransactions,
                        COALESCE(SUM(Total), 0) AS TotalSales,
                        COALESCE(AVG(Total), 0) AS AverageTransaction
                    FROM Sales", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        Stats.TotalSales = reader.GetDecimal("TotalSales");
                        Stats.TotalTransactions = reader.GetInt32("TotalTransactions");
                        Stats.AverageTransaction = reader.GetDecimal("AverageTransaction");
                    }
                }
            }
        }

        public void LoadLowStockItems()
        {
            LowStockItems.Clear();

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                var cmd = new MySqlCommand("SELECT Name, Stock FROM Items WHERE Stock <= 10 ORDER BY Stock ASC LIMIT 50", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        LowStockItems.Add(new LowStockItem
                        {
                            Name = reader.GetString("Name"),
                            Stock = reader.GetInt32("Stock")
                        });
                    }
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
