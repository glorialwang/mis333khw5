using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Wang_Gloria_HW5.Models
{
    public class Order
    {
        private const Decimal TAX_RATE = 0.0825m;

        //Primary Key
        public Int32 OrderID { get; set; }

        //Order Number
        [Display(Name ="Order Number:")]
        public Int32 OrderNumber { get; set; }

        //Order Date
        [Display(Name = "Order Date:")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime OrderDate { get; set; }

        //Order Notes
        [Display(Name = "Order Notes:")]
        public String OrderNotes { get; set; }
        
        // Order Subtotal
        [Display(Name = "Order Subtotal")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public Decimal OrderSubtotal
        {
            get { return OrderDetails.Sum(rd => rd.ExtendendedPrice); }
        }

        //Sales Tax
        [Display(Name = "Sales Tax(8.25%)")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public Decimal SalesTax
        {
            get { return OrderSubtotal * TAX_RATE; }
        }

        //Order Total
        [Display(Name = "Order Total:")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public Decimal OrderTotal
        {
            get { return OrderSubtotal + SalesTax; }
        }

        //Navigational Properties
        public List<OrderDetail> OrderDetails { get; set; }
        public AppUser User { get; set; }

        public Order()
        {
            if (OrderDetails == null)
            {
                OrderDetails = new List<OrderDetail>();
            }
        }
    }
}
