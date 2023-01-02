using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Wang_Gloria_HW5.Models
{
    public class Supplier
    {
        //Primary Key
        public Int32 SupplierID { get; set; }

        //Supplier's name
        [Display(Name = "Name:")]
        [Required(ErrorMessage = "Supplier's name is required.")]
        public String SupplierName { get; set; }

        //Email
        [Display(Name = "Email:")]
        [Required(ErrorMessage = "Supplier's email is required.")]
        [DataType(DataType.EmailAddress)]
        public String Email { get; set; }

        //PhoneNumber
        [Display(Name = "Phone Number:")]
        [Required(ErrorMessage = "Supplier's phone number is required.")]
        [DataType(DataType.PhoneNumber)]
        public String PhoneNumber { get; set; }

        //Navigational Properties
        public List<Product> Products { get; set; }

        public Supplier()
        {
            if (Products == null)
            {
                Products = new List<Product>();
            }
        }
    }
}
