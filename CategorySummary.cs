using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareStore.Models
{
    public class CategorySummary
    {
        public int Id { get; set; }                // Category ID
        public string Name { get; set; }           // Category Name
        public int ItemCount { get; set; }         // Number of items in this category
    }
}
