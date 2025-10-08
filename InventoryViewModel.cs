using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using MySql.Data.MySqlClient;
using HardwareStore.Models;
using HardwareStore.Views;
using System.Linq;

namespace HardwareStore.ViewModels
{
    public class InventoryViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Item> _allItems = new ObservableCollection<Item>();

        private ObservableCollection<Item> _items = new ObservableCollection<Item>();
        public ObservableCollection<Item> Items
        {
            get => _items;
            set
            {
                _items = value;
                OnPropertyChanged(nameof(Items));
            }
        }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
                ApplyFilters(); // ✅ live filtering
            }
        }



        public ObservableCollection<Category> Categories { get; set; } = new ObservableCollection<Category>();

        private Category _selectedCategory;
        public Category SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
                OnPropertyChanged(nameof(SelectedCategory));
                // TODO: trigger filter update here
            }
        }

        public InventoryViewModel()
        {
            
            LoadItems();
        }



        public void LoadItems()
        {
            var tempList = new ObservableCollection<Item>(); // ✅ Declare first

            string connectionString = "server=localhost;uid=root;pwd=;database=pos_hardware_store";
            string query = @"
                SELECT i.Name, i.SKU, i.Price, i.Stock, c.Name AS CategoryName, i.DiscountPercent
                FROM Items i
                LEFT JOIN Categories c ON i.CategoryId = c.Id"; 
            
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
                            tempList.Add(new Item
                            {
                                Name = reader.GetString("Name"),
                                SKU = reader.GetString("SKU"),
                                Price = reader.GetDecimal("Price"),
                                Stock = reader.GetInt32("Stock"),
                                DiscountPercent = reader.GetDecimal("DiscountPercent"),
                                CategoryName = reader.IsDBNull(reader.GetOrdinal("CategoryName"))
                                    ? "Uncategorized"
                                    : reader.GetString("CategoryName")
                            });
                        }
                    }
                }

                _allItems = tempList; // ✅ Now assign to the master list
                Items = new ObservableCollection<Item>(_allItems); // ✅ And display it
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Error loading items: " + ex.Message);
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));

        private void ApplyFilters()
        {
            if (_allItems == null) return;

            var filtered = _allItems;

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                string lowerSearch = SearchText.ToLower();
                filtered = new ObservableCollection<Item>(
                    _allItems.Where(item =>
                        item.Name != null && item.Name.ToLower().Contains(lowerSearch)
                    )
                );
            }

            Items = filtered;
        }
        




    }
}
