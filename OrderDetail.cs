using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Wang_Gloria_HW5.Models
{
    public class OrderDetail
    {
        //Order Detail ID
        public Int32 OrderDetailID { get; set; }

        //Quantity
        [Display(Name = "Order quantity:")]
        [Required(ErrorMessage = "You must specify the quantity")]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a valid quantity number")]
        public Int32 Quantity{ get; set; }

        //Product Price
        [Display(Name = "Product Price:")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public Decimal ProductPrice { get; set; }

        //Extended Price
        [Display(Name = "Extended Price:")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public Decimal ExtendendedPrice { get; set; }

        //Navigational Properties
        public Order Order { get; set; }

        public Product Product { get; set; }
    }
}
