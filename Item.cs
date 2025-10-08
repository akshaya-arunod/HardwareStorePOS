using System.ComponentModel;

namespace HardwareStore.Models
{
    public class Item : INotifyPropertyChanged
    {
        private string _name;
        private string _sku;
        private int _stock;
        private decimal _price;
        private string _categoryName;
        private decimal _discountPercent;

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(nameof(Name)); }
        }

        public string SKU
        {
            get => _sku;
            set { _sku = value; OnPropertyChanged(nameof(SKU)); }
        }

        public int Stock
        {
            get => _stock;
            set { _stock = value; OnPropertyChanged(nameof(Stock)); }
        }

        public decimal Price
        {
            get => _price;
            set { _price = value; OnPropertyChanged(nameof(Price)); }
        }

        public string CategoryName
        {
            get => _categoryName;
            set { _categoryName = value; OnPropertyChanged(nameof(CategoryName)); }
        }

        public decimal DiscountPercent 
        {
            get => _discountPercent;
            set { _discountPercent = value; OnPropertyChanged(nameof(DiscountPercent)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
