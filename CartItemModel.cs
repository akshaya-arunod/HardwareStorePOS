using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareStore.ViewModels
{
    public class CartItemModel : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string Name { get; set; }

        private int _quantity;
        public int Quantity
        {
            get => _quantity;
            set
            {
                if (_quantity != value)
                {
                    _quantity = value;
                    OnPropertyChanged(nameof(Quantity));
                    OnPropertyChanged(nameof(Total));
                    OnPropertyChanged(nameof(DiscountAmount));
                    OnPropertyChanged(nameof(ValueAfterDiscount));
                }
            }
        }

        public decimal Price { get; set; }
        public decimal DiscountPercent { get; set; }  

        public decimal Total => Quantity * Price;

        // Calculate discount amount for the quantity
        public decimal DiscountAmount => Total * (DiscountPercent / 100m);

        // Value after discount (what the customer pays for this item)
        public decimal ValueAfterDiscount => Total - DiscountAmount;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }


}
