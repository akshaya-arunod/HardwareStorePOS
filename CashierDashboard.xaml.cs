using HardwareStore.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace HardwareStore.Views
{
    public partial class CashierDashboard : Window
    {
        public CashierDashboard()
        {
            InitializeComponent();
     
        }

        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var tb = sender as TextBox;
            if (tb != null && tb.Text == "Search item...")
            {
                tb.Text = "";
                tb.Foreground = Brushes.Black;
            }
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var tb = sender as TextBox;
            if (tb != null && string.IsNullOrWhiteSpace(tb.Text))
            {
                tb.Text = "Search item...";
                tb.Foreground = Brushes.Gray;
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var vm = DataContext as CashierDashboardViewModel;
            if (vm != null)
            {
                vm.HandleSearchOrBarcode();
            }
        }

       
        private void btnProceedPayment_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is CashierDashboardViewModel vm)
            {
                if (!vm.CartItems.Any())
                {
                    MessageBox.Show("Cart is empty. Please add items before proceeding.");
                    return;
                }

                decimal subtotal = vm.CartItems.Sum(i => i.Total);
                decimal discount = vm.CartItems.Sum(i => i.DiscountAmount);
                decimal total = vm.CartItems.Sum(i => i.ValueAfterDiscount);

                var paymentWindow = new PaymentWindow(vm.CartItems.ToList(), subtotal, discount, total);

                if (paymentWindow.ShowDialog() == true)
                {
                    vm.ClearCart(); // Clear cart and reset totals
                }
            }
        }
    }
}

