using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace Wang_Gloria_HW5.Models
{
    public class AppUser:IdentityUser
    {
        //TODO: Add additional user fields here
        //First name is provided as an example
        //User's first name
        [Required(ErrorMessage = "User's first name is required.")]
        [Display(Name = "First Name:")]
        public string FirstName { get; set; }

        //User's last name
        [Required(ErrorMessage = "User's last name is required.")]
        [Display(Name = "Last Name:")]
        public string LastName { get; set; }

        //User Name
        [Display(Name = "User Name:")]
        public String FullName
        {
            get { return FirstName + " " + LastName; }
        }

        //Navigational Properties
        public List<Order> Orders { get; set; }
        public AppUser()
        {
            if (Orders == null)
            {
                Orders = new List<Order>();
            }

        }
    }
}
