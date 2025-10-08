using System.Collections.ObjectModel;
using System.ComponentModel;
using MySql.Data.MySqlClient;
using HardwareStore.Models;

namespace HardwareStore.ViewModels
{
    public class CategoryViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<CategorySummary> _categories = new ObservableCollection<CategorySummary>();
        public ObservableCollection<CategorySummary> Categories
        {
            get => _categories;
            set
            {
                _categories = value;
                OnPropertyChanged(nameof(Categories));
            }
        }

        public CategoryViewModel()
        {
            LoadCategories();
        }

        public void LoadCategories()
        {
            var tempList = new ObservableCollection<CategorySummary>();

            string connectionString = "server=localhost;uid=root;pwd=;database=pos_hardware_store";

            string query = @"
                SELECT c.Id, c.Name, COUNT(i.Id) AS ItemCount
                FROM Categories c
                LEFT JOIN Items i ON c.Id = i.CategoryId
                GROUP BY c.Id, c.Name";

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
                            tempList.Add(new CategorySummary
                            {
                                Id = reader.GetInt32("Id"),
                                Name = reader.GetString("Name"),
                                ItemCount = reader.GetInt32("ItemCount")
                            });
                        }
                    }
                }

                Categories = tempList;
            }
            catch (System.Exception ex)
            {
                System.Windows.MessageBox.Show("Error loading categories: " + ex.Message);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
