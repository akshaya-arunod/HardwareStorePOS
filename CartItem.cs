using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareStore.Models
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
                    OnPropertyChanged(nameof(DiscountPercent));
                    OnPropertyChanged(nameof(DiscountAmount));
                    OnPropertyChanged(nameof(DiscountedTotal));
                }
            }
        }

        public decimal Price { get; set; }
        public decimal DiscountPercent { get; set; }  // New property

        public decimal Total => Quantity * Price;

        public decimal DiscountAmount => Total * DiscountPercent / 100m;

        public decimal DiscountedTotal => Total - DiscountAmount;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

}
