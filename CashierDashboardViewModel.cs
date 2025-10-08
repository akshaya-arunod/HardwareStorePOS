using HardwareStore.Helpers; // Adjust to your namespace
using HardwareStore.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace HardwareStore.ViewModels
{
    internal class CashierDashboardViewModel : INotifyPropertyChanged
    {
        private readonly string connectionString = "server=localhost;uid=root;pwd=;database=pos_hardware_store";

        public ObservableCollection<ItemModel> Items { get; } = new ObservableCollection<ItemModel>();
        public ObservableCollection<CartItemModel> CartItems { get; } = new ObservableCollection<CartItemModel>();

        public ICommand AddToCartCommand { get; }
        public RelayCommand<CartItemModel> IncreaseQuantityCommand { get; }
        public RelayCommand<CartItemModel> DecreaseQuantityCommand { get; }
        public RelayCommand<CartItemModel> RemoveFromCartCommand { get; }

        private List<ItemModel> _allItems = new List<ItemModel>();



        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged(nameof(SearchText));

                }
            }
        }

        public CashierDashboardViewModel()
        {
            LoadItemsFromDatabase();

            AddToCartCommand = new RelayCommand<ItemModel>(AddToCart);
            IncreaseQuantityCommand = new RelayCommand<CartItemModel>(IncreaseQuantity);
            DecreaseQuantityCommand = new RelayCommand<CartItemModel>(DecreaseQuantity);
            RemoveFromCartCommand = new RelayCommand<CartItemModel>(RemoveFromCart);

            UpdateCartSummary();
        }

        public void LoadItemsFromDatabase()
        {
            _allItems.Clear();

            using (var connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = "SELECT Id, Name, Price, Stock, DiscountPercent, SKU FROM Items";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var item = new ItemModel
                            {
                                Id = reader.GetInt32("Id"),
                                Name = reader.GetString("Name"),
                                Price = reader.GetDecimal("Price"),
                                Stock = reader.GetInt32("Stock"),
                                DiscountPercent = reader.GetDecimal("DiscountPercent"),
                                SKU = reader.GetString("SKU")
                            };
                            _allItems.Add(item);
                        }
                    }

                    FilterItems();  // Show all initially
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to load items: " + ex.Message);
                }
            }
        }

        public void HandleSearchOrBarcode()
        {
            string input = SearchText?.Trim();

            if (string.IsNullOrEmpty(input))
            {
                FilterItems();
                return;
            }

            // Check for exact barcode match
            var item = _allItems.FirstOrDefault(i => i.SKU.Equals(input, StringComparison.OrdinalIgnoreCase));
            if (item != null)
            {
                AddToCart(item);
                SearchText = string.Empty; // Clear search box
                FilterItems();             // Optionally reload all items
            }
            else
            {
                // Not found as barcode, now try as name
                var matches = _allItems.Where(i => i.Name.IndexOf(input, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
                if (matches.Any())
                {
                    FilterItems(); // Search by name
                }
                else
                {
                    MessageBox.Show("No item found with this name or barcode.", "Item Not Found", MessageBoxButton.OK, MessageBoxImage.Warning);
                    SearchText = string.Empty;
                    FilterItems();
                }
            }
        }



        private void FilterItems()
        {
            Items.Clear();

            var filtered = string.IsNullOrWhiteSpace(SearchText)
                ? _allItems
                : _allItems.Where(i => i.Name.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0).ToList();

            foreach (var item in filtered)
            {
                Items.Add(item);
            }
        }

        private void AddToCart(ItemModel item)
        {
            var existing = CartItems.FirstOrDefault(ci => ci.Id == item.Id);
            if (existing != null)
            {
                existing.Quantity++;
            }
            else
            {
                var cartItem = new CartItemModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    Price = item.Price,
                    DiscountPercent = item.DiscountPercent,
                    Quantity = 1
                };

                cartItem.PropertyChanged += CartItem_PropertyChanged;

                CartItems.Add(cartItem);
            }

            UpdateCartSummary();
        }

        public void AddItemByBarcode(string barcode)
        {
            var item = _allItems.FirstOrDefault(i => i.SKU == barcode);
            if (item != null)
            {
                AddToCart(item);
            }
            else
            {
                MessageBox.Show("Item with this barcode not found.");
            }
        }

        private void CartItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CartItemModel.Quantity) ||
                e.PropertyName == nameof(CartItemModel.DiscountPercent))
            {
                UpdateCartSummary();
            }
        }

        private void IncreaseQuantity(CartItemModel cartItem)
        {
            if (cartItem == null) return;

            cartItem.Quantity++;
            UpdateCartSummary();
        }

        private void DecreaseQuantity(CartItemModel cartItem)
        {
            if (cartItem == null) return;

            if (cartItem.Quantity > 1)
            {
                cartItem.Quantity--;
            }
            else
            {
                CartItems.Remove(cartItem);
            }
            UpdateCartSummary();
        }

        private void RemoveFromCart(CartItemModel cartItem)
        {
            if (cartItem == null) return;

            CartItems.Remove(cartItem);
            UpdateCartSummary();
        }

        private decimal _subtotal;
        public decimal Subtotal
        {
            get => _subtotal;
            set
            {
                if (_subtotal != value)
                {
                    _subtotal = value;
                    OnPropertyChanged(nameof(Subtotal));
                }
            }
        }

        private decimal _tax;
        public decimal Tax
        {
            get => _tax;
            set
            {
                if (_tax != value)
                {
                    _tax = value;
                    OnPropertyChanged(nameof(Tax));
                }
            }
        }

        private decimal _total;
        public decimal Total
        {
            get => _total;
            set
            {
                if (_total != value)
                {
                    _total = value;
                    OnPropertyChanged(nameof(Total));
                }
            }
        }

        private decimal _discount;
        public decimal Discount
        {
            get => _discount;
            set
            {
                if (_discount != value)
                {
                    _discount = value;
                    OnPropertyChanged(nameof(Discount));
                }
            }
        }


        private void UpdateCartSummary()
        {
            Subtotal = CartItems.Sum(item => item.Total);
            Discount = CartItems.Sum(item => item.DiscountAmount);
            Total = Subtotal - Discount;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


        public void ClearCart()
        {
            CartItems.Clear();
            UpdateCartSummary(); // Recalculate totals after clearing
        }




        public int CurrentUserId { get; set; } // Set this when cashier logs in

        public bool SaveSale(string paymentMethod, string customerName = "")
        {
            if (!CartItems.Any())
            {
                MessageBox.Show("Cart is empty. Cannot save sale.");
                return false;
            }

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // 1. Insert Sale header
                        string insertSaleQuery = @"
                    INSERT INTO Sales (UserId, CustomerName, Total, Tax)
                    VALUES (@userId, @customerName, @total, @tax);
                    SELECT LAST_INSERT_ID();";
                        var saleCmd = new MySqlCommand(insertSaleQuery, connection, transaction);
                        saleCmd.Parameters.AddWithValue("@userId", CurrentUserId);
                        saleCmd.Parameters.AddWithValue("@customerName", customerName);
                        saleCmd.Parameters.AddWithValue("@total", Total);
                        saleCmd.Parameters.AddWithValue("@tax", Tax);

                        long saleId = Convert.ToInt64(saleCmd.ExecuteScalar());

                        // 2. Insert SaleItems and update stock
                        foreach (var cartItem in CartItems)
                        {
                            // Insert into SaleItems
                            string insertItemQuery = @"
                        INSERT INTO SaleItems (SaleId, ItemId, Quantity, Price, LineTotal)
                        VALUES (@saleId, @itemId, @quantity, @price, @lineTotal)";
                            var itemCmd = new MySqlCommand(insertItemQuery, connection, transaction);
                            itemCmd.Parameters.AddWithValue("@saleId", saleId);
                            itemCmd.Parameters.AddWithValue("@itemId", cartItem.Id);
                            itemCmd.Parameters.AddWithValue("@quantity", cartItem.Quantity);
                            itemCmd.Parameters.AddWithValue("@price", cartItem.Price);
                            itemCmd.Parameters.AddWithValue("@lineTotal", cartItem.ValueAfterDiscount);
                            itemCmd.ExecuteNonQuery();

                            // Update inventory
                            string updateStockQuery = "UPDATE Items SET Stock = Stock - @quantity WHERE Id = @itemId";
                            var stockCmd = new MySqlCommand(updateStockQuery, connection, transaction);
                            stockCmd.Parameters.AddWithValue("@quantity", cartItem.Quantity);
                            stockCmd.Parameters.AddWithValue("@itemId", cartItem.Id);
                            stockCmd.ExecuteNonQuery();
                        }

                        // 3. Insert Payment
                        string insertPaymentQuery = @"
                    INSERT INTO Payments (SaleId, PaymentMethod, Amount)
                    VALUES (@saleId, @paymentMethod, @amount)";
                        var paymentCmd = new MySqlCommand(insertPaymentQuery, connection, transaction);
                        paymentCmd.Parameters.AddWithValue("@saleId", saleId);
                        paymentCmd.Parameters.AddWithValue("@paymentMethod", paymentMethod);
                        paymentCmd.Parameters.AddWithValue("@amount", Total);
                        paymentCmd.ExecuteNonQuery();

                        // Commit transaction
                        transaction.Commit();

                        // Clear cart after successful payment
                        ClearCart();

                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show("Failed to complete sale: " + ex.Message);
                        return false;
                    }
                }
            }
        }





    }


}
