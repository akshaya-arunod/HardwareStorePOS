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
using HardwareStore.ViewModels;



namespace HardwareStore.Views
{
    /// <summary>
    /// Interaction logic for PaymentWindow.xaml
    /// </summary>
    public partial class PaymentWindow : Window
    {
        private readonly List<CartItemModel> _cartItems;
        private readonly decimal _subtotal;
        private readonly decimal _discount;
        private readonly decimal _finalTotal;

        public PaymentWindow(List<CartItemModel> cartItems, decimal subtotal, decimal discount, decimal finalTotal)
        {
            InitializeComponent();

            _cartItems = cartItems ?? new List<CartItemModel>();
            _subtotal = subtotal;
            _discount = discount;
            _finalTotal = finalTotal;

            txtSubtotal.Text = _subtotal.ToString("C");
            txtDiscount.Text = _discount.ToString("C");
            txtFinalTotal.Text = _finalTotal.ToString("C");
        }

        // Use PascalCase for methods
        private void ConfirmPayment_Click(object sender, RoutedEventArgs e)
        {
            var vm = Owner.DataContext as CashierDashboardViewModel;
            if (vm == null)
            {
                MessageBox.Show("Unable to access cashier data.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!decimal.TryParse(txtCashReceived.Text, out decimal cashReceived))
            {
                MessageBox.Show("Please enter a valid cash amount.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (cashReceived < _finalTotal)
            {
                MessageBox.Show("Insufficient cash received.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Save sale to DB and update inventory
            bool success = vm.SaveSale("Cash", txtCustomerName.Text);
            if (!success) return;

            decimal balance = cashReceived - _finalTotal;
            txtBalance.Text = balance.ToString("C");

            MessageBox.Show("Payment successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            DialogResult = true;
            Close();
        }


    }
}
